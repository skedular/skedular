using Booking.Api.Mappers;
using Booking.Api.Services;
using Booking.Api.Services.Authorization;

namespace Booking.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ILocationAuthorizationService, LocationAuthorizationService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IDeskService, DeskService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();
}
