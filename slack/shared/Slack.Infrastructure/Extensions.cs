using Slack.Infrastructure.Jobs;
using Slack.Infrastructure.Services;

namespace Slack.Infrastructure;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices() =>
            services
                .AddScoped<IMigrationService, MigrationService>();

        public IServiceCollection AddJobs() =>
            services
                .AddHostedService<InfrastructureMigrationJob>();
    }
}
