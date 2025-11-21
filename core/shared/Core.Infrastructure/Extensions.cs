using Core.Infrastructure.Jobs;
using Core.Infrastructure.Services;

namespace Core.Infrastructure;

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
