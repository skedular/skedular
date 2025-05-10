using Enterprise.Shared.Configurations.Extensions;
using Enterprise.Shared.Database;
using Location.Shared.Database;
using Microsoft.Extensions.Hosting;

namespace Location.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<LocationDbContext>(() => CreateHostBuilder(args), CancellationToken.None);

    // ReSharper disable once MemberCanBePrivate.Global
    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((host, builder) =>
                host.Configuration = builder.BuildConfig<Program>(host.HostingEnvironment.EnvironmentName, args))
            .ConfigureServices((host, services) =>
                services.WithPooledDbContextFactory<LocationDbContext>(host.Configuration, host.HostingEnvironment, "LocationPostgresConnection"));
}
