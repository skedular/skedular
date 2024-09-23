using Slack.Api.Components;
using Slack.Api.Handlers.ActionHandlers.Booking;
using Slack.Api.Handlers.ActionHandlers.Commons;
using Slack.Api.Handlers.ActionHandlers.Desk;
using Slack.Api.Handlers.ActionHandlers.Feedback;
using Slack.Api.Handlers.ActionHandlers.Location;
using Slack.Api.Handlers.ActionHandlers.Team;
using Slack.Api.Handlers.ActionHandlers.Zone;
using Slack.Api.Jobs;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using BookingsPage = Slack.Api.Pages.BookingsPage;
using HomePage = Slack.Api.Pages.HomePage;
using SettingsPage = Slack.Api.Pages.SettingsPage;
using LocationsPage = Slack.Api.Pages.LocationsPage;
using TeamsPage = Slack.Api.Pages.TeamsPage;
using ZonesPage = Slack.Api.Pages.ZonesPage;
using DesksPage = Slack.Api.Pages.DesksPage;
using BillingPage = Slack.Api.Pages.BillingPage;

namespace Slack.Api;

public static class Extensions
{
    public static IServiceCollection AddMappers(this IServiceCollection services) =>
        services.AddSingleton<IMapper, Mapper>();

    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IHomePageContextService, HomePageContextService>()
            .AddScoped<IBookingsPageContextService, BookingsPageContextService>()
            .AddScoped<ILocationsPageContextService, LocationsPageContextService>()
            .AddScoped<ITeamsPageContextService, TeamsPageContextService>()
            .AddScoped<IDesksPageContextService, DesksPageContextService>()
            .AddScoped<ICustomerService, CustomerService>()
            .AddScoped<IOrganizationService, OrganizationService>()
            .AddScoped<ILocationService, LocationService>()
            .AddScoped<ITeamService, TeamService>()
            .AddScoped<IBookingService, BookingService>()
            .AddScoped<IBillingService, BillingService>()
            .AddScoped<IWorkspaceService, WorkspaceService>()
            .AddScoped<IWorkspaceOnboardingService, WorkspaceOnboardingService>()
            .AddScoped<IWorkspaceMemberService, WorkspaceMemberService>()
            .AddScoped<IWorkspaceChannelService, WorkspaceChannelService>();

    public static IServiceCollection AddPages(this IServiceCollection services) =>
        services
            .AddScoped<IPageNavigator, PageNavigator>()
            .AddScoped<ICommonComponents, CommonComponents>()
            .AddScoped<IBookingComponents, BookingComponents>()
            .AddScoped<ILocationComponents, LocationComponents>()
            .AddScoped<IZoneComponents, ZoneComponentsComponents>()
            .AddScoped<IDeskComponents, DeskComponents>()
            .AddScoped<ITeamComponents, TeamComponents>()
            .AddScoped<ISettingsComponents, SettingsComponents>()
            .AddScoped<IHomePage, HomePage>()
            .AddScoped<IBookingsPage, BookingsPage>()
            .AddScoped<ILocationsPage, LocationsPage>()
            .AddScoped<ITeamsPage, TeamsPage>()
            .AddScoped<IZonesPage, ZonesPage>()
            .AddScoped<IDesksPage, DesksPage>()
            .AddScoped<ISettingsPage, SettingsPage>()
            .AddScoped<IBillingPage, BillingPage>();

    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        services
            .AddHostedService<ConnectionKeepAliveJob>();

        services
            .AddSingleton<AsyncPageRenderingService<BookingsPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<BookingsPage>>())
            .AddSingleton<AsyncPageRenderingService<DesksPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<DesksPage>>())
            .AddSingleton<AsyncPageRenderingService<HomePage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<HomePage>>())
            .AddSingleton<AsyncPageRenderingService<HomePage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<HomePage>>())
            .AddSingleton<AsyncPageRenderingService<LocationsPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<LocationsPage>>())
            .AddSingleton<AsyncPageRenderingService<PageNavigator>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<PageNavigator>>())
            .AddSingleton<AsyncPageRenderingService<SettingsPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<SettingsPage>>())
            .AddSingleton<AsyncPageRenderingService<TeamsPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<TeamsPage>>())
            .AddSingleton<AsyncPageRenderingService<ZonesPage>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<ZonesPage>>());

        services
            .AddSingleton<AsyncPageRenderingService<AddBookingButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<AddBookingButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<CancelBookingButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<CancelBookingButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<EditBookingButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<EditBookingButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<InstantAddBookingButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<InstantAddBookingButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<JoinBookingButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<JoinBookingButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<DismissSetupDefaultLocationButtonHandler>>()
            .AddHostedService(sp =>
                sp.GetRequiredService<AsyncPageRenderingService<DismissSetupDefaultLocationButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<DismissSetupPreferredDesksButtonHandler>>()
            .AddHostedService(sp =>
                sp.GetRequiredService<AsyncPageRenderingService<DismissSetupPreferredDesksButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<DismissSetupPreferredZonesButtonHandler>>()
            .AddHostedService(sp =>
                sp.GetRequiredService<AsyncPageRenderingService<DismissSetupPreferredZonesButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<AddDeskButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<AddDeskButtonHandler>>())
            .AddSingleton<AsyncPageRenderingService<BulkAddDesksButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<BulkAddDesksButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<SendUsFeedbackButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<SendUsFeedbackButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<AddLocationButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<AddLocationButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<AddTeamButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<AddTeamButtonHandler>>());

        services
            .AddSingleton<AsyncPageRenderingService<AddZoneButtonHandler>>()
            .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService<AddZoneButtonHandler>>());

        return services;
    }
}
