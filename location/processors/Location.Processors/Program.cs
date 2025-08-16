using Api.Shared.Clients.Events.Skedular.Organization.V1.Key;
using Api.Shared.Clients.Events.Skedular.Organization.V1.Value;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Temporal;
using Location.Processors.Subscribers;
using Location.Shared;
using Location.Shared.Database;

namespace Location.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

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
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "locationdb")
            .AddKafkaReliableEventConsumers<
                BookingSubscriber,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddJobs()
            .AddTemporalClient(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
