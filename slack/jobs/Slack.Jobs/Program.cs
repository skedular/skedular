using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Slack.Shared;
using Slack.Shared.Activities;
using Slack.Shared.Database;
using Slack.Shared.Workflows;
using Temporalio.Extensions.Hosting;

namespace Slack.Jobs;

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
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "slackdb", true)
            .AddKafkaOutboxBackgroundService<SlackDbContext>()
            .AddTemporalOutboxBackgroundService<SlackDbContext>()
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
            .AddSlack(configuration, _ => { })
            .AddSharedCrossDomainClients(configuration)
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion, "temporal")
            .AddWorkflow<NewSlackWorkspaceJoined>()
            .AddWorkflow<ReSyncSlackWorkspace>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<SlackIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
