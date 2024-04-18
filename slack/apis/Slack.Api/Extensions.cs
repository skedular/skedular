using Slack.Api.Components;
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

    public static IServiceCollection AddJobs(this IServiceCollection services) =>
        services
            .AddHostedService<ConnectionKeepAliveJob>();

}
