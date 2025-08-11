using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Slack.Shared;
using Slack.Shared.Activities;
using Slack.Shared.Configurations;
using Slack.Shared.Database;
using Slack.Shared.Workflows.NewSlackWorkspaceJoined;
using Slack.Shared.Workflows.ReSyncSlackWorkspace;
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

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "slackdb")
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
            .AddGrpcClients(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<NewSlackWorkspaceJoined>()
            .AddWorkflow<ReSyncSlackWorkspace>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<SlackIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
