using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Notification.Processors.Subscribers;
using Notification.Shared;
using Notification.Shared.Database;

namespace Notification.Processors;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "NotificationPostgresConnection")
            .WithPooledDbContextFactory<NotificationDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka()
            .AddKafkaReliableEventConsumersV2<
                NotificationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Notification.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                CustomerSubscriber,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                LocationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                OrganizationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Organization.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                TeamSubscriber,
                Key,
                Event>(kafkaConfiguration);

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddServices()
            .AddMappers()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(Environment, Configuration);
}
