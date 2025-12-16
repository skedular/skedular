using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Shared.Configurations;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;
using Organization.Shared.Services.Cache;

namespace Organization.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration)
        {
            var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
            ArgumentNullException.ThrowIfNull(emailConfiguration);

            return services.AddSingleton(emailConfiguration);
        }

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalService, TemporalService>()
                .AddSingleton<IGraphService, GraphService>()
                .AddScoped<IOrganizationStripeConnectAccountLinkService, OrganizationStripeConnectAccountLinkService>()
                .AddScoped<IOrganizationMemberService, OrganizationMemberService>()
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ICachedTagService, CachedTagService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<IOrganizationPhysicalAddressRepository, OrganizationPhysicalAddressRepository>()
                .AddScoped<IAzureInstallStateUserIdLookupRepository, AzureInstallStateUserIdLookupRepository>()
                .AddScoped<IAzureTenantRepository, AzureTenantRepository>()
                .AddScoped<IAzureTenantMemberRepository, AzureTenantMemberRepository>()
                .AddScoped<IBookingRepository, BookingRepository>()
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IDailyMemberCountRecordingRepository, DailyMemberCountRecordingRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<IIndustryMainCategoryRepository, IndustryMainCategoryRepository>()
                .AddScoped<IIndustrySubCategoryRepository, IndustrySubCategoryRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationOfferingActiveMemberRepository, OrganizationOfferingActiveMemberRepository>()
                .AddScoped<IOrganizationOfferingRepository, OrganizationOfferingRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<ITermsOfUseRepository, TermsOfUseRepository>()
                .AddScoped<ITagRepository, TagRepository>()
                .AddScoped<IOrganizationStripeCustomerRepository, OrganizationOrganizationStripeCustomerRepository>()
                .AddScoped<IOrganizationStripePaymentIntentRepository, OrganizationOrganizationStripePaymentIntentRepository>()
                .AddScoped<IOrganizationStripePaymentMethodRepository, OrganizationStripePaymentMethodRepository>()
                .AddScoped<IOrganizationBillingDetailsRepository, OrganizationOrganizationBillingDetailsRepository>()
                .AddScoped<IOrganizationStripeConnectAccountRefreshCodeRepository, OrganizationStripeConnectAccountRefreshCodeRepository>()
                .AddScoped<IOrganizationStripeConnectAccountRepository, OrganizationStripeConnectAccountRepository>()
                .AddScoped<IOrganizationStripeConnectAccountAuthorizationRepository, OrganizationStripeConnectAccountAuthorizationRepository>()
                .AddScoped<IOrganizationBankAccountRepository, OrganizationBankAccountRepository>()
                .AddScoped<IOrganizationTaxDetailsRepository, OrganizationTaxDetailsRepository>();

        public IServiceCollection AddPublishers() =>
            services
                .AddScoped<IOrganizationInternalPublisher, OrganizationInternalPublisher>()
                .AddScoped<IOrganizationPublisher, OrganizationPublisher>();

        public IServiceCollection AddOutboxPublishers() =>
            services
                .AddSingleton<IOrganizationOutboxPublisher, OrganizationOutboxPublisher>();

        public IServiceCollection AddDomainSharedGrpcClients(IConfiguration configuration)
        {
            var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
            ArgumentNullException.ThrowIfNull(customerConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

            var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
            ArgumentNullException.ThrowIfNull(locationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);

            services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);
            services.AddGrpcClient<LocationService.LocationServiceClient>(GrpcClients.ConfigureLocation);

            return services
                .AddSingleton(customerConfiguration)
                .AddSingleton(locationConfiguration);
        }
    }
}
