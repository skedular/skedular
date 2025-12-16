using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slack.Shared.Components;
using Slack.Shared.Configurations;
using Slack.Shared.Mappers;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using Slack.Shared.Services.Cache;
using Slack.Shared.Services.CrossDomains;
using SlackNet.AspNetCore;
using BookingService = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService;
using CustomerService = Slack.Shared.Services.CrossDomains.CustomerService;
using LocationService = Slack.Shared.Services.CrossDomains.LocationService;
using OrganizationService = Api.Shared.Services.Grpc.Skedular.Organization.V1.OrganizationService;
using TeamService = Api.Shared.Services.Grpc.Skedular.Team.V1.TeamService;

namespace Slack.Shared;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDomainSharedConfigurations(IConfiguration configuration)
        {
            var slackConfigurationService = configuration.GetSection(SlackConfigurationService.Key).Get<SlackConfigurationService>();
            ArgumentNullException.ThrowIfNull(slackConfigurationService);

            if (string.IsNullOrWhiteSpace(slackConfigurationService.AppId))
            {
                Console.Error.WriteLine("slackConfiguration.AppId is null");
            }

            if (string.IsNullOrWhiteSpace(slackConfigurationService.ClientId))
            {
                Console.Error.WriteLine("slackConfiguration.ClientId is null");
            }

            if (string.IsNullOrWhiteSpace(slackConfigurationService.ClientSecret))
            {
                Console.Error.WriteLine("slackConfiguration.ClientSecret is null");
            }

            if (string.IsNullOrWhiteSpace(slackConfigurationService.SigningSecret))
            {
                Console.Error.WriteLine("slackConfiguration.SigningSecret is null");
            }

            if (slackConfigurationService.RedirectUrl is null)
            {
                Console.Error.WriteLine("slackConfiguration.RedirectUrl is null");
            }

            if (slackConfigurationService.SuccessInstallUrl is null)
            {
                Console.Error.WriteLine("slackConfiguration.SuccessInstallUrl is null");
            }

            var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
            ArgumentNullException.ThrowIfNull(emailConfiguration);

            return services
                .AddSingleton(slackConfigurationService)
                .AddSingleton(emailConfiguration);
        }

        public IServiceCollection AddDomainSharedMappers() =>
            services
                .AddSingleton<IMapper, Mapper>()
                .AddSingleton<ITemporalService, TemporalService>();

        public IServiceCollection AddDomainSharedServices() =>
            services
                .AddSingleton<ITemporalOutboxService, TemporalOutboxService>()
                .AddSingleton<ITemporalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddSingleton<ITemporalSignalOutboxExecutor>(sp => sp.GetRequiredService<ITemporalOutboxService>())
                .AddScoped<ICachedOrganizationService, CachedOrganizationService>()
                .AddScoped<ICachedCustomerService, CachedCustomerService>()
                .AddScoped<ILocationDailyUpdaterService, LocationDailyUpdaterService>()
                .AddScoped<ITeamDailyUpdaterService, TeamDailyUpdaterService>()
                .AddScoped<IWorkspaceService, WorkspaceService>()
                .AddScoped<IWorkspaceMemberService, WorkspaceMemberService>()
                .AddScoped<IWorkspaceChannelService, WorkspaceChannelService>()
                .AddSingleton<ICustomerService, CustomerService>()
                .AddSingleton<IOrganizationService, Services.CrossDomains.OrganizationService>()
                .AddSingleton<IOrganizationPermissionsService, OrganizationPermissionsService>()
                .AddSingleton<ILocationService, LocationService>()
                .AddSingleton<ILocationPermissionsService, LocationPermissionsService>()
                .AddSingleton<ITeamService, Services.CrossDomains.TeamService>()
                .AddSingleton<ITeamPermissionsService, TeamPermissionsService>()
                .AddSingleton<IOrganizationMemberService, OrganizationMemberService>()
                .AddSingleton<IOrganizationTagService, OrganizationTagService>()
                .AddSingleton<IOrganizationCustomTagService, OrganizationCustomTagService>()
                .AddSingleton<IOrganizationZoneService, OrganizationZoneService>()
                .AddSingleton<IOrganizationProductTagService, OrganizationProductTagService>()
                .AddSingleton<ILocationResourceService, LocationResourceService>()
                .AddSingleton<IBookingPermissionsService, BookingPermissionsService>()
                .AddSingleton<IBookingService, Services.CrossDomains.BookingService>()
                .AddSingleton<IOrganizationBillingService, OrganizationBillingService>();

        public IServiceCollection AddRepositoryFactory() =>
            services
                .AddScoped<IRepositoryFactory, RepositoryFactory>();

        public IServiceCollection AddRepositories() =>
            services
                .AddScoped<ICustomerRepository, CustomerRepository>()
                .AddScoped<IIdentityRepository, IdentityRepository>()
                .AddScoped<ILocationRepository, LocationRepository>()
                .AddScoped<IOrganizationRepository, OrganizationRepository>()
                .AddScoped<IOrganizationMemberRepository, OrganizationMemberRepository>()
                .AddScoped<IOrganizationSsoSettingRepository, OrganizationSsoSettingRepository>()
                .AddScoped<ITeamRepository, TeamRepository>()
                .AddScoped<IWorkspaceChannelRepository, WorkspaceChannelRepository>()
                .AddScoped<IWorkspaceMemberRepository, WorkspaceMemberRepository>()
                .AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        public IServiceCollection AddPublishers() =>
            services;

        public IServiceCollection AddOutboxPublishers() =>
            services;

        public IServiceCollection AddSlack(IConfiguration configuration,
            Action<AspNetSlackServiceConfiguration>? configure)
        {
            var slackConfiguration = configuration.GetSection(SlackConfiguration.Key).Get<SlackConfigurationService>();
            ArgumentNullException.ThrowIfNull(slackConfiguration);

            if (string.IsNullOrWhiteSpace(slackConfiguration.SigningSecret))
            {
                Console.Error.WriteLine("slackConfiguration.SigningSecret is null");
            }

            return services
                .AddScoped<IBookingComponents, BookingComponents>()
                .AddSlackNet(option =>
                {
                    option.UseSigningSecret(slackConfiguration.SigningSecret);
                    configure?.Invoke(option);
                });
        }

        public IServiceCollection AddDomainSharedGrpcClients(IConfiguration configuration)
        {
            var bookingConfiguration = configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
            ArgumentNullException.ThrowIfNull(bookingConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

            var customerConfiguration = configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
            ArgumentNullException.ThrowIfNull(customerConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

            var locationConfiguration = configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
            ArgumentNullException.ThrowIfNull(locationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);

            var organizationConfiguration = configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
            ArgumentNullException.ThrowIfNull(organizationConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

            var teamConfiguration = configuration.GetSection(TeamConfiguration.Key).Get<TeamConfiguration>();
            ArgumentNullException.ThrowIfNull(teamConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(teamConfiguration.ApiKey);
            ArgumentNullException.ThrowIfNull(teamConfiguration.GrpcUrl);

            services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureBooking);
            services.AddGrpcClient<Api.Shared.Services.Grpc.Skedular.Customer.V1.CustomerService.CustomerServiceClient>(GrpcClients
                .ConfigureCustomer);
            services.AddGrpcClient<Api.Shared.Services.Grpc.Skedular.Location.V1.LocationService.LocationServiceClient>(GrpcClients
                .ConfigureLocation);
            services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);
            services.AddGrpcClient<TeamService.TeamServiceClient>(GrpcClients.ConfigureTeam);

            return services
                .AddSingleton(bookingConfiguration)
                .AddSingleton(customerConfiguration)
                .AddSingleton(locationConfiguration)
                .AddSingleton(organizationConfiguration)
                .AddSingleton(teamConfiguration);
        }
    }
}
