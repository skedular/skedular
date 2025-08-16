using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Mappers;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;

namespace MsTeams.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services;

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>()
            .AddSingleton<ITemporalService, TemporalService>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<IGraphService, GraphService>()
            .AddSingleton<ITemporalOutboxExecutor, TemporalOutboxExecutorService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
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

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services;
}
