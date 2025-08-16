using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using MsTeams.Shared;
using MsTeams.Shared.Activities;
using MsTeams.Shared.Database;
using MsTeams.Shared.Workflows.ReSyncMsTeams;
using Temporalio.Extensions.Hosting;

namespace MsTeams.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<MsTeamsDbContext>(configuration, environment, "msteamsdb")
            .AddKafkaOutboxBackgroundService<MsTeamsDbContext>()
            .AddTemporalOutboxBackgroundService<MsTeamsDbContext>()
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices();

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<ReSyncMsTeams>()
            .AddScopedActivities<MsTeamsIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
