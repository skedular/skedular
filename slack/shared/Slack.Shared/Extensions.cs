using Api.Shared.Services.Grpc.UnityHub.Billing.V1;
using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Grpc.UnityHub.MsTeams.V1;
using Api.Shared.Services.Grpc.UnityHub.Notification.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Api.Shared.Services.Grpc.UnityHub.Payment.V1;
using Api.Shared.Services.Grpc.UnityHub.Team.V1;
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
    public static IServiceCollection AddDomainSharedMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddDomainSharedServices(this IServiceCollection services) =>
        services
            .AddScoped<IWorkspaceMemberService, WorkspaceMemberService>();

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
            .AddScoped<ITeamRepository, TeamRepository>()
            .AddScoped<IWorkspaceChannelRepository, WorkspaceChannelRepository>()
            .AddScoped<IWorkspaceMemberRepository, WorkspaceMemberRepository>()
            .AddScoped<IWorkspaceRepository, WorkspaceRepository>();

    public static IServiceCollection AddPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ISlackInternalPublisher, SlackInternalPublisher>();

    public static IServiceCollection AddOutboxPublishers(this IServiceCollection services) =>
        services
            .AddScoped<ISlackInternalOutboxPublisher, SlackInternalOutboxPublisher>();

    public static IServiceCollection AddSlack(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<AspNetSlackServiceConfiguration>? configure)
    {
        var slackConfiguration = configuration.GetSection(SlackConfiguration.Key).Get<SlackConfiguration>();
        ArgumentNullException.ThrowIfNull(slackConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.SigningSecret);

        return services
            .AddScoped<IBookingComponents, BookingComponents>()
            .AddSlackNet(option =>
            {
                option.UseSigningSecret(slackConfiguration.SigningSecret);
                configure?.Invoke(option);
            });
    }

    public static IServiceCollection AddUnityHubGrpcServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var slackConfiguration = configuration.GetSection(SlackConfiguration.Key).Get<SlackConfiguration>();
        ArgumentNullException.ThrowIfNull(slackConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.ApiKey);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.AppId);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.ClientId);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.ClientSecret);
        ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.SigningSecret);
        ArgumentNullException.ThrowIfNull(slackConfiguration.RedirectUrl);
        ArgumentNullException.ThrowIfNull(slackConfiguration.SuccessInstallUrl);

        var billingConfiguration =
            configuration.GetSection(BillingConfiguration.Key).Get<BillingConfiguration>();
        ArgumentNullException.ThrowIfNull(billingConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(billingConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(billingConfiguration.GrpcUrl);

        var bookingConfiguration =
            configuration.GetSection(BookingConfiguration.Key).Get<BookingConfiguration>();
        ArgumentNullException.ThrowIfNull(bookingConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(bookingConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(bookingConfiguration.GrpcUrl);

        var customerConfiguration =
            configuration.GetSection(CustomerConfiguration.Key).Get<CustomerConfiguration>();
        ArgumentNullException.ThrowIfNull(customerConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(customerConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(customerConfiguration.GrpcUrl);

        var locationConfiguration =
            configuration.GetSection(LocationConfiguration.Key).Get<LocationConfiguration>();
        ArgumentNullException.ThrowIfNull(locationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(locationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(locationConfiguration.GrpcUrl);

        var msTeamsConfiguration =
            configuration.GetSection(MsTeamsConfiguration.Key).Get<MsTeamsConfiguration>();
        ArgumentNullException.ThrowIfNull(msTeamsConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(msTeamsConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(msTeamsConfiguration.GrpcUrl);

        var notificationConfiguration =
            configuration.GetSection(NotificationConfiguration.Key).Get<NotificationConfiguration>();
        ArgumentNullException.ThrowIfNull(notificationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(notificationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(notificationConfiguration.GrpcUrl);

        var organizationConfiguration =
            configuration.GetSection(OrganizationConfiguration.Key).Get<OrganizationConfiguration>();
        ArgumentNullException.ThrowIfNull(organizationConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(organizationConfiguration.GrpcUrl);

        var paymentConfiguration =
            configuration.GetSection(PaymentConfiguration.Key).Get<PaymentConfiguration>();
        ArgumentNullException.ThrowIfNull(paymentConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(paymentConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(paymentConfiguration.GrpcUrl);

        var teamConfiguration =
            configuration.GetSection(TeamConfiguration.Key).Get<TeamConfiguration>();
        ArgumentNullException.ThrowIfNull(teamConfiguration);
        ArgumentException.ThrowIfNullOrWhiteSpace(teamConfiguration.ApiKey);
        ArgumentNullException.ThrowIfNull(teamConfiguration.GrpcUrl);

        services.AddGrpcClient<BillingService.BillingServiceClient>(GrpcClients.ConfigureBilling);
        services.AddGrpcClient<BookingService.BookingServiceClient>(GrpcClients.ConfigureBooking);
        services.AddGrpcClient<CustomerService.CustomerServiceClient>(GrpcClients.ConfigureCustomer);
        services.AddGrpcClient<LocationService.LocationServiceClient>(GrpcClients.ConfigureLocation);
        services.AddGrpcClient<NotificationService.NotificationServiceClient>(GrpcClients.ConfigureNotification);
        services.AddGrpcClient<MsTeamsService.MsTeamsServiceClient>(GrpcClients.ConfigureMsTeams);
        services.AddGrpcClient<OrganizationService.OrganizationServiceClient>(GrpcClients.ConfigureOrganization);
        services.AddGrpcClient<PaymentService.PaymentServiceClient>(GrpcClients.ConfigurePayment);
        services.AddGrpcClient<TeamService.TeamServiceClient>(GrpcClients.ConfigureTeam);

        return services
            .AddSingleton(slackConfiguration)
            .AddSingleton(billingConfiguration)
            .AddSingleton(bookingConfiguration)
            .AddSingleton(customerConfiguration)
            .AddSingleton(locationConfiguration)
            .AddSingleton(msTeamsConfiguration)
            .AddSingleton(notificationConfiguration)
            .AddSingleton(organizationConfiguration)
            .AddSingleton(paymentConfiguration)
            .AddSingleton(teamConfiguration);
    }
}
