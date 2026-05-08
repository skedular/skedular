using Api.Shared.Services.Configurations.Grpc;
using Organization.Api.Mappers;
using Organization.Api.Services;
using Organization.Api.Services.Authorization;
using IOrganizationMemberService = Organization.Api.Services.IOrganizationMemberService;
using OrganizationMemberService = Organization.Api.Services.OrganizationMemberService;

namespace Organization.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services
                .AddSingleton<IGraphQlMapper, GraphQlMapper>()
                .AddSingleton<IGrpcMapper, GrpcMapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<IWorkaroundService, WorkaroundService>()
                .AddScoped<IAzureTenantOnboardingService, AzureTenantOnboardingService>()
                .AddScoped<IAzureTenantService, AzureTenantService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<IOrganizationTermsOfUseService, OrganizationTermsOfUseService>()
                .AddScoped<IIndustryMainCategoryService, IndustryMainCategoryService>()
                .AddScoped<IOrganizationService, OrganizationService>()
                .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
                .AddScoped<IOrganizationMemberService, OrganizationMemberService>()
                .AddScoped<IOrganizationAnalyticsService, OrganizationAnalyticsService>()
                .AddScoped<IInvitationService, InvitationService>()
                .AddScoped<ITagService, TagService>()
                .AddScoped<IOrganizationSsoService, OrganizationSsoService>()
                .AddScoped<IPaymentService, PaymentService>()
                .AddScoped<IStripeCustomerService, StripeCustomerService>()
                .AddScoped<IOrganizationBillingService, OrganizationBillingService>()
                .AddScoped<IOrganizationStripeConnectAccountService, OrganizationStripeConnectAccountService>()
                .AddScoped<IOrganizationBankAccountService, OrganizationBankAccountService>()
                .AddScoped<IOrganizationTaxDetailsService, OrganizationTaxDetailsService>()
                .AddScoped<IOrganizationXeroConnectionService, OrganizationXeroConnectionService>()
                .AddScoped<IOrganizationPhysicalAddressService, OrganizationPhysicalAddressService>()
                .AddScoped<IOrganizationOwnershipService, OrganizationOwnershipService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
            ArgumentNullException.ThrowIfNull(organizationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);

            return services
                .AddSingleton(organizationConfiguration);
        }
    }
}
