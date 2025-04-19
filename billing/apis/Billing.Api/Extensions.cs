using Billing.Api.Mappers;
using Billing.Api.Services;
using Billing.Api.Services.Authorization;

namespace Billing.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IOrganizationBillingService, OrganizationBillingService>()
            .AddScoped<ICustomerBillingService, CustomerBillingService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
