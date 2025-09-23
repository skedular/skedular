using Api.Shared.Services.Configurations.Grpc;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Api.Services.Authorization;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Security;

namespace Customer.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<ICustomerHelper>(sp => sp.GetRequiredService<ICachedCustomerService>())
            .AddScoped<ICustomerHelperService, CustomerHelperService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICustomerSettingsService, CustomerSettingsService>()
            .AddScoped<ICustomerOrganizationSettingsService, CustomerOrganizationSettingsService>()
            .AddScoped<ICustomerLocationSettingsService, CustomerLocationSettingsService>()
            .AddScoped<ICustomerResourceSettingsService, CustomerResourceSettingsService>()
            .AddScoped<ICustomerFeedbackService, CustomerFeedbackService>()
            .AddScoped<ICustomerDetailsService, CustomerDetailsService>()
            .AddScoped<ILocationAuthorizationService, LocationAuthorizationService>()
            .AddScoped<ICustomerOrganizationTagSettingsService, CustomerOrganizationTagSettingsService>()
            .AddScoped<IPaymentService, PaymentService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>()
            .AddScoped<IBillingService, BillingService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
        ArgumentNullException.ThrowIfNull(customerConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);

        return services
            .AddSingleton(customerConfiguration);
    }
}
