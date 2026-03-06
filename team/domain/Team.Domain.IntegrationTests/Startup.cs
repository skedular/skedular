using Api.Shared.Clients.OpenApi.Skedular.Team.V1;
using Api.Shared.Services;
using Aspire.Hosting.Testing;
using Enterprise.Shared.Configurations;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Flurl;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.Internal;
using Projects;
using Team.Shared;
using Team.Shared.Database;
using Testing.Shared.IntegrationTests.Aspire;

namespace Team.Domain.IntegrationTests;

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        var environment = new HostingEnvironment { EnvironmentName = Environments.Production };

#pragma warning disable VSTHRD104
#pragma warning disable VSTHRD002
#pragma warning disable CA2012
        var builder = DistributedApplicationTestingBuilder.CreateAsync<Team_Domain_AppHost>().Result;
        var distributedApp = builder.AddDefaultServices().StartAsync(CancellationToken.None).Result;

        var kafkaConnectionString = distributedApp.GetConnectionStringAsync("kafka").Result;
        var teamDbConnectionString = distributedApp.GetConnectionStringAsync("teamdb").Result;
#pragma warning restore CA2012
#pragma warning restore VSTHRD002
#pragma warning restore VSTHRD104

        ArgumentException.ThrowIfNullOrWhiteSpace(teamDbConnectionString);

        var pgadmin = distributedApp.GetEndpoint("pgadmin");
        var kafkaUi = distributedApp.GetEndpoint("kafka-ui");

        Console.WriteLine($"pgadmin: {pgadmin}");
        Console.WriteLine($"kafkaUi: {kafkaUi}");

        var teamApiClient = distributedApp.CreateHttpClient("teamapi");

        var configuration = new ConfigurationBuilder().BuildConfig<Startup>(environment.EnvironmentName);

        services.TryAddSingleton(TimeProvider.System);

        services.AddKafkaWithConnectionString(configuration, kafkaConnectionString);

        services
            .WithPooledDbContextFactoryWithConnectionString<TeamDbContext>(
                configuration,
                environment,
                teamDbConnectionString,
                true,
                "teamdb")
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddSingleton<ITeamClient>(_ => new TeamClient(teamApiClient));

        services
            .AddSkedularGraphQLV1()
            .ConfigureHttpClient(httpClient => httpClient.BaseAddress = teamApiClient.BaseAddress.AppendPathSegment("/v1/graphql").ToUri());
    }
}
