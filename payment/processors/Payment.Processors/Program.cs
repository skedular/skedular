using Api.Shared.Clients.Events.Skedular.Billing.V1.Key;
using Api.Shared.Clients.Events.Skedular.Billing.V1.Value;
using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Payment;
using Payment.Processors.Subscribers;
using Payment.Shared;
using Payment.Shared.Database;

namespace Payment.Processors;

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
            .WithPooledDbContextFactory<PaymentDbContext>(configuration, environment, "paymentdb")
            .AddKafkaReliableEventConsumers<
                BookingSubscriber,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Booking.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                PaymentInternalSubscriber,
                Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.PaymentInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                MarketplaceSubscriber,
                Api.Shared.Clients.Events.Skedular.Marketplace.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Marketplace.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                BillingSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddStripe(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
