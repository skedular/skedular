using Payment.Api.Mappers;
using Payment.Api.Services;
using Payment.Api.Services.Authorization;

namespace Payment.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IOrganizationService, OrganizationService>()
            .AddScoped<IOrganizationPaymentService, OrganizationPaymentService>()
            .AddScoped<IOrganizationStripeConnectAccountService, OrganizationStripeConnectAccountService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;
}
