using Api.Shared.Services;
using Customer.Infrastructure.Services;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;

namespace Customer.Infrastructure;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        _ = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb", true)
            .AddRepositoryFactory()
            .AddRootLevelSharedServices()
            .AddServices()
            .AddJobs()
            .AddTemporalClient(configuration, "temporal");

        return builder.Build().UseWebApplicationDefaults<Program>();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
