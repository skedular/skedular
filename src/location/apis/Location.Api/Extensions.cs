using Api.Shared.Services.Configurations.Grpc;
using Api.Shared.Services.Offering;
using Location.Api.Mappers;
using Location.Api.Services;
using Location.Api.Services.Authorization;

namespace Location.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services
                .AddSingleton<IGraphQlMapper, GraphQlMapper>()
                .AddSingleton<IGrpcMapper, GrpcMapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IPricingEntitlementEvaluator, PricingEntitlementEvaluator>()
                .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<ILocationService, LocationService>()
                .AddScoped<ILocationRestrictedInformationService, LocationRestrictedInformationService>()
                .AddScoped<ILocationBookingAccessService, LocationBookingAccessService>()
                .AddScoped<ILocationOwnershipService, LocationOwnershipService>()
                .AddScoped<ILocationOpeningHoursService, LocationOpeningHoursService>()
                .AddScoped<IResourceAvailableHoursService, ResourceAvailableHoursService>()
                .AddScoped<IResourceService, ResourceService>()
                .AddScoped<IBulkAddResourcesService, BulkAddResourcesService>()
                .AddScoped<ILocationAnalyticsService, LocationAnalyticsService>()
                .AddScoped<IWorkaroundService, WorkaroundService>()
                .AddScoped<IFloorPlanService, FloorPlanService>()
                .AddScoped<ILocationPhysicalAddressService, LocationPhysicalAddressService>()
                .AddScoped<ILocationContactedViaService, LocationContactedViaService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
            ArgumentNullException.ThrowIfNull(locationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);

            return services.AddSingleton(locationConfiguration);
        }
    }
}
