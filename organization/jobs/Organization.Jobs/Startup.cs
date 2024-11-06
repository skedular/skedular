using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox;
using Organization.Shared;
using Organization.Shared.Configurations;
using Organization.Shared.Database;

namespace Organization.Jobs;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        var emailConfiguration = Configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddDatabase(Configuration, true, "OrganizationPostgresConnection")
            .WithPooledDbContextFactory<OrganizationDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddOutboxBackgroundService()
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        var kafkaConfiguration = Configuration.GetSection(KafkaConfiguration.Key).Get<KafkaConfiguration>();
        ArgumentNullException.ThrowIfNull(kafkaConfiguration);

        services.AddKafka();

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(Environment, Configuration);
}
