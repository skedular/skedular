using Api.Shared.Clients.Events.UnityHub.Organization.V1.Key;
using Api.Shared.Clients.Events.UnityHub.Organization.V1.Value;
using Billing.Processors.Subscribers;
using Billing.Shared;
using Billing.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox;

namespace Billing.Processors;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "BillingPostgresConnection")
            .WithPooledDbContextFactory<BillingDbContext>(Migration.SetAssembly, Environment)
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services
            .AddKafka()
            .AddKafkaReliableEventConsumersV2<
                BillingInternalSubscriber,
                Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.BillingInternal.V1.Value.Event>(kafkaConfiguration)
            .AddKafkaReliableEventConsumersV2<
                CustomerSubscriber,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Key.Key,
                Api.Shared.Clients.Events.UnityHub.Customer.V1.Value.Event>(kafkaConfiguration)
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
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration
        );
}
