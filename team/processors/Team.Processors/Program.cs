using Api.Shared.Clients.Events.Skedular.Location.V1;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;
using Team.Processors.Subscribers;
using Team.Shared;
using Team.Shared.Database;
using OrganizationEvent = Api.Shared.Clients.Events.Skedular.Organization.V1.Event;
using OrganizationKey = Api.Shared.Clients.Events.Skedular.Organization.V1.Key;

namespace Team.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        var kafkaConfiguration = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .WithPooledDbContextFactory<TeamDbContext>(configuration, environment, "teamdb", true)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Key,
                Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                OrganizationKey,
                OrganizationEvent>(kafkaConfiguration)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddTemporalClient(configuration, "temporal");

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
