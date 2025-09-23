using Customer.Shared.Configurations;
using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Customer.Shared.Services;
using Customer.Shared.Services.Cache;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        return services;
    }

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<ITemporalOutboxExecutor, TemporalOutboxExecutorService>()
            .AddSingleton<ITemporalService, TemporalService>()
            .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerFeedbackRepository, CustomerFeedbackRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<IResourceRepository, ResourceRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IStripeCustomerRepository, StripeCustomerRepository>()
            .AddScoped<IStripePaymentIntentRepository, StripePaymentIntentRepository>()
            .AddScoped<IStripePaymentMethodRepository, StripePaymentMethodRepository>()
            .AddScoped<ICustomerBillingDetailsRepository, CustomerBillingDetailsRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services.AddSingleton<ICustomerPublisher, CustomerPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ICustomerOutboxPublisher, CustomerOutboxPublisher>()
            .AddSingleton<ITemporalOutboxPublisher, TemporalOutboxPublisher>();
}
