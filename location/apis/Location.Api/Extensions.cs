using Enterprise.Shared.Storage;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;
using Location.Shared.Services;

namespace Location.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ILocationService, LocationService>()
            .AddScoped<ILocationOpeningHoursService, LocationOpeningHoursService>()
            .AddScoped<IResourceAvailableHoursService, ResourceAvailableHoursService>()
            .AddScoped<IResourceService, ResourceService>()
            .AddScoped<ILocationAnalyticsService, LocationAnalyticsService>()
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<IFileStorageService, LocalFileStorageService>()
            .AddScoped<IFloorPlanStorageService, FloorPlanStorageService>()
            .AddScoped<IFloorPlanService, FloorPlanService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
