using Api.Shared.Clients.OpenApi.Skedular.Booking.V1;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.InfrastructureTest.V1;
using Aspire.Hosting.Testing;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres;
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
using Constants = Enterprise.Shared.HealthCheck.Constants;

namespace Booking.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetSharedInfrastructureGrpc(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Booking_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var bookingDbConnectionString = distributedApp.GetConnectionStringAsync("bookingdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(bookingDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var bookingFakeDependenciesHttpClient = distributedApp.CreateHttpClient("bookingfakedependencies");
        ArgumentNullException.ThrowIfNull(bookingFakeDependenciesHttpClient.BaseAddress);

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        bookingFakeDependenciesHttpClient
            .WaitForSuccessfulGetAsync(Constants.LivenessPath, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

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

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<BookingDbContext>(
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
            .AddSingleton<IBookingClient>(_ => new BookingClient(bookingApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = bookingApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
