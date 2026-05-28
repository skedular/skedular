using MsTeams.Infrastructure.Jobs;
using MsTeams.Infrastructure.Services;

namespace MsTeams.Infrastructure;

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
