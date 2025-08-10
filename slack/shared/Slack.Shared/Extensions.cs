using Api.Shared.Clients.Configurations.Grpc;
using Api.Shared.Clients.Grpc;
using Api.Shared.Services.Grpc.Skedular.Booking.V1;
using Api.Shared.Services.Grpc.Skedular.Customer.V1;
using Api.Shared.Services.Grpc.Skedular.Location.V1;
using Api.Shared.Services.Grpc.Skedular.Organization.V1;
using Api.Shared.Services.Grpc.Skedular.Team.V1;
using Enterprise.Shared.Outbox;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Slack.Shared.Components;
using Slack.Shared.Configurations;
using Slack.Shared.Mappers;
using Slack.Shared.Publishers;
using Slack.Shared.Repositories;
using Slack.Shared.Services;
using SlackNet.AspNetCore;

namespace Slack.Shared;

public static class Extensions
{
    public static IServiceCollection AddDomainSharedConfigurations(this IServiceCollection services, IConfiguration configuration)
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

        return services.AddSingleton(slackConfigurationService);
    }

    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services
            .AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddSingleton<ITemporalOutboxExecutor, TemporalOutboxExecutorService>()
            .AddScoped<ILocationDailyUpdaterService, LocationDailyUpdaterService>()
            .AddScoped<ITeamDailyUpdaterService, TeamDailyUpdaterService>()
            .AddScoped<IWorkspaceService, WorkspaceService>()
            .AddScoped<IWorkspaceMemberService, WorkspaceMemberService>()
            .AddScoped<IWorkspaceChannelService, WorkspaceChannelService>();

    public static IServiceCollection AddRepositoryFactory(this IServiceCollection services) =>
        services
            .AddScoped<IRepositoryFactory, RepositoryFactory>();

    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
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

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ISlackInternalPublisher, SlackInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddSingleton<ISlackInternalOutboxPublisher, SlackInternalOutboxPublisher>()
            .AddSingleton<ITemporalOutboxPublisher, TemporalOutboxPublisher>();

    public static IServiceCollection AddSlack(
        this IServiceCollection services,
        IConfiguration configuration,
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

    public static IServiceCollection AddGrpcClients(this IServiceCollection services, IConfiguration configuration)
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
        services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);
        services.AddGrpcClient<LocationService.LocationServiceClient>(GrpcClients.ConfigureLocation);
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
