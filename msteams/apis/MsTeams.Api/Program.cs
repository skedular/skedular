using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using MsTeams.Api.Grpc;
using MsTeams.Shared;
using MsTeams.Shared.Database;

namespace MsTeams.Api;

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
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .WithPooledDbContextFactory<MsTeamsDbContext>(configuration, environment, "msteamsdb")
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        services.AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<MsTeamsGrpcService>();

        return app;
    }
}
