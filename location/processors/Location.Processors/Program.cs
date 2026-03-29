using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;
using Location.Processors.Subscribers;
using Location.Shared;
using Location.Shared.Database;
using BookingEvent = Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event;
using BookingKey = Api.Shared.Clients.Events.Skedular.Booking.V1.Key.Key;
using OrganizationEvent = Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event;
using OrganizationKey = Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key;

namespace Location.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        var kafkaConfiguration = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "locationdb", true)
            .AddKafkaReliableEventConsumers<
                BookingSubscriber,
                BookingKey,
                BookingEvent>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                MarketplaceSubscriber,
                Key,
                Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                OrganizationKey,
                OrganizationEvent>(kafkaConfiguration)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddJobs()
            .AddSharedCrossDomainClients(configuration)
            .AddTemporalClient(configuration, "temporal");

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
