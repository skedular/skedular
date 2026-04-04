using Api.Shared.Clients.OpenApi.Skedular.Marketplace.V1;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.Marketplace.V1;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Flurl;
using Marketplace.Shared;
using Marketplace.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;
using Constants = Enterprise.Shared.HealthCheck.Constants;

namespace Marketplace.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Marketplace_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var marketplaceDbConnectionString = distributedApp.GetConnectionStringAsync("marketplacedb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(marketplaceDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var marketplaceFakeDependenciesHttpClient = distributedApp.CreateHttpClient("marketplacefakedependencies");
        ArgumentNullException.ThrowIfNull(marketplaceFakeDependenciesHttpClient.BaseAddress);

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        marketplaceFakeDependenciesHttpClient
            .WaitForSuccessfulGetAsync(Constants.LivenessPath, CancellationToken.None)
            .GetAwaiter()
            .GetResult();
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        var marketplaceApiGrpcEndpoint = distributedApp.GetEndpoint("marketplaceapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(marketplaceApiGrpcEndpoint);

        var marketplaceApiClient = distributedApp.CreateHttpClient("marketplaceapi");
        ArgumentNullException.ThrowIfNull(marketplaceApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var marketplaceApiGrpcChannel = GrpcChannelFactory.Create(marketplaceApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("marketplace-api-grpc-channel", marketplaceApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new MarketplaceService.MarketplaceServiceClient(marketplaceApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<MarketplaceDbContext>(
                configuration,
                environment,
                marketplaceDbConnectionString,
                true,
                "marketplacedb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<IMarketplaceClient>(_ => new MarketplaceClient(marketplaceApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = marketplaceApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
