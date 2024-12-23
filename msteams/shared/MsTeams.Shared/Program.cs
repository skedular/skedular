using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.Extensions.Hosting;
using MsTeams.Shared.Database;

namespace MsTeams.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<MsTeamsDbContext>(() => CreateHostBuilder(args), CancellationToken.None);

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        // ReSharper disable once MemberCanBePrivate.Global
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
            {
                host.Configuration =
                    builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args);
            })
            .ConfigureServices((host, services) =>
            {
                services
                    .AddDatabase(host.Configuration, false, "MsTeamsPostgresConnection")
                    .WithDbContextFactory<MsTeamsDbContext>(
                        host.Configuration,
                        Migration.SetAssembly,
                        host.HostingEnvironment);
            });
}
