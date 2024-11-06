using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using MsTeams.Processors.Subscribers;
using MsTeams.Shared;
using MsTeams.Shared.Database;

namespace MsTeams.Processors;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "MsTeamsPostgresConnection")
            .WithDbContextFactory<MsTeamsDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka()
            .AddKafkaReliableEventConsumersV2<
                MsTeamsInternalSubscriber,
                Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.MsTeamsInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                CustomerSubscriber,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                LocationSubscriber,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Location.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                TeamSubscriber,
                Api.Shared.Clients.Events.UnityHub.Team.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Team.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                OrganizationSubscriber,
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
            .AddJobs();
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(Environment, Configuration);
}
