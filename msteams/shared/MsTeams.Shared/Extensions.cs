using Enterprise.Shared.Outbox.Temporal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Mappers;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;
using MsTeams.Shared.Services.Cache;

namespace MsTeams.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration) =>
            services;

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IEntityMapper, EntityMapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<IWorkflowIdService, WorkflowIdService>()
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddSingleton<IGraphService, GraphService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<IAzureTenantRepository, AzureTenantRepository>()
                .AddScoped<IAzureTenantTeamChannelRepository, AzureTenantTeamChannelRepository>()
                .AddScoped<IAzureTenantTeamRepository, AzureTenantTeamRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<ITeamRepository, TeamRepository>();

        public IServiceCollection AddPublishers() =>
            services;

        public IServiceCollection AddOutboxPublishers() =>
            services;
    }
}
