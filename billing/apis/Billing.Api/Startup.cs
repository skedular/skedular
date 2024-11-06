using Billing.Api.GraphQL;
using Billing.Api.Grpc;
using Billing.Shared;
using Billing.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;

namespace Billing.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "BillingPostgresConnection")
            .WithPooledDbContextFactory<BillingDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();

        services
            .AddGraphql<BillingDbContext>(Configuration, builder =>
            {
                builder
                    .AddQueryType<BillingQuery>()
                    .AddMutationType<BillingMutation>();
            });

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddServices()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<BillingGrpcService>();
            }
        );
}
