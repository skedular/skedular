using Customer.Shared.Configurations;
using Customer.Shared.Mappers;
using Customer.Shared.Publishers;
using Customer.Shared.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Customer.Shared;

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
            .AddScoped<ICustomerFeedbackRepository, CustomerFeedbackRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<ILocationRepository, LocationRepository>()
            .AddScoped<ILocationResourceRepository, LocationResourceRepository>()
            .AddScoped<ILocationMemberRepository, LocationMemberRepository>()
            .AddScoped<IDeskRepository, DeskRepository>()
            .AddScoped<IRoomRepository, RoomRepository>()
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
            .AddScoped<IOrganizationTagRepository, OrganizationTagRepository>()
            .AddScoped<IOrganizationResourceTypeRepository, OrganizationResourceTypeRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) => services.AddScoped<ICustomerPublisher, CustomerPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ICustomerOutboxPublisher, CustomerOutboxPublisher>()
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddSkedularGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
        ArgumentNullException.ThrowIfNull(customerConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);

        return services
            .AddSingleton(customerConfiguration);
    }
}
