using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Location.Shared;
using Location.Shared.Database;

namespace Location.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

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
            .AddCdn(configuration)
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "locationdb")
            .AddKafkaOutboxBackgroundService<LocationDbContext>()
            .AddTemporalOutboxBackgroundService<LocationDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
