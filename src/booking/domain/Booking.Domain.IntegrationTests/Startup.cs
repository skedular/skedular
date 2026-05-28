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
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace Booking.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Booking_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var bookingDbConnectionString = distributedApp.GetConnectionStringAsync("bookingdb").Result;
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
                                   new BookingConfiguration { ApiKey = "XXX" };
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
                builder => builder.AddHttpMessageHandler<TestBearerTokenHandler>())
            .ConfigureWebSocketClient(webSocketClient =>
            {
                var wsUri = new UriBuilder(bookingApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri()) { Scheme = "ws" }.Uri;
                webSocketClient.Uri = wsUri;
            });
    }
}
