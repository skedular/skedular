using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services;

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<IResourceBookingSlotHelperService, ResourceBookingSlotHelperService>()
            .AddScoped<IResourceBookingSlotsHelperService, ResourceBookingSlotsHelperService>()
            .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
            .AddSingleton<IBookingCheckoutSessionHelperService, BookingCheckoutSessionHelperService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<IBookingCheckoutSessionRepository, BookingCheckoutSessionRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>()
            .AddScoped<IResourceBookingSlotRepository, ResourceBookingSlotRepository>()
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductVersionRepository, ProductVersionRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBookingInternalPublisher, BookingInternalPublisher>()
            .AddScoped<IBookingPublisher, BookingPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBookingInternalOutboxPublisher, BookingInternalOutboxPublisher>()
            .AddScoped<IBookingOutboxPublisher, BookingOutboxPublisher>();
}
