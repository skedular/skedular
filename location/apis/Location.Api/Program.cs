using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Context;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Security.Token;
using Enterprise.Shared.Temporal;
using Location.Api.Grpc;
using Location.Api.Services;
using Location.Shared;
using Location.Shared.Database;

namespace Location.Api;

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
            .WithPooledPostgreSqlDbContextFactory<LocationDbContext>(configuration, environment, "locationdb", true)
            .AddGraphql(configuration, "location-api", requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedConfigurations(configuration)
            .AddRootLevelSharedServices()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration)
            .AddTemporalClient(configuration, "temporal")
            .AddGrpc();

        // When running with FAKE_DEPENDENCIES=true (integration tests), replace the
        // empty token-service list with a pass-through that accepts any bearer token
        // value directly as the verifiable token — no external identity provider needed.
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

        app.MapGrpcService<LocationGrpcService>();
        app.MapGrpcService<LocationGraphqlGrpcService>();
        app.MapGrpcService<LocationResourcesGrpcService>();

        return app;
    }
}
