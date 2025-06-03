using Api.Shared.Clients.Events.Skedular.Customer.V1.Key;
using Api.Shared.Clients.Events.Skedular.Customer.V1.Value;
using Core.Processors.Subscribers;
using Core.Shared;
using Core.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;

namespace Core.Processors;

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

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka(configuration)
            .AddCdn(configuration)
            .WithPooledDbContextFactory<CoreDbContext>(configuration, environment, "coredb")
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
