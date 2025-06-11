using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Database;

namespace Slack.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .AddCdn(configuration)
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "slackdb")
            .AddKafkaOutboxBackgroundService<SlackDbContext>()
            .AddTemporalOutboxBackgroundService<SlackDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddSlack(configuration, _ => { })
            .AddGrpcServices(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
