using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Temporal;
using Organization.Shared;
using Organization.Shared.Activities;
using Organization.Shared.Configurations;
using Organization.Shared.Database;
using Organization.Shared.Workflows.AddPayment;
using Organization.Shared.Workflows.GenerateOrganizationDailyAnalytics;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationExistingCustomer;
using Organization.Shared.Workflows.Invitation.InviteToJoinOrganizationNewCustomer;
using Organization.Shared.Workflows.NewOrganizationJoined;
using Organization.Shared.Workflows.OrganizationOfferingRenewal;
using Organization.Shared.Workflows.ReSyncAzureTenant;
using Temporalio.Extensions.Hosting;

namespace Organization.Jobs;

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

        services
            .AddKafka(configuration)
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb", true)
            .AddKafkaOutboxBackgroundService<OrganizationDbContext>()
            .AddTemporalOutboxBackgroundService<OrganizationDbContext>()
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
            .AddGrpcClients(configuration);

        services
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion)
            .AddWorkflow<ScheduleRenewOrganizationOffering>()
            .AddWorkflow<AddOrganizationStripePaymentMethod>()
            .AddWorkflow<InviteToJoinOrganizationExistingCustomer>()
            .AddWorkflow<InviteToJoinOrganizationNewCustomer>()
            .AddWorkflow<GenerateOrganizationDailyAnalytics>()
            .AddWorkflow<ReSyncAzureTenant>()
            .AddWorkflow<NewOrganizationJoined>()
            .AddScopedActivities<OrganizationOfferings>()
            .AddScopedActivities<StripeIntegrations>()
            .AddScopedActivities<EmailIntegrations>()
            .AddScopedActivities<OrganizationDailyAnalytics>()
            .AddScopedActivities<AzureTenantIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
