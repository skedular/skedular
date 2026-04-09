using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Stripe;
using Stripe.Checkout;
using StripeConfiguration = Enterprise.Shared.Payment.Configurations.StripeConfiguration;

namespace Enterprise.Shared.Payment;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers Stripe SDK service interfaces as singletons and sets the global
        ///     <c>Stripe.StripeConfiguration.ApiKey</c> from the <c>Stripe:SecretKey</c> config value.
        ///     Reads configuration from the <c>Stripe</c> section.
        ///     Inject the narrowest Stripe interface needed in domain code rather than the concrete
        ///     service class so the Stripe boundary can be mocked in tests.
        /// </summary>
        /// <param name="configuration">Application configuration (reads the <c>Stripe</c> section).</param>
        public IServiceCollection AddStripe(IConfiguration configuration)
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
                .AddSingleton<IDeletable<Customer, CustomerDeleteOptions>, CustomerService>()
                .AddSingleton<IListable<Customer, CustomerListOptions>, CustomerService>()
                .AddSingleton<IListable<Customer, CustomerListOptions>, CustomerService>()
                .AddSingleton<IRetrievable<Customer, CustomerGetOptions>, CustomerService>()
                .AddSingleton<ICreatable<Account, AccountCreateOptions>, AccountService>()
                .AddSingleton<IUpdatable<Account, AccountUpdateOptions>, AccountService>()
                .AddSingleton<IDeletable<Account, AccountDeleteOptions>, AccountService>()
                .AddSingleton<IRetrievable<Account, AccountGetOptions>, AccountService>()
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
                .AddSingleton<ICreatable<Session, SessionCreateOptions>, SessionService>()
                .AddSingleton<IListable<Session, SessionListOptions>, SessionService>()
                .AddSingleton<IRetrievable<Session, SessionGetOptions>, SessionService>()
                .AddSingleton<IUpdatable<Session, SessionUpdateOptions>, SessionService>()
                .AddSingleton<ICreatable<OAuthToken, OAuthTokenCreateOptions>, OAuthTokenService>()
                .AddSingleton<PaymentMethodService>();
        }
    }
}
