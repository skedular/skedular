using Booking.Api.GraphQL;
using Booking.Api.Grpc;
using Booking.Shared;
using Booking.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using HotChocolate.Language;
using HotChocolate.Types;

namespace Booking.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "BookingPostgresConnection")
            .WithPooledDbContextFactory<BookingDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();
        services.AddRedis(Configuration);

        services.AddGraphql<BookingDbContext>(
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
            .AddMappers()
            .AddJobs()
            .AddSkedularGrpcServices(Configuration);
    }

    public override void Configure(IApplicationBuilder app) =>
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<BookingGrpcService>();
            }
        );
}
