using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Organization.Processors.Subscribers;
using Organization.Shared;
using Organization.Shared.Configurations;
using Organization.Shared.Database;

namespace Organization.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb", true)
            .AddKafkaReliableEventConsumers<
                OrganizationInternalSubscriber,
                Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.OrganizationInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                BookingSubscriber,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.Skedular.Location.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                TeamSubscriber,
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
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddGrpcClients(configuration)
            .AddTemporalClient(configuration)
            .AddStripe(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
