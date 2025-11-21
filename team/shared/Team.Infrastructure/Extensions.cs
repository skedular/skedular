using Team.Infrastructure.Jobs;
using Team.Infrastructure.Services;

namespace Team.Infrastructure;

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
