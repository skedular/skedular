using Location.Shared.Configurations;
using Location.Shared.Mappers;
using Location.Shared.Publishers;
using Location.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Location.Shared;

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
            .AddScoped<IAddressRepository, AddressRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IDailyDeskCountRecordingRepository, DailyDeskCountRecordingRepository>()
            .AddScoped<IDailyRoomCountRecordingRepository, DailyRoomCountRecordingRepository>()
            .AddScoped<IDeskRepository, DeskRepository>()
            .AddScoped<IRoomRepository, RoomRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IJoinInvitationRepository, JoinInvitationRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<ILocationMemberRepository, LocationMemberRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ILocationInternalPublisher, LocationInternalPublisher>()
            .AddScoped<ILocationPublisher, LocationPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ILocationOutboxPublisher, LocationOutboxPublisher>()
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddSkedularGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
        ArgumentNullException.ThrowIfNull(locationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);

        return services.AddSingleton(locationConfiguration);
    }
}
