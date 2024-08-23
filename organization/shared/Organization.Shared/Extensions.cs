using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Shared.Configurations;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;
using Organization.Shared.Services;

namespace Organization.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IMsGraphService, MsGraphService>()
            .AddSingleton<IMsGraphServiceClientService, MsGraphServiceClientService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
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
            .AddScoped<ITermsOfUseRepository, TermsOfUseRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationInternalPublisher, OrganizationInternalPublisher>()
            .AddScoped<IOrganizationPublisher, OrganizationPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationOutboxPublisher, OrganizationOutboxPublisher>()
            .AddScoped<IOrganizationInternalOutboxPublisher, OrganizationInternalOutboxPublisher>()
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddUnityHubGrpcServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var organizationConfiguration =
            configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);

        var customerConfiguration =
            configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
        ArgumentNullException.ThrowIfNull(customerConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

        var locationConfiguration =
            configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
        ArgumentNullException.ThrowIfNull(locationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);

        services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);
        services.AddGrpcClient<LocationService.LocationServiceClient>(GrpcClients.ConfigureLocation);

        return services
            .AddSingleton(organizationConfiguration)
            .AddSingleton(customerConfiguration)
            .AddSingleton(locationConfiguration);
    }
}
