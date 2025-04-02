using Marketplace.Api.Mappers;
using Marketplace.Api.Services;
using Marketplace.Api.Services.Authorization;

namespace Marketplace.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IProductService, ProductService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
