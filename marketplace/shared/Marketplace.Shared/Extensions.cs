using Marketplace.Shared.Configurations;
using Marketplace.Shared.Mappers;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Shared;

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
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductVersionRepository, ProductVersionRepository>()
            .AddScoped<IOrganizationStripeConnectAccountRepository, OrganizationStripeConnectAccountRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IMarketplacePublisher, MarketplacePublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IMarketplaceOutboxPublisher, MarketplaceOutboxPublisher>();

    public static IServiceCollection AddSkedularGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var msTeamsConfiguration = configuration.GetSection(MarketplaceConfiguration.Key).Get<MarketplaceConfiguration>();
        ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);

        return services
            .AddSingleton(msTeamsConfiguration);
    }
}
