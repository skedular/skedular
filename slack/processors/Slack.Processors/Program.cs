using Api.Shared.Clients.Events.Skedular.Team.V1.Key;
using Api.Shared.Clients.Events.Skedular.Team.V1.Value;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Slack.Processors.Subscribers;
using Slack.Shared;
using Slack.Shared.Database;

namespace Slack.Processors;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var kafkaConfiguration = configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "SlackPostgresConnection")
            .AddKafkaReliableEventConsumers<
                SlackInternalSubscriber,
                Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.SlackInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.Skedular.Customer.V1.Value.Event>(kafkaConfiguration)
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
            .AddJobs()
            .AddSlack(configuration, _ => { })
            .AddGrpcServices(configuration);

        return builder.Build().UseApplicationBuilderDefaults();
    }
}
