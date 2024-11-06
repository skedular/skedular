using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Kafka.Configurations;
using Enterprise.Shared.Outbox;
using Payment.Shared;
using Payment.Shared.Database;

namespace Payment.Jobs;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "PaymentPostgresConnection")
            .WithPooledDbContextFactory<PaymentDbContext>(Configuration, Migration.SetAssembly, Environment)
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
            .AddJobs();
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(Environment, Configuration);
}
