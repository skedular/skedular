using Customer.Shared;
using Customer.Shared.Configurations;
using Customer.Shared.Database;
using Customer.Shared.Workflows;
using Customer.Shared.Workflows.Activities;
using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Temporalio.Extensions.Hosting;

namespace Customer.Jobs;

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

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .WithPooledDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb")
            .AddKafkaOutboxBackgroundService<CustomerDbContext>()
            .AddTemporalOutboxBackgroundService<CustomerDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddStripe(configuration)
            .AddGrpcServices(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<AddCustomerStripePaymentMethod>()
            .AddScopedActivities<StripeIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
