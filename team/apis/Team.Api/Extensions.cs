using Api.Shared.Services.Configurations.Grpc;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;

namespace Team.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
            .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
            .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<ICachedCustomerService, CachedCustomerService>()
            .AddScoped<ITeamService, TeamService>()
            .AddScoped<ITeamMemberService, TeamMemberService>()
            .AddScoped<ITeamInvitationService, TeamInvitationService>()
            .AddScoped<IWorkaroundService, WorkaroundService>();

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services;

    public static IServiceCollection AddGrpcServices(this IServiceCollection services, IConfiguration configuration)
    {
        var teamConfiguration = configuration.GetSection(TeamConfiguration.Key).Get<TeamConfiguration>();
        ArgumentNullException.ThrowIfNull(teamConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(teamConfiguration.ApiKey);

        return services
            .AddSingleton(teamConfiguration);
    }
}
