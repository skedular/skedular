using Api.Shared.Clients.OpenApi.Skedular.Core.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.Core.Graphql.V1;
using Api.Shared.Grpc.Skedular.Core.Core.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Core.Shared;
using Core.Shared.Database;
using Enterprise.Shared;
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

namespace Core.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Core_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var coreDbConnectionString = distributedApp.GetConnectionStringAsync("coredb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(coreDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var coreApiGrpcEndpoint = distributedApp.GetEndpoint("coreapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(coreApiGrpcEndpoint);

        var coreApiClient = distributedApp.CreateHttpClient("coreapi");
        ArgumentNullException.ThrowIfNull(coreApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var coreApiGrpcChannel = GrpcChannelFactory.Create(coreApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("core-api-grpc-channel", coreApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new CoreService.CoreServiceClient(coreApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<CoreDbContext>(
                configuration,
                environment,
                coreDbConnectionString,
                true,
                "coredb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ICoreCoreClient>(_ => new CoreCoreClient(coreApiClient))
            .AddSingleton<ICoreGraphqlClient>(_ => new CoreGraphqlClient(coreApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = coreApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
