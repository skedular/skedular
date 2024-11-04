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
}
