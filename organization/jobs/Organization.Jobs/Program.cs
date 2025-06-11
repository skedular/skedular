using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Organization.Shared;
using Organization.Shared.Configurations;
using Organization.Shared.Database;
using Organization.Shared.Workflows;
using Organization.Shared.Workflows.Activities;
using Temporalio.Extensions.Hosting;

namespace Organization.Jobs;

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

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .AddCdn(configuration)
            .WithPooledDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb")
            .AddKafkaOutboxBackgroundService<OrganizationDbContext>()
            .AddTemporalOutboxBackgroundService<OrganizationDbContext>()
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
            .AddWorkflow<ScheduleRenewOrganizationOffering>()
            .AddWorkflow<AddOrganizationStripePaymentMethod>()
            .AddScopedActivities<OrganizationOfferings>()
            .AddScopedActivities<StripeIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
