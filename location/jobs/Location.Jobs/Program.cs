using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Enterprise.Shared.Temporal;
using Location.Shared;
using Location.Shared.Activities;
using Location.Shared.Configurations;
using Location.Shared.Database;
using Location.Shared.Workflows.GenerateLocationDailyAnalytics;
using Location.Shared.Workflows.NewLocationJoined;
using Location.Shared.Workflows.PrecomputeLocationProductRelationships;
using Temporalio.Extensions.Hosting;

namespace Location.Jobs;

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

        _ = services.AddKafka(configuration);

        services
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "locationdb", true)
            .AddKafkaOutboxBackgroundService<LocationDbContext>()
            .AddTemporalOutboxBackgroundService<LocationDbContext>()
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices();

        services
            .AddTemporalWorker(configuration, typeof(Program).Assembly.GetName().Name!, GitVersionInformation.InformationalVersion)
            .AddWorkflow<GenerateLocationDailyAnalytics>()
            .AddWorkflow<ComputeOrganizationLocationsAndProductsRelationships>()
            .AddWorkflow<NewLocationJoined>()
            .AddScopedActivities<LocationDailyAnalytics>()
            .AddScopedActivities<LocationsProductsRelationships>()
            .AddScopedActivities<EmailIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
