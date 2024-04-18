using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MsTeams.Shared.Configurations;
using MsTeams.Shared.Factories;
using MsTeams.Shared.Mappers;
using MsTeams.Shared.Publishers;
using MsTeams.Shared.Repositories;
using MsTeams.Shared.Services;

namespace MsTeams.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IMsGraphService, MsGraphService>()
            .AddSingleton<IGraphServiceClientFactory, CreateGraphServiceClientFactory>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ITemporaryAuthorizationCodeRepository, TemporaryAuthorizationCodeRepository>()
            .AddScoped<ITenantRepository, TenantRepository>()
            .AddScoped<ITenantMemberRepository, TenantMemberRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IMsTeamsInternalPublisher, MsTeamsInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddAzure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var azureAdConfiguration = configuration.GetSection(AzureAdConfiguration.Key).Get<AzureAdConfiguration>();
        ArgumentNullException.ThrowIfNull(azureAdConfiguration);
        services.AddSingleton(azureAdConfiguration);

        var graphApiConfiguration = configuration.GetSection(GraphApiConfiguration.Key).Get<GraphApiConfiguration>();
        ArgumentNullException.ThrowIfNull(graphApiConfiguration);
        services.AddSingleton(graphApiConfiguration);

        return services;
    }
}
