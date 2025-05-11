using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Location.Infrastructure.Services;
using Location.Shared;
using Location.Shared.Database;

namespace Location.Infrastructure;

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
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "locationdb")
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs();

        return builder.Build().AddWebApplicationDefaults();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
