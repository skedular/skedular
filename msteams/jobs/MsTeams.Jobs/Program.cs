using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using MsTeams.Shared;
using MsTeams.Shared.Activities;
using MsTeams.Shared.Database;
using MsTeams.Shared.Workflows;
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
        _ = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<MsTeamsDbContext>(configuration, environment, "msteamsdb", true)
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
            .AddServices()
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion, "temporal")
            .AddWorkflow<ReSyncMsTeams>()
            .AddScopedActivities<MsTeamsIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
