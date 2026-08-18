using Api.Shared.Clients.OpenApi.Skedular.Booking.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.StripeWebhook.V1;
using Api.Shared.Clients.OpenApi.Skedular.Booking.XeroWebhook.V1;
using Api.Shared.Clients.OpenApi.Skedular.BookingWorkaround.V1;
using Api.Shared.Grpc.Skedular.Booking.Core.V1;
using Api.Shared.Grpc.Skedular.InfrastructureTest.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Aspire.Hosting.Testing;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Encryption;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Random;
using Enterprise.Shared.Telemetry;
using Enterprise.Shared.Telemetry.PropagatorFunctions;
using Enterprise.Shared.Temporal;
using Enterprise.Shared.Temporal.Configurations;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using OpenTelemetry.Context.Propagation;
using Projects;
using Temporalio.Client;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace Booking.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment
        {
            EnvironmentName = Environments.Production,
        };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Booking_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var bookingDbConnectionString = distributedApp.GetConnectionStringAsync("bookingdb").Result;
        var temporalConnectionString = distributedApp.GetConnectionStringAsync("temporal").Result;
        var schemaRegistryEndpoint = distributedApp.GetEndpoint("redpanda", "schema-registry").ToString();
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(bookingDbConnectionString);

        var infrastructureSharedGrpcEndpoint = distributedApp.GetEndpoint("bookingfakedependencies", "Grpc").ToString();
        var bookingApiGrpcEndpoint = distributedApp.GetEndpoint("bookingapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(infrastructureSharedGrpcEndpoint);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingApiGrpcEndpoint);

        var bookingApiClient = distributedApp.CreateHttpClient("bookingapi");
        ArgumentNullException.ThrowIfNull(bookingApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        configuration["Stripe:SecretKey"] = configuration["Stripe:SecretKey"] ?? "sk_test_integration";
        configuration["Kafka:SchemaRegistry:Url"] = schemaRegistryEndpoint;
        services.AddSingleton(
            configuration.GetSection(ApplicationConfiguration.Key).Get<ApplicationConfiguration>() ??
            new ApplicationConfiguration());
        services.AddHttpContextAccessor();
        services.AddSingleton<IContext, Context>();
        services.AddSingleton<IActivityAccessor, ActivityAccessor>();
        services.AddSingleton<IActivityGetter, ActivityGetter>();
        services.AddSingleton(typeof(IActivityPropagator<>), typeof(ActivityPropagator<>));
        services.AddSingleton<IPropagationContextGetter, PropagationContextGetter>();
        services.AddSingleton<TextMapPropagator>(_ => Propagators.DefaultTextMapPropagator);
        services.AddSingleton<IPropagatorFunctionProvider<IDictionary<string, string>>, StringDictionaryPropagatorFunctions>();
        services.AddSingleton<IRandomHelper, RandomHelper>();
        services.AddSingleton<ITemporalClient>(_ =>
            TemporalClient.ConnectAsync(new TemporalClientConnectOptions
            {
                TargetHost = temporalConnectionString,
                Namespace = "default",
#pragma warning disable VSTHRD002
            }).Result);
#pragma warning restore VSTHRD002
        services.AddSingleton<ITemporalHelperService, TemporalHelperService>();
        services.AddSingleton(new TemporalConfiguration
        {
            Connection = new ConnectionConfig
            {
                Namespace = "default",
                Target = temporalConnectionString ?? string.Empty,
            },
        });
        services.AddTemporalOutboxService();
        services.AddHybridCache();
        services.AddSingleton<IGraphQlTopicEventSender, NoOpGraphQlTopicEventSender>();
        services.AddSingleton<IStringEncryptionAlgorithm, StringEncryptionAlgorithm>();
        var bookingApiGrpcChannel = GrpcChannelFactory.Create(bookingApiGrpcEndpoint);
        var infrastructureSharedGrpcChannel = GrpcChannelFactory.Create(infrastructureSharedGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("booking-api-grpc-channel", bookingApiGrpcChannel)
            .AddKeyedSingleton("infrastructure-shared-grpc-channel", infrastructureSharedGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new BookingService.BookingServiceClient(bookingApiGrpcChannel))
            .AddSingleton(_ =>
                new InfrastructureTestService.InfrastructureTestServiceClient(infrastructureSharedGrpcChannel));

        var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>() ??
                                   new BookingConfiguration
                                   {
                                       ApiKey = "XXX",
                                   };
        services.AddSingleton(bookingConfiguration);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<BookingDbContext>(
                configuration,
                environment,
                bookingDbConnectionString,
                true,
                "bookingdb")
            .AddSharedCrossDomainClients(configuration)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddStripe(configuration)
            .AddXeroServices(configuration)
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<IBookingCoreClient>(_ => new BookingCoreClient(bookingApiClient))
            .AddSingleton<IBookingGraphqlClient>(_ => new BookingGraphqlClient(bookingApiClient))
            .AddSingleton<IBookingStripeWebhookClient>(_ => new BookingStripeWebhookClient(bookingApiClient))
            .AddSingleton<IBookingWorkaroundClient>(_ => new BookingWorkaroundClient(bookingApiClient))
            .AddSingleton<IBookingXeroWebhookClient>(_ => new BookingXeroWebhookClient(bookingApiClient));

        services.AddTransient<TestBearerTokenHandler>();

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(
                httpClient => httpClient.BaseAddress = bookingApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri(),
                clientBuilder => clientBuilder.AddHttpMessageHandler<TestBearerTokenHandler>())
            .ConfigureWebSocketClient(webSocketClient =>
            {
                var wsUri = new UriBuilder(bookingApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri())
                {
                    Scheme = "ws",
                }.Uri;
                webSocketClient.Uri = wsUri;
            });
    }

    private sealed class NoOpGraphQlTopicEventSender : IGraphQlTopicEventSender
    {
        public Task RaiseGraphqlChangeAsync(string topicName, string id, CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
