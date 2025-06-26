using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
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
            .AddSingleton<IBookingCheckoutSessionHelperService, BookingCheckoutSessionHelperService>()
            .AddScoped<IResourceBookingSlotsHelperService, ResourceBookingSlotsHelperService>()
            .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
            .AddScoped<IStripeProductPricingService, StripeProductPricingService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services.AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IBookingRepository, BookingRepository>()
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
            .AddScoped<IProductVersionRepository, ProductVersionRepository>()
            .AddScoped<IStripeProductRepository, StripeProductRepository>()
            .AddScoped<IStripePriceRepository, StripePriceRepository>()
            .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
            .AddScoped<IStripeCheckoutSessionRepository, StripeCheckoutSessionRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBookingInternalPublisher, BookingInternalPublisher>()
            .AddScoped<IBookingPublisher, BookingPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBookingInternalOutboxPublisher, BookingInternalOutboxPublisher>()
            .AddScoped<IBookingOutboxPublisher, BookingOutboxPublisher>();

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

        services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);

        return services
            .AddSingleton(organizationConfiguration);
    }
}
