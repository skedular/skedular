using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Organization.Shared.Configurations;
using Organization.Shared.Mappers;
using Organization.Shared.Publishers;
using Organization.Shared.Repositories;

namespace Organization.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services;

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
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddUnityHubGrpcServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var organizationConfiguration =
            configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);

        return services
            .AddSingleton(organizationConfiguration);
    }
}
