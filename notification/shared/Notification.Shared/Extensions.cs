using Microsoft.Extensions.DependencyInjection;
using Notification.Shared.Mappers;
using Notification.Shared.Repositories;

namespace Notification.Shared;

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
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<INotificationRepository, NotificationRepository>()
            .AddScoped<ITeamRepository, TeamRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services;
}
