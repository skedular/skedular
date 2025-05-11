using Booking.Infrastructure.Services;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;

namespace Booking.Infrastructure;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddServiceDefaults<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb")
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddPublishers();

        return builder.Build().AddWebApplicationDefaults();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
