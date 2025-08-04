using Api.Shared.Services;
using Booking.Shared;
using Booking.Shared.Activities;
using Booking.Shared.Database;
using Booking.Shared.Workflows.Payment.PayViaBankTransfer;
using Booking.Shared.Workflows.Payment.PayViaCard;
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
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb")
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
            .AddGrpcClients(configuration)
            .AddStripe(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<PayBookingViaCard>()
            .AddWorkflow<PayBookingViaBankTransfer>()
            .AddScopedActivities<StripeIntegrations>()
            .AddScopedActivities<BookingIntegrations>()
            .AddScopedActivities<InvoiceIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
