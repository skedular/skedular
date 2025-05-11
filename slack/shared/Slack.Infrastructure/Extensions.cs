using Slack.Infrastructure.Jobs;
using Slack.Infrastructure.Services;

namespace Slack.Infrastructure;

public static class Extensions
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IMigrationService, MigrationService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<InfrastructureMigrationJob>();
}
