using Enterprise.Shared;
using Enterprise.Shared.Database.Postgres;
using Enterprise.Shared.Kafka;
using Organization.Infrastructure.Services;
using Organization.Shared;
using Organization.Shared.Database;

namespace Organization.Infrastructure;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddDefaultServices<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;
        _ = services.AddKafka(configuration, "kafka");

        services
            .WithPooledDbContextFactory<OrganizationDbContext>(configuration, environment, "organizationdb", true)
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs();

        return builder.Build().UseWebApplicationDefaults<Program>();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
