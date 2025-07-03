using Api.Shared.Services.Configurations.Grpc;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;

namespace Organization.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IWorkaroundService, WorkaroundService>()
            .AddScoped<IAzureTenantOnboardingService, AzureTenantOnboardingService>()
            .AddScoped<IAzureTenantService, AzureTenantService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<IOrganizationTermsOfUseService, OrganizationTermsOfUseService>()
            .AddScoped<IIndustryMainCategoryService, IndustryMainCategoryService>()
            .AddScoped<IOrganizationService, OrganizationService>()
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<IOrganizationMemberService, OrganizationMemberService>()
            .AddScoped<IOrganizationAnalyticsService, OrganizationAnalyticsService>()
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationInvitationService, OrganizationInvitationService>()
            .AddScoped<ITagService, TagService>()
            .AddScoped<IOrganizationSsoService, OrganizationSsoService>()
            .AddScoped<IPaymentService, PaymentService>()
            .AddScoped<IStripeCustomerService, StripeCustomerService>()
            .AddScoped<IOrganizationBillingService, OrganizationBillingService>()
            .AddScoped<IOrganizationStripeConnectAccountService, OrganizationStripeConnectAccountService>()
            .AddScoped<IOrganizationBankAccountService, OrganizationBankAccountService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);

        return services
            .AddSingleton(organizationConfiguration);
    }
}
