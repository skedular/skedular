using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.Outbox;
using Team.Shared;
using Team.Shared.Configurations;
using Team.Shared.Database;

namespace Team.Jobs;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddServiceDefaults<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .WithPooledDbContextFactory<TeamDbContext>(configuration, environment, "TeamPostgresConnection")
            .AddOutboxBackgroundService<TeamDbContext>()
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
