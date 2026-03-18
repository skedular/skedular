using Api.Shared.Services;
using Booking.Shared;
using Booking.Shared.Activities;
using Booking.Shared.Database;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Temporalio.Extensions.Hosting;

namespace Booking.Jobs;

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
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb", true)
            .AddKafkaOutboxBackgroundService<BookingDbContext>()
            .AddTemporalOutboxBackgroundService<BookingDbContext>()
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddSharedCrossDomainClients(configuration)
            .AddInDomainClients(configuration)
            .AddStripe(configuration)
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion, "temporal")
            .AddWorkflow<GenerateLocationResourcesSlots>()
            .AddWorkflow<GenerateResourcesSlots>()
            .AddWorkflow<PayBookingViaCard>()
            .AddWorkflow<PayBookingViaBankTransfer>()
            .AddWorkflow<BookPrivateRecurringResources>()
            .AddWorkflow<BookMarketplaceBookingSubscriptionResources>()
            .AddWorkflow<PayRecurringBookingViaCard>()
            .AddScopedActivities<LocationResourceSlot>()
            .AddScopedActivities<StripeIntegrations>()
            .AddScopedActivities<BookingIntegrations>()
            .AddScopedActivities<InvoiceIntegrations>()
            .AddScopedActivities<PrivateRecurringBookingIntegrations>()
            .AddScopedActivities<MarketplaceBookingSubscriptionIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
