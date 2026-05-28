using Api.Shared.Services.Configurations.Grpc;
using MsTeams.Api.Services;

namespace MsTeams.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddServices() =>
            services
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IWorkaroundService, WorkaroundService>();

        public IServiceCollection AddMappers() =>
            services;

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var msTeamsConfiguration = configuration.GetSection(MsTeamsConfiguration.Key).Get<MsTeamsConfiguration>();
            ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);

            return services
                .AddSingleton(msTeamsConfiguration);
        }
    }
}
