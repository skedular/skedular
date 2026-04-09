using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using Team.Shared;
using Team.Shared.Activities;
using Team.Shared.Database;
using Team.Shared.Workflows;
using Temporalio.Extensions.Hosting;

namespace Team.Jobs;

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
            .WithPooledDbContextFactory<TeamDbContext>(configuration, environment, "teamdb", true)
            .AddKafkaOutboxBackgroundService<TeamDbContext>()
            .AddTemporalOutboxBackgroundService<TeamDbContext>()
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
            .AddSharedCrossDomainClients(configuration)
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion, "temporal")
            .AddWorkflow<InviteToJoinTeam>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<InvitationIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
