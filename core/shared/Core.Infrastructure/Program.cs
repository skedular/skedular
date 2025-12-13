using Api.Shared.Services;
using Core.Infrastructure.Services;
using Core.Shared;
using Core.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;

namespace Core.Infrastructure;

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
            .AddFileStorage(configuration, CoreApiHelper.GetPublicCdnFileEndpoint(), CoreApiHelper.GetPrivateFileEndpoint())
            .WithPooledDbContextFactory<CoreDbContext>(configuration, environment, "coredb", true)
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
