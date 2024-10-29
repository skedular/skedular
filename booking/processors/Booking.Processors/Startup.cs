using Api.Shared.Clients.Events.UnityHub.Team.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Team.V1.Value;
using Booking.Processors.Subscribers;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;

namespace Booking.Processors;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "BookingPostgresConnection")
            .WithPooledDbContextFactory<BookingDbContext>(Migration.SetAssembly, Environment)
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka()
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
            .AddMappers()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration
        );
}
