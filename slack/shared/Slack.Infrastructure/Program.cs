using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Slack.Infrastructure.Services;
using Slack.Shared;
using Slack.Shared.Database;

namespace Slack.Infrastructure;

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
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "slackdb", true)
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
