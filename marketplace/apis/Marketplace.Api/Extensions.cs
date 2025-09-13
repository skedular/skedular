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
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IProductService, ProductService>()
            .AddScoped<IProductVersionService, ProductVersionService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();

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
