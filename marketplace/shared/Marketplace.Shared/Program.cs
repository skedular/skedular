using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Marketplace.Shared.Database;
using Microsoft.Extensions.Hosting;

namespace Marketplace.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<MarketplaceDbContext>(() => CreateHostBuilder(args), CancellationToken.None);

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        // ReSharper disable once MemberCanBePrivate.Global
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
            {
                host.Configuration = builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args);
            })
            .ConfigureServices((host, services) =>
            {
                services
                    .AddDatabase(host.Configuration, false, "MarketplacePostgresConnection")
                    .WithDbContextFactory<MarketplaceDbContext>(host.Configuration, Migration.SetAssembly, host.HostingEnvironment);
            });
}
