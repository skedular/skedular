using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Customer.Processors.Subscribers;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Temporal;

namespace Customer.Processors;

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

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka(configuration)
            .WithPooledDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb")
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.Skedular.Location.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Organization.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                TeamSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddMappers()
            .AddGrpcServices(configuration)
            .AddTemporalClient(configuration);

        return builder.Build().UseWebApplicationDefaults<Program>();
    }
}
