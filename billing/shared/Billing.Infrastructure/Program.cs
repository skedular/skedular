using Billing.Infrastructure.Services;
using Billing.Shared;
using Billing.Shared.Database;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;

namespace Billing.Infrastructure;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunAsync();

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args).AddServiceDefaults<Program>();
        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        services
            .WithPooledDbContextFactory<BillingDbContext>(configuration, environment, "billingdb")
            .AddRepositoryFactory()
            .AddServices()
            .AddJobs();

        return builder.Build().AddWebApplicationDefaults();
    }

    public static async Task MigrateAsync(WebApplication app, CancellationToken cancellationToken)
    {
        await using var scope = app.Services.CreateAsyncScope();
        await scope.ServiceProvider.GetRequiredService<IMigrationService>().MigrateAsync(cancellationToken);
    }
}
