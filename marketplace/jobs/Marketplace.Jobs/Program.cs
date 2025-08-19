using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox;
using Marketplace.Shared;
using Marketplace.Shared.Database;

namespace Marketplace.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<MarketplaceDbContext>(configuration, environment, "marketplacedb", true)
            .AddKafkaOutboxBackgroundService<MarketplaceDbContext>()
            .AddTemporalOutboxBackgroundService<MarketplaceDbContext>()
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
