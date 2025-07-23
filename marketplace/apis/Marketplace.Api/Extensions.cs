using Api.Shared.Services.Configurations.Grpc;
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
            .AddSingleton<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddSingleton<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var msTeamsConfiguration = configuration.GetSection(MarketplaceConfiguration.Key).Get<MarketplaceConfiguration>();
        ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);

        return services
            .AddSingleton(msTeamsConfiguration);
    }
}
