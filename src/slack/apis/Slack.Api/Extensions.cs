using Api.Shared.Services.Configurations.Grpc;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Pages;
using Slack.Api.Services;
using BookingsPage = Slack.Api.Pages.BookingsPage;
using HomePage = Slack.Api.Pages.HomePage;
using SettingsPage = Slack.Api.Pages.SettingsPage;
using LocationsPage = Slack.Api.Pages.LocationsPage;
using TeamsPage = Slack.Api.Pages.TeamsPage;
using ZonesPage = Slack.Api.Pages.ZonesPage;
using BillingPage = Slack.Api.Pages.BillingPage;

namespace Slack.Api;

public static class Extensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddMappers() =>
            services.AddSingleton<IEntityMapper, EntityMapper>();

        public IServiceCollection AddServices() =>
            services
                .AddScoped<IWorkaroundService, WorkaroundService>()
                .AddScoped<IHomePageContextService, HomePageContextService>()
                .AddScoped<IBookingsPageContextService, BookingsPageContextService>()
                .AddScoped<ILocationsPageContextService, LocationsPageContextService>()
                .AddScoped<ITeamsPageContextService, TeamsPageContextService>()
                .AddScoped<IResourcesPageContextService, ResourcesPageContextService>()
                .AddScoped<IWorkspaceService, WorkspaceService>()
                .AddScoped<IWorkspaceOnboardingService, WorkspaceOnboardingService>()
                .AddScoped<IWorkspaceMemberService, WorkspaceMemberService>()
                .AddScoped<IWorkspaceChannelService, WorkspaceChannelService>();

        public IServiceCollection AddPages() =>
            services
                .AddScoped<IPageNavigator, PageNavigator>()
                .AddScoped<ICommonComponents, CommonComponents>()
                .AddScoped<IBookingComponents, BookingComponents>()
                .AddScoped<ILocationComponents, LocationComponents>()
                .AddScoped<ICustomTagComponents, CustomTagComponents>()
                .AddScoped<IZoneComponents, ZoneComponents>()
                .AddScoped<IResourceComponents, ResourceComponents>()
                .AddScoped<ITeamComponents, TeamComponents>()
                .AddScoped<IHomePage, HomePage>()
                .AddScoped<IBookingsPage, BookingsPage>()
                .AddScoped<ILocationsPage, LocationsPage>()
                .AddScoped<ICustomTagsPage, CustomTagsPage>()
                .AddScoped<ITeamsPage, TeamsPage>()
                .AddScoped<IZonesPage, ZonesPage>()
                .AddScoped<IResourcesPage, ResourcesPage>()
                .AddScoped<ISettingsPage, SettingsPage>()
                .AddScoped<IBillingPage, BillingPage>();

        public IServiceCollection AddJobs() =>
            services
                .AddSingleton<AsyncPageRenderingService>()
                .AddHostedService(sp => sp.GetRequiredService<AsyncPageRenderingService>());

        public IServiceCollection AddGrpcServices(IConfiguration configuration)
        {
            var slackConfiguration = configuration.GetSection(SlackConfiguration.Key).Get<SlackConfiguration>();
            ArgumentNullException.ThrowIfNull(slackConfiguration);
            ArgumentException.ThrowIfNullOrWhiteSpace(slackConfiguration.ApiKey);

            return services
                .AddSingleton(slackConfiguration);
        }
    }
}
