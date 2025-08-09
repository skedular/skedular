using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Location.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services;

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ILocationPhysicalAddressRepository, LocationPhysicalAddressRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IDailyDeskCountRecordingRepository, DailyDeskCountRecordingRepository>()
            .AddScoped<IDailyRoomCountRecordingRepository, DailyRoomCountRecordingRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IFloorPlanRepository, FloorPlanRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ILocationInternalPublisher, LocationInternalPublisher>()
            .AddSingleton<ILocationPublisher, LocationPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ILocationOutboxPublisher, LocationOutboxPublisher>();
}
