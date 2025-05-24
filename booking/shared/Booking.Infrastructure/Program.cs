using Booking.Infrastructure.Services;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;

namespace Booking.Infrastructure;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb")
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddPublishers();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
