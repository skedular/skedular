using System.Reflection;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox.Kafka;
using Enterprise.Shared.Outbox.Temporal;
using Enterprise.Shared.Temporal;
using Location.Shared;
using Location.Shared.Activities;
using Location.Shared.Database;
using Location.Shared.Workflows;
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
        _ = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .WithPooledPostgreSqlDbContextFactory<LocationDbContext>(configuration, environment, "locationdb", true)
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
            .AddServices()
            .AddSharedCrossDomainClients(configuration)
            .AddTemporalWorker(
                configuration,
                typeof(Program).Assembly.GetName().Name!,
                (Attribute.GetCustomAttribute(typeof(Program).Assembly, typeof(AssemblyInformationalVersionAttribute)) as
                    AssemblyInformationalVersionAttribute)?.InformationalVersion ?? "0.0.0",
                "temporal")
            .AddWorkflow<GenerateLocationDailyAnalytics>()
            .AddWorkflow<RecomputeLocationBookingDerivedState>()
            .AddWorkflow<ComputeOrganizationLocationsAndProductsRelationships>()
            .AddWorkflow<NewLocationJoined>()
            .AddScopedActivities<LocationDailyAnalytics>()
            .AddScopedActivities<LocationBookingDerivedState>()
            .AddScopedActivities<LocationsProductsRelationships>()
            .AddScopedActivities<EmailIntegrations>();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
