using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.Extensions.Hosting;
using Slack.Shared.Database;

namespace Slack.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<SlackDbContext>(() => CreateHostBuilder(args), CancellationToken.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
                host.Configuration = builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args))
            .ConfigureServices((host, services) =>
                services.WithPooledDbContextFactory<SlackDbContext>(host.Configuration, host.HostingEnvironment, "SlackPostgresConnection"));
}
