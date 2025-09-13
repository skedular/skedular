using Marketplace.Shared.Mappers;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Marketplace.Shared.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services;

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ICachedProductService, CachedProductService>()
            .AddScoped<ICachedProductVersionService, CachedProductVersionService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductVersionRepository, ProductVersionRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IMarketplacePublisher, MarketplacePublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<IMarketplaceOutboxPublisher, MarketplaceOutboxPublisher>();
}
