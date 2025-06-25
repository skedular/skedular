using Api.Shared.Services.Configurations.Grpc;
using Customer.Api.Mappers;
using Customer.Api.Services;
using Customer.Api.Services.Authorization;
using Enterprise.Shared.Security;

namespace Customer.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) => services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<ICustomerHelper, CustomerHelper>()
            .AddScoped<ICustomerHelperService, CustomerHelperService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ICustomerSettingsService, CustomerSettingsService>()
            .AddScoped<ICustomerOrganizationSettingsService, CustomerOrganizationSettingsService>()
            .AddScoped<ICustomerLocationSettingsService, CustomerLocationSettingsService>()
            .AddScoped<ICustomerTeamSettingsService, CustomerTeamSettingsService>()
            .AddScoped<ICustomerResourceSettingsService, CustomerResourceSettingsService>()
            .AddScoped<ICustomerFeedbackService, CustomerFeedbackService>()
            .AddScoped<ICustomerDetailsService, CustomerDetailsService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<ILocationAuthorizationService, LocationAuthorizationService>()
            .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
            .AddScoped<ICustomerOrganizationTagSettingsService, CustomerOrganizationTagSettingsService>()
            .AddScoped<IPaymentService, PaymentService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>()
            .AddScoped<IMyBillingService, MyBillingService>();

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
