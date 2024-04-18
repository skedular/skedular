using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Api.Shared.Clients.OpenApi.UnityHub.Notification.V1;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Notification.Processors.Subscribers;
using Notification.Shared;
using Notification.Shared.Configurations;
using Notification.Shared.Database;

namespace Notification.Processors;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "NotificationPostgresConnection")
            .WithPooledDbContextFactory<NotificationDbContext>(Migration.SetAssembly, Environment)
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka()
            .AddKafkaReliableEventConsumers<
                NotificationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Notification.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Notification.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                CustomerSubscriber,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                LocationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                OrganizationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Organization.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Organization.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumers<
                TeamSubscriber,
                Key,
                Event>(kafkaConfiguration);

        var notificationConfiguration =
            Configuration.GetSection(NotificationConfiguration.Key).Get<NotificationConfiguration>();
        ArgumentNullException.ThrowIfNull(notificationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(notificationConfiguration.BaseUri);
        services.AddSingleton(notificationConfiguration);

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddServices()
            .AddMappers();
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration
        );
}
