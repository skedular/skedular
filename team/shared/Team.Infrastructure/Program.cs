using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Team.Infrastructure.Services;
using Team.Shared;
using Team.Shared.Database;

namespace Team.Infrastructure;

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
            .WithPooledDbContextFactory<TeamDbContext>(configuration, environment, "teamdb")
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
