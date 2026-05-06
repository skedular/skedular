using Api.Shared.Clients.OpenApi.Skedular.Marketplace.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Marketplace.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Marketplace.Workaround.V1;
using Api.Shared.Grpc.Skedular.Marketplace.Core.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql;
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
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

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
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<MarketplaceDbContext>(
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
            .AddSingleton<IMarketplaceCoreClient>(_ => new MarketplaceCoreClient(marketplaceApiClient))
            .AddSingleton<IMarketplaceGraphqlClient>(_ => new MarketplaceGraphqlClient(marketplaceApiClient))
            .AddSingleton<IMarketplaceWorkaroundClient>(_ => new MarketplaceWorkaroundClient(marketplaceApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = marketplaceApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
