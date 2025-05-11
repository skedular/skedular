using Marketplace.Infrastructure.Jobs;
using Marketplace.Infrastructure.Services;

namespace Marketplace.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IMigrationService, MigrationService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<InfrastructureMigrationJob>();
}
