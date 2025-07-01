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
using Organization.Shared.Workflows.Services;

namespace Organization.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var organizationConfigurationService = configuration.GetSection(OrganizationConfigurationService.Key).Get<OrganizationConfigurationService>();
        ArgumentNullException.ThrowIfNull(organizationConfigurationService);

        return services.AddSingleton(organizationConfigurationService);
    }

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>()
            .AddScoped<ITemporalOutboxExecutor, TemporalOutboxExecutorService>()
            .AddScoped<ITemporalSignalOutboxExecutor, TemporalSignalOutboxExecutorService>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationStripeConnectAccountLinkService, OrganizationStripeConnectAccountLinkService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IAddressRepository, AddressRepository>()
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
            .AddScoped<IOrganizationBankAccountRepository, OrganizationBankAccountRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationInternalPublisher, OrganizationInternalPublisher>()
            .AddScoped<IOrganizationPublisher, OrganizationPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationOutboxPublisher, OrganizationOutboxPublisher>()
            .AddScoped<IOrganizationInternalOutboxPublisher, OrganizationInternalOutboxPublisher>()
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
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
