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
            .AddScoped<IStripeCustomerService, StripeCustomerService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) => services;
}
