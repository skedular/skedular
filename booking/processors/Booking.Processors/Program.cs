using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key;
using Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value;
using Booking.Processors.Subscribers;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;

namespace Booking.Processors;

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
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb")
            .AddKafkaReliableEventConsumers<
                BookingInternalSubscriber,
                Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.BookingInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.Skedular.Location.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                TeamSubscriber,
                Api.Shared.Clients.Events.Skedular.Team.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Team.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                MarketplaceSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedConfigurations(configuration)
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddMappers()
            .AddGrpcClients(configuration)
            .AddTemporalClient(configuration)
            .AddStripe(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
