using Enterprise.Shared;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Payment.Api.Grpc;
using Payment.Shared;
using Payment.Shared.Database;

namespace Payment.Api;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .AddKafka(configuration)
            .AddSso()
            .AddSecurity(configuration)
            .WithPooledDbContextFactory<PaymentDbContext>(configuration, environment, "paymentdb")
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddStripe(configuration);

        services.AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<PaymentGrpcService>();

        return app;
    }
}
