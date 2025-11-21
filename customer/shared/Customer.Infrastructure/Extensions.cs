using Customer.Infrastructure.Jobs;
using Customer.Infrastructure.Services;

namespace Customer.Infrastructure;

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
