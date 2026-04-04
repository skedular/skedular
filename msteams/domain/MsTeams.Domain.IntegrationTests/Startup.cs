using Api.Shared.Clients.OpenApi.Skedular.MsTeams.V1;
using Api.Shared.Services;
using Api.Shared.Services.Grpc.Skedular.MsTeams.V1;
using Aspire.Hosting.Testing;
using Enterprise.Shared;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database.Postgres;
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
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        DomainAppHostEnvironmentVariables.SetFakeDependencies(true);
        var builder = DistributedApplicationTestingBuilder.CreateAsync<MsTeams_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var msteamsDbConnectionString = distributedApp.GetConnectionStringAsync("msteamsdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(msteamsDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

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
            .WithPooledDbContextFactoryWithConnectionString<MsTeamsDbContext>(
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
            .AddSingleton<IMsTeamsClient>(_ => new MsTeamsClient(msteamsApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = msteamsApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
