using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Shared.Mappers;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Payment.Shared.Services;
using Stripe;
using StripeConfiguration = Payment.Shared.Configurations.StripeConfiguration;

namespace Payment.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IStripeConnectAccountLinkService, StripeConnectAccountLinkService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IAddressRepository, AddressRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationOfferingRepository, OrganizationOfferingRepository>()
            .AddScoped<IOrganizationStripeConnectAccountRepository, OrganizationStripeConnectAccountRepository>()
            .AddScoped<IOrganizationStripeConnectAccountRefreshCodeRepository, OrganizationStripeConnectAccountRefreshCodeRepository>()
            .AddScoped<IProductRepository, ProductRepository>()
            .AddScoped<IProductVersionRepository, ProductVersionRepository>()
            .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
            .AddScoped<IStripePaymentIntentRepository, StripePaymentIntentRepository>()
            .AddScoped<IStripePaymentMethodRepository, StripePaymentMethodRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IPaymentPublisher, PaymentPublisher>()
            .AddScoped<IPaymentInternalPublisher, PaymentInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IPaymentOutboxPublisher, PaymentOutboxPublisher>();

    public static IServiceCollection AddStripe(this IServiceCollection services, IConfiguration configuration)
    {
        var stripeConfiguration = configuration.GetSection(StripeConfiguration.Key).Get<StripeConfiguration>();

        ArgumentNullException.ThrowIfNull(stripeConfiguration);

        if (string.IsNullOrWhiteSpace(stripeConfiguration.SecretKey))
        {
            Console.Error.WriteLine("stripeConfiguration.SecretKey is null");
        }

        Stripe.StripeConfiguration.ApiKey = stripeConfiguration.SecretKey;

        return services
            .AddSingleton(stripeConfiguration)
            .AddSingleton<ICreatable<Customer, CustomerCreateOptions>, CustomerService>()
            .AddSingleton<IUpdatable<Customer, CustomerUpdateOptions>, CustomerService>()
            .AddSingleton<ICreatable<Account, AccountCreateOptions>, AccountService>()
            .AddSingleton<IUpdatable<Account, AccountUpdateOptions>, AccountService>()
            .AddSingleton<IDeletable<Account, AccountDeleteOptions>, AccountService>()
            .AddSingleton<ICreatable<AccountLink, AccountLinkCreateOptions>, AccountLinkService>()
            .AddSingleton<ICreatable<PaymentIntent, PaymentIntentCreateOptions>, PaymentIntentService>()
            .AddSingleton<ICreatable<SetupIntent, SetupIntentCreateOptions>, SetupIntentService>()
            .AddSingleton<IRetrievable<SetupIntent, SetupIntentGetOptions>, SetupIntentService>()
            .AddSingleton<IRetrievable<PaymentMethod, PaymentMethodGetOptions>, PaymentMethodService>()
            .AddSingleton<ICreatable<Product, ProductCreateOptions>, ProductService>()
            .AddSingleton<IUpdatable<Product, ProductUpdateOptions>, ProductService>()
            .AddSingleton<IDeletable<Product, ProductDeleteOptions>, ProductService>()
            .AddSingleton<IRetrievable<Product, ProductGetOptions>, ProductService>()
            .AddSingleton<IListable<Product, ProductListOptions>, ProductService>()
            .AddSingleton<ICreatable<Price, PriceCreateOptions>, PriceService>()
            .AddSingleton<IUpdatable<Price, PriceUpdateOptions>, PriceService>()
            .AddSingleton<IRetrievable<Price, PriceGetOptions>, PriceService>()
            .AddSingleton<PaymentMethodService>();
    }
}
