using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Temporal;
using Organization.Api.Grpc;
using Organization.Shared;
using Organization.Shared.Database;

namespace Organization.Api;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        _ = services.AddKafka(configuration);

        services
            .AddRedis(configuration, "redis")
            .AddSso()
            .AddSecurity()
            .WithPooledDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb", true)
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddMappers()
            .AddGrpcServices(configuration)
            .AddGrpcClients(configuration)
            .AddStripe(configuration)
            .AddTemporalClient(configuration);

        services.AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults<Program>()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<OrganizationGrpcService>();

        return app;
    }
}
