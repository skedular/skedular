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
            .AddScoped<ILocationAuthorizationService, LocationAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ILocationService, LocationService>()
            .AddScoped<ILocationOpeningHoursService, LocationOpeningHoursService>()
            .AddScoped<IResourceAvailableHoursService, ResourceAvailableHoursService>()
            .AddScoped<ILocationMemberService, LocationMemberService>()
            .AddScoped<IResourceService, ResourceService>()
            .AddScoped<ILocationAnalyticsService, LocationAnalyticsService>()
            .AddScoped<ILocationInvitationService, LocationInvitationService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
