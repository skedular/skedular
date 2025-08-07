using Api.Shared.Services;
using Customer.Shared;
using Customer.Shared.Activities;
using Customer.Shared.Database;
using Customer.Shared.Workflows.AddPayment;
using Customer.Shared.Workflows.Feedback;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
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
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb")
            .AddKafkaOutboxBackgroundService<CustomerDbContext>()
            .AddTemporalOutboxBackgroundService<CustomerDbContext>()
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
            .AddStripe(configuration);

        services
            .AddTemporalWorker(configuration)
            .AddWorkflow<SubmitCustomerFeedback>()
            .AddWorkflow<AddCustomerStripePaymentMethod>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<StripeIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
