using Api.Shared.Services;
using Customer.Shared;
using Customer.Shared.Activities;
using Customer.Shared.Database;
using Customer.Shared.Workflows;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Temporalio.Extensions.Hosting;

namespace Customer.Jobs;

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
            .WithPooledPostgreSqlDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb", true)
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
            .AddStripe(configuration)
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion, "temporal")
            .AddWorkflow<SubmitCustomerFeedback>()
            .AddWorkflow<AddCustomerStripePaymentMethod>()
            .AddWorkflow<NewCustomerJoined>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<StripeIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
