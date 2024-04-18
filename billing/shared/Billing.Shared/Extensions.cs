using Billing.Shared.Configurations;
using Billing.Shared.Mappers;
using Billing.Shared.Publishers;
using Billing.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Billing.Shared;

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
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationOfferingRepository, OrganizationOfferingRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBillingInternalPublisher, BillingInternalPublisher>()
            .AddScoped<IBillingPublisher, BillingPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<IBillingOutboxPublisher, BillingOutboxPublisher>();

    public static IServiceCollection AddUnityHubGrpcServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var billingConfiguration =
            configuration.GetSection(BillingConfiguration.Key).Get<BillingConfiguration>();
        ArgumentNullException.ThrowIfNull(billingConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(billingConfiguration.ApiKey);

        return services
            .AddSingleton(billingConfiguration);
    }
}
