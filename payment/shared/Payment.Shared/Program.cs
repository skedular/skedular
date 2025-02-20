using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Microsoft.Extensions.Hosting;
using Payment.Shared.Database;

namespace Payment.Shared;

public class Program
{
    public static async Task Main(string[] args) => await MigrationHelper.RunMigrationAsync<PaymentDbContext>(() => CreateHostBuilder(args),
        CancellationToken.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
            {
                host.Configuration = builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args);
            })
            .ConfigureServices((host, services) =>
            {
                services
                    .AddDatabase(host.Configuration, false, "PaymentPostgresConnection")
                    .WithDbContextFactory<PaymentDbContext>(
                        host.Configuration,
                        Migration.SetAssembly,
                        host.HostingEnvironment);
            });
}
