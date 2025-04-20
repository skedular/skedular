using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Payment.Shared.Mappers;
using Payment.Shared.Publishers;
using Payment.Shared.Repositories;
using Stripe;
using StripeConfiguration = Payment.Shared.Configurations.StripeConfiguration;

namespace Payment.Shared;

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
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<IOrganizationOfferingRepository, OrganizationOfferingRepository>()
            .AddScoped<IOrganizationOfferingStripePaymentIntentRepository, OrganizationOfferingStripePaymentIntentRepository>()
            .AddScoped<IOrganizationStripePaymentMethodRepository, OrganizationStripePaymentMethodRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services;

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
            .AddScoped<ICreatable<Customer, CustomerCreateOptions>, CustomerService>()
            .AddScoped<IUpdatable<Customer, CustomerUpdateOptions>, CustomerService>()
            .AddScoped<ICreatable<Account, AccountCreateOptions>, AccountService>()
            .AddScoped<IUpdatable<Account, AccountUpdateOptions>, AccountService>()
            .AddScoped<ICreatable<PaymentIntent, PaymentIntentCreateOptions>, PaymentIntentService>()
            .AddScoped<ICreatable<SetupIntent, SetupIntentCreateOptions>, SetupIntentService>()
            .AddScoped<IRetrievable<SetupIntent, SetupIntentGetOptions>, SetupIntentService>()
            .AddScoped<IRetrievable<PaymentMethod, PaymentMethodGetOptions>, PaymentMethodService>();
    }
}
