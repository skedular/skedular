using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using HotChocolate.Language;
using HotChocolate.Types;
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
        services.AddRedis(Configuration);

        services.AddGraphql<OrganizationDbContext>(
            Configuration,
            builder =>
            {
                // builder.AddApiTypes()
                builder.AddTypeExtension<Mutation>();
                builder.AddTypeExtension<Query>();
                builder.ConfigureSchema(b => b.TryAddRootType(() => new ObjectType(d => d.Name(OperationTypeNames.Query)), OperationType.Query));
                builder
                    .ConfigureSchema(b => b.TryAddRootType(() => new ObjectType(d => d.Name(OperationTypeNames.Mutation)), OperationType.Mutation));
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
            .AddSkedularGrpcServices(Configuration);
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
