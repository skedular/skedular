using Marketplace.Shared.Mappers;
using Marketplace.Shared.Publishers;
using Marketplace.Shared.Repositories;
using Marketplace.Shared.Services.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Marketplace.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration) =>
            services;

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedProductService, CachedProductService>()
                .AddScoped<ICachedProductVersionService, CachedProductVersionService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
                .AddScoped<IProductRepository, ProductRepository>()
                .AddScoped<IProductVersionRepository, ProductVersionRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<ILocationPhysicalAddressRepository, LocationPhysicalAddressRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddScoped<IMarketplacePublisher, MarketplacePublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<IMarketplaceOutboxPublisher, MarketplaceOutboxPublisher>();
    }
}
