using Api.Shared.Services;
using Core.Shared;
using Core.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.FileStorage;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;

namespace Core.Jobs;

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
            .AddKafkaOutboxBackgroundService<CoreDbContext>()
            .AddTemporalOutboxBackgroundService<CoreDbContext>()
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
