using Payment.Infrastructure.Jobs;
using Payment.Infrastructure.Services;

namespace Payment.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IMigrationService, MigrationService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<InfrastructureMigrationJob>();
}
