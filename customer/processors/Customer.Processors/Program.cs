using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services;
using Customer.Processors.Subscribers;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;
using CustomerReadinessKey = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Key;
using CustomerReadinessEvent = Api.Shared.Clients.Events.Skedular.CustomerReadiness.V1.Event;

namespace Customer.Processors;

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
            .WithPooledPostgreSqlDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb", true)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.Skedular.Location.V1.Key,
                Api.Shared.Clients.Events.Skedular.Location.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Key,
                Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerReadinessEventSubscriber,
                CustomerReadinessKey,
                CustomerReadinessEvent>(kafkaConfiguration)
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
