using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Notification.Api.Grpc;
using Notification.Shared;
using Notification.Shared.Database;

namespace Notification.Api;

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
            .WithPooledDbContextFactory<NotificationDbContext>(configuration, environment, "notificationdb")
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddMappers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        services.AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<NotificationGrpcService>();

        return app;
    }
}
