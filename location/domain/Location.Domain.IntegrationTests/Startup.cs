using Api.Shared.Clients.OpenApi.Skedular.Location.Analytics.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.Location.Workaround.V1;
using Api.Shared.Grpc.Skedular.Location.Core.V1;
using Api.Shared.Grpc.Skedular.Location.Resources.V1;
using Api.Shared.Services;
using Api.Shared.Services.Configurations.Grpc;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Random;
using Flurl;
using Location.Shared;
using Location.Shared.Database;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace Location.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Location_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var locationDbConnectionString = distributedApp.GetConnectionStringAsync("locationdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(locationDbConnectionString);
        var locationApiGrpcEndpoint = distributedApp.GetEndpoint("locationapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(locationApiGrpcEndpoint);

        var locationApiClient = distributedApp.CreateHttpClient("locationapi");
        ArgumentNullException.ThrowIfNull(locationApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var locationApiGrpcChannel = GrpcChannelFactory.Create(locationApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services.TryAddSingleton<IRandomHelper, RandomHelper>();
        services
            .AddKeyedSingleton("location-api-grpc-channel", locationApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new LocationService.LocationServiceClient(locationApiGrpcChannel))
            .AddSingleton(_ => new LocationResourcesService.LocationResourcesServiceClient(locationApiGrpcChannel));

        var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>() ??
                                    new LocationConfiguration { ApiKey = "XXX" };
        services.AddSingleton(locationConfiguration);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<LocationDbContext>(
                configuration,
                environment,
                locationDbConnectionString,
                true,
                "locationdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ILocationCoreClient>(_ => new LocationCoreClient(locationApiClient))
            .AddSingleton<ILocationGraphqlClient>(_ => new LocationGraphqlClient(locationApiClient))
            .AddSingleton<ILocationWorkaroundClient>(_ => new LocationWorkaroundClient(locationApiClient))
            .AddSingleton<ILocationAnalyticsClient>(_ => new LocationAnalyticsClient(locationApiClient));

        services.AddTransient<TestBearerTokenHandler>();

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(
                httpClient => httpClient.BaseAddress = locationApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri(),
                builder => builder.AddHttpMessageHandler<TestBearerTokenHandler>());
    }
}
