using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Team.Shared.Configurations;
using Team.Shared.Mappers;
using Team.Shared.Publishers;
using Team.Shared.Repositories;

namespace Team.Shared;

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
            .AddScoped<IBookingRepository, BookingRepository>()
            .AddScoped<ICustomerRepository, CustomerRepository>()
            .AddScoped<IIdentityRepository, IdentityRepository>()
            .AddScoped<IJoinInvitationRepository, JoinInvitationRepository>()
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<IOrganizationRepository, OrganizationRepository>()
            .AddScoped<ITeamMemberRepository, TeamMemberRepository>()
            .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
            .AddScoped<ILocationRepository, LocationRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ITeamPublisher, TeamPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ITeamOutboxPublisher, TeamOutboxPublisher>()
            .AddScoped<INotificationOutboxPublisher, NotificationOutboxPublisher>();

    public static IServiceCollection AddSkedularGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var teamConfiguration = configuration.GetSection(TeamConfiguration.Key).Get<TeamConfiguration>();
        ArgumentNullException.ThrowIfNull(teamConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(teamConfiguration.ApiKey);

        return services
            .AddSingleton(teamConfiguration);
    }
}
