using Booking.Shared;
using Booking.Shared.Database;
using Booking.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Temporalio.Extensions.Hosting;

namespace Booking.Jobs;

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
            .WithPooledDbContextFactory<BookingDbContext>(configuration, environment, "bookingdb")
            .AddKafkaOutboxBackgroundService<BookingDbContext>()
            .AddTemporalOutboxBackgroundService<BookingDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<BookingPaidThroughStripe>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
