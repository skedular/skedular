using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Organization.Api.GraphQL;
using Organization.Api.Grpc;
using Organization.Shared;
using Organization.Shared.Configurations;
using Organization.Shared.Database;

namespace Organization.Api;

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
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();

        services
            .AddGraphql<OrganizationDbContext>(Configuration, builder =>
            {
                builder
                    .AddQueryType<OrganizationQuery>()
                    .AddMutationType<OrganizationMutation>();
            });

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddServices()
            .AddJobs()
            .AddMappers()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<OrganizationGrpcService>();
            }
        );
}
