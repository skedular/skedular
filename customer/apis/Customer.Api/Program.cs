using Api.Shared.Services;
using Customer.Api.Grpc;
using Customer.Shared;
using Customer.Shared.Database;
using Enterprise.Shared;
using Enterprise.Shared.Cache;
using Enterprise.Shared.Database.PostgreSql;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Payment;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Enterprise.Shared.Temporal;

namespace Customer.Api;

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
            .WithPooledPostgreSqlDbContextFactory<CustomerDbContext>(configuration, environment, "customerdb", true)
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
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
            .AddStripe(configuration)
            .AddTemporalClient(configuration, "temporal")
            .AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults<Program>()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<CustomerGrpcService>();

        return app;
    }
}
