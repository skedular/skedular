using Booking.Shared.Database;
using Enterprise.Shared.Database;
using Enterprise.Shared.Infrastructure.Configuration.Extensions;
using Microsoft.Extensions.Hosting;

namespace Booking.Shared;

public class Program
{
    public static async Task Main(string[] args) =>
        await MigrationHelper.RunMigrationAsync<BookingDbContext>(() => CreateHostBuilder(args), default);

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
                    .AddDatabase(host.Configuration, false, "BookingPostgresConnection")
                    .WithDbContextFactory<BookingDbContext>(Migration.SetAssembly, host.HostingEnvironment);
            });
}
