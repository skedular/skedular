using Api.Shared.Services.Configurations.Grpc;
using Team.Api.Mappers;
using Team.Api.Services;
using Team.Api.Services.Authorization;

namespace Team.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IMapper, Mapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IOrganizationAuthorizationService, OrganizationAuthorizationService>()
                .AddScoped<IOrganizationSsoAuthorizationService, OrganizationSsoAuthorizationService>()
                .AddScoped<IOrganizationOfferingService, OrganizationOfferingService>()
                .AddScoped<ITeamAuthorizationService, TeamAuthorizationService>()
                .AddScoped<ITeamService, TeamService>()
                .AddScoped<ICustomerService, CustomerService>()
                .AddScoped<ITeamMemberService, TeamMemberService>()
                .AddScoped<IInvitationService, InvitationService>()
                .AddScoped<IWorkaroundService, WorkaroundService>();

        public IServiceCollection AddJobs() =>
            services;

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var teamConfiguration = configuration.GetSection(TeamConfiguration.Key).Get<TeamConfiguration>();
            ArgumentNullException.ThrowIfNull(teamConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(teamConfiguration.ApiKey);

            return services
                .AddSingleton(teamConfiguration);
        }
    }
}
