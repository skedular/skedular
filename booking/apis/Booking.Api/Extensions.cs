using Api.Shared.Services.Configurations.Grpc;
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
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
            .AddScoped<IBookingPaymentService, BookingPaymentService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IResourceService, ResourceService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
        ArgumentNullException.ThrowIfNull(bookingConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);

        return services
            .AddSingleton(bookingConfiguration);
    }
}
