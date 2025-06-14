using Microsoft.Extensions.DependencyInjection;
using Payment.Shared.Mappers;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Payment.Shared.Services;

namespace Payment.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IStripeConnectAccountLinkService, StripeConnectAccountLinkService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>()
            .AddScoped<IStripeProductPricingService, StripeProductPricingService>()
            .AddSingleton<IOrganizationStripeConnectAccountHelper, OrganizationStripeConnectAccountHelper>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IAddressRepository, AddressRepository>()
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationOfferingRepository, OrganizationOfferingRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductVersionRepository, ProductVersionRepository>()
            .AddScoped<IStripeCheckoutSessionRepository, StripeCheckoutSessionRepository>()
            .AddScoped<IStripeConnectAccountRefreshCodeRepository, StripeConnectAccountRefreshCodeRepository>()
            .AddScoped<IStripeConnectAccountRepository, StripeConnectAccountRepository>()
            .AddScoped<IStripeConnectAccountAuthorizationRepository, StripeConnectAccountAuthorizationRepository>()
            .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
            .AddScoped<IStripePriceRepository, StripePriceRepository>()
            .AddScoped<IStripeProductRepository, StripeProductRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IPaymentPublisher, PaymentPublisher>()
            .AddScoped<IPaymentInternalPublisher, PaymentInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IPaymentOutboxPublisher, PaymentOutboxPublisher>();
}
