using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Team.Api.GraphQL;
using Team.Api.Grpc;
using Team.Shared;
using Team.Shared.Configurations;
using Team.Shared.Database;

namespace Team.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        var emailConfiguration = Configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddDatabase(Configuration, true, "TeamPostgresConnection")
            .WithPooledDbContextFactory<TeamDbContext>(Migration.SetAssembly, Environment)
            .AddOutboxBackgroundService()
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();

        services
            .AddGraphql<TeamDbContext>(Configuration, builder =>
            {
                builder
                    .AddQueryType<TeamQuery>()
                    .AddMutationType<TeamMutation>();
            });

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddServices()
            .AddMappers()
            .AddUnityHubGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<TeamGrpcService>();
            }
        );
}
