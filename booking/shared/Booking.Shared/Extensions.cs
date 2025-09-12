using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Core.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Booking.Shared.Configurations;
using Booking.Shared.Mappers;
using Booking.Shared.Publishers;
using Booking.Shared.Repositories;
using Booking.Shared.Services;
using Booking.Shared.Services.Cache;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Booking.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        return services;
    }

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>()
            .AddSingleton<ITemporalService, TemporalService>()
            .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<IBookingCheckoutSessionHelperService, BookingCheckoutSessionHelperService>()
            .AddSingleton<ITemporalOutboxExecutor, TemporalOutboxExecutorService>()
            .AddSingleton<ITemporalSignalOutboxExecutor, TemporalSignalOutboxExecutorService>()
            .AddScoped<ILocationResourceBookingSlotsHelperService, LocationResourceBookingSlotsHelperService>()
            .AddScoped<IBookingResourceSlotsHelperService, BookingResourceSlotsHelperService>()
            .AddScoped<IStripeProductPricingService, StripeProductPricingService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>()
            .AddScoped<IBookingInvoiceService, BookingInvoiceService>()
            .AddScoped<IOrganizationInvoiceCounterService, OrganizationInvoiceCounterService>();

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
            .AddScoped<IStripeCheckoutSessionRepository, StripeCheckoutSessionRepository>()
            .AddScoped<IOrganizationInvoiceCounterRepository, OrganizationInvoiceCounterRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<IBookingInternalPublisher, BookingInternalPublisher>()
            .AddSingleton<IBookingPublisher, BookingPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<IBookingOutboxPublisher, BookingOutboxPublisher>()
            .AddSingleton<ITemporalOutboxPublisher, TemporalOutboxPublisher>();

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var coreConfiguration = configuration.GetSection(CoreConfiguration.Key).Get<CoreConfiguration>();
        ArgumentNullException.ThrowIfNull(coreConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(coreConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(coreConfiguration.GrpcUrl);

        var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

        services.AddGrpcClient<CoreService.CoreServiceClient>(GrpcClients.ConfigureCore);
        services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);

        return services
            .AddSingleton(coreConfiguration)
            .AddSingleton(organizationConfiguration);
    }
}
