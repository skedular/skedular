using Api.Shared.Services.Configurations.Grpc;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;

namespace Location.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddSingleton<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ILocationService, LocationService>()
            .AddScoped<ILocationOpeningHoursService, LocationOpeningHoursService>()
            .AddScoped<IResourceAvailableHoursService, ResourceAvailableHoursService>()
            .AddScoped<IResourceService, ResourceService>()
            .AddScoped<ILocationAnalyticsService, LocationAnalyticsService>()
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<IFloorPlanService, FloorPlanService>()
            .AddScoped<ILocationPhysicalAddressService, LocationPhysicalAddressService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
        ArgumentNullException.ThrowIfNull(locationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);

        return services.AddSingleton(locationConfiguration);
    }
}
