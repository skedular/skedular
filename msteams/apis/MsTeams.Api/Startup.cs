using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using MsTeams.Api.Grpc;
using MsTeams.Shared;
using MsTeams.Shared.Database;

namespace MsTeams.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "MsTeamsPostgresConnection")
            .WithPooledDbContextFactory<MsTeamsDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();
        services.AddRedis(Configuration);

        services.AddGraphql<MsTeamsDbContext>(Configuration, builder => { builder.AddApiTypes(); });

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddServices()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddSkedularGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<MsTeamsGrpcService>();
            }
        );
}
