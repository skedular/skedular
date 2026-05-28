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
using Marketplace.Api.Grpc;
using Marketplace.Api.Services;
using Marketplace.Shared;
using Marketplace.Shared.Database;

namespace Marketplace.Api;

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
            .WithPooledPostgreSqlDbContextFactory<MarketplaceDbContext>(configuration, environment, "marketplacedb", true)
            .AddGraphql(configuration, "marketplace-api", requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
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
            .AddGrpcServices(configuration)
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

        app.MapGrpcService<MarketplaceGrpcService>();
        app.MapGrpcService<MarketplaceGraphqlGrpcService>();

        return app;
    }
}
