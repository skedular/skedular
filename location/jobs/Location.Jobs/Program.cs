using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Location.Shared;
using Location.Shared.Database;

namespace Location.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddServiceDefaults<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .WithPooledDbContextFactory<LocationDbContext>(configuration, environment, "LocationPostgresConnection")
            .AddOutboxBackgroundService<LocationDbContext>()
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddGrpcServices(configuration);

        return builder.Build().AddWebApplicationDefaults();
    }
}
