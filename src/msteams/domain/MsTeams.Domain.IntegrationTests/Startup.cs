using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Core.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Graphql.V1;
using Api.Shared.Clients.OpenApi.Skedular.MsTeams.Workaround.V1;
using Api.Shared.Grpc.Skedular.MsTeams.Core.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
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
using MsTeams.Shared;
using MsTeams.Shared.Database;
using Projects;
using Testing.Shared.IntegrationTests;
using Testing.Shared.IntegrationTests.Aspire;

namespace MsTeams.Domain.IntegrationTests;

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
        var builder = DistributedApplicationTestingBuilder.CreateAsync<MsTeams_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(TestContext.Current.CancellationToken).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var msteamsDbConnectionString = distributedApp.GetConnectionStringAsync("msteamsdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(msteamsDbConnectionString);
        var msTeamsApiGrpcEndpoint = distributedApp.GetEndpoint("msteamsapi", "Grpc").ToString();
        ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsApiGrpcEndpoint);

        var msteamsApiClient = distributedApp.CreateHttpClient("msteamsapi");
        ArgumentNullException.ThrowIfNull(msteamsApiClient.BaseAddress);

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);
        var msTeamsApiGrpcChannel = GrpcChannelFactory.Create(msTeamsApiGrpcEndpoint);

        services.TryAddSingleton(TimeProvider.System);
        services
            .AddKeyedSingleton("msteams-api-grpc-channel", msTeamsApiGrpcChannel)
            .AddTestingSharedIntegrationTests()
            .AddSingleton(_ => new MsTeamsService.MsTeamsServiceClient(msTeamsApiGrpcChannel));

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledPostgreSqlDbContextFactoryWithConnectionString<MsTeamsDbContext>(
                configuration,
                environment,
                msteamsDbConnectionString,
                true,
                "msteamsdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<IMsTeamsCoreClient>(_ => new MsTeamsCoreClient(msteamsApiClient))
            .AddSingleton<IMsTeamsGraphqlClient>(_ => new MsTeamsGraphqlClient(msteamsApiClient))
            .AddSingleton<IMsTeamsWorkaroundClient>(_ => new MsTeamsWorkaroundClient(msteamsApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = msteamsApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
