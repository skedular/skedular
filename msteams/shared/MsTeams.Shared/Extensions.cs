using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Configurations;
using MsTeams.Shared.Mappers;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;

namespace MsTeams.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services;

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
        services
            .AddScoped<IMsTeamsInternalPublisher, MsTeamsInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var msTeamsConfiguration = configuration.GetSection(MsTeamsConfiguration.Key).Get<MsTeamsConfiguration>();
        ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);

        return services
            .AddSingleton(msTeamsConfiguration);
    }
}
