using Billing.Shared.Database;
using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.Extensions.Hosting;

namespace Billing.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<BillingDbContext>(() => CreateHostBuilder(args), CancellationToken.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
                host.Configuration = builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args))
            .ConfigureServices((host, services) =>
                services.WithPooledDbContextFactory<BillingDbContext>(host.Configuration, host.HostingEnvironment, "BillingPostgresConnection"));
}
