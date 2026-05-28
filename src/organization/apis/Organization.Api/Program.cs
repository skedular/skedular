using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Accounting;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Temporal;
using Organization.Api.Grpc;
using Organization.Api.Services;
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
        _ = services.AddKafka(configuration, "kafka");

        services
            .AddRedis(configuration, "redis")
            .AddSso()
            .AddSecurity()
            .WithPooledPostgreSqlDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb", true)
            .AddGraphql(configuration, "organization-api", requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddXeroServices(configuration)
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
            .AddSharedCrossDomainClients(configuration)
            .AddStripe(configuration)
            .AddTemporalClient(configuration, "temporal")
            .AddGrpc();

        if (DomainAppHostEnvironmentVariables.IsFakeDependenciesEnabled())
        {
            services.AddSingleton<IEnumerable<ITokenService>>(sp =>
                [new FakeTokenService(sp.GetRequiredService<IContext>())]);
        }

        var app = builder
            .Build()
            .UseWebApplicationDefaults<Program>()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<OrganizationGrpcService>();
        app.MapGrpcService<OrganizationGraphqlGrpcService>();
        app.MapGrpcService<OrganizationTagsGrpcService>();
        app.MapGrpcService<OrganizationZonesGrpcService>();
        app.MapGrpcService<OrganizationBillingGrpcService>();

        return app;
    }
}
