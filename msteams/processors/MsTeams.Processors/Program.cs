using Api.Shared.Clients.Events.Skedular.Organization.V1;
using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Temporal;
using MsTeams.Processors.Subscribers;
using MsTeams.Shared;
using MsTeams.Shared.Database;

namespace MsTeams.Processors;

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
            .WithPooledPostgreSqlDbContextFactory<MsTeamsDbContext>(configuration, environment, "msteamsdb", true)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.Skedular.Location.V1.Key,
                Api.Shared.Clients.Events.Skedular.Location.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                TeamSubscriber,
                Api.Shared.Clients.Events.Skedular.Team.V1.Key,
                Api.Shared.Clients.Events.Skedular.Team.V1.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Key,
                Event>(kafkaConfiguration)
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddTemporalClient(configuration, "temporal");

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
