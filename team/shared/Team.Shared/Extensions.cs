using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;
using Team.Shared.Services;
using Team.Shared.Services.Cache;

namespace Team.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration) =>
        services;

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<ITemporalOutboxExecutor, TemporalOutboxExecutorService>()
            .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ICachedTeamService, CachedTeamService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IJoinInvitationRepository, JoinInvitationRepository>()
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
            .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<ILocationRepository, LocationRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ITeamPublisher, TeamPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ITeamOutboxPublisher, TeamOutboxPublisher>()
            .AddSingleton<ITemporalOutboxPublisher, TemporalOutboxPublisher>();

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
        ArgumentNullException.ThrowIfNull(customerConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

        services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);

        return services
            .AddSingleton(customerConfiguration);
    }
}
