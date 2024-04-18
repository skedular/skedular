using Billing.Shared.Database;
using Enterprise.Shared.Database;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Microsoft.Extensions.Hosting;

namespace Billing.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<BillingDbContext>(() => CreateHostBuilder(args), default);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
            {
                host.Configuration =
                    builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args);
            })
            .ConfigureServices((host, services) =>
            {
                services
                    .AddDatabase(host.Configuration, false, "BillingPostgresConnection")
                    .WithDbContextFactory<BillingDbContext>(Migration.SetAssembly, host.HostingEnvironment);
            });
}
