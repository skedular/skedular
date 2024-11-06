using Enterprise.Shared;
using Enterprise.Shared.Application.WebHostService;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Outbox;
using Slack.Api.GraphQL;
using Slack.Api.Grpc;
using Slack.Api.Handlers.ActionHandlers.Billing;
using Slack.Api.Handlers.ActionHandlers.Booking;
using Slack.Api.Handlers.ActionHandlers.Commons;
using Slack.Api.Handlers.ActionHandlers.Desk;
using Slack.Api.Handlers.ActionHandlers.Feedback;
using Slack.Api.Handlers.ActionHandlers.Location;
using Slack.Api.Handlers.ActionHandlers.Team;
using Slack.Api.Handlers.ActionHandlers.Zone;
using Slack.Api.Handlers.OptionProviders;
using Slack.Api.Pages;
using Slack.Shared;
using Slack.Shared.Constants;
using Slack.Shared.Database;
using SlackNet.AspNetCore;
using SlackNet.Blocks;

namespace Slack.Api;

public class Startup(IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
    : StartupCustom(configuration, webHostEnvironment)
{
    protected override void ConfigureCustomServices(IServiceCollection services)
    {
        services
            .AddDatabase(Configuration, true, "SlackPostgresConnection")
            .WithPooledDbContextFactory<SlackDbContext>(Configuration, Migration.SetAssembly, Environment)
            .AddOutboxService()
            .AddDatabaseHealthCheck();

        services.AddKafka();

        services
            .AddGraphql<SlackDbContext>(Configuration, builder =>
            {
                builder
                    .AddQueryType<SlackQuery>();
            });

        services
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddServices()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddSlack(Configuration, options =>
            {
                PageNavigator.RegisterHandlers(options);
                HomePage.RegisterHandlers(options);
                BookingsPage.RegisterHandlers(options);
                LocationsPage.RegisterHandlers(options);
                TeamsPage.RegisterHandlers(options);
                ZonesPage.RegisterHandlers(options);
                DesksPage.RegisterHandlers(options);
                SettingsPage.RegisterHandlers(options);
                BillingPage.RegisterHandlers(options);

                options
                    .RegisterBlockOptionProvider<CountryOptionProvider>(OptionLoaderKeys.CountryKey)
                    .RegisterBlockOptionProvider<TimezoneOptionProvider>(OptionLoaderKeys.TimezoneKey)
                    .RegisterBlockOptionProvider<OrganizationMemberOptionProvider>(
                        OptionLoaderKeys.OrganizationMemberKey)
                    .RegisterBlockOptionProvider<OrganizationMemberAndCustomerPairOptionProvider>(
                        OptionLoaderKeys.OrganizationMemberAndCustomerPairKey)
                    .RegisterBlockOptionProvider<OrganizationLocationOptionProvider>(
                        OptionLoaderKeys.OrganizationLocationKey)
                    .RegisterBlockOptionProvider<OrganizationTeamOptionProvider>(OptionLoaderKeys.OrganizationTeamKey);

                options
                    .RegisterBlockActionHandler<ButtonAction, DismissSetupDefaultLocationButtonHandler>(
                        LocationActionTypes.DismissSetupDefaultLocation)
                    .RegisterBlockActionHandler<ButtonAction, DismissSetupPreferredDesksButtonHandler>(
                        DeskActionTypes.DismissSetupPreferredDesks)
                    .RegisterBlockActionHandler<ButtonAction, DismissSetupPreferredZonesButtonHandler>(
                        ZoneActionTypes.DismissSetupPreferredZones);

                options
                    .RegisterBlockActionHandler<ButtonAction, InstantAddBookingButtonHandler>(
                        BookingActionTypes.InstantAddBooking)
                    .RegisterBlockActionHandler<ButtonAction, JoinBookingButtonHandler>(
                        BookingActionTypes.JoinBooking)
                    .RegisterBlockActionHandler<ButtonAction, CancelBookingButtonHandler>(
                        BookingActionTypes.CancelBooking)
                    .RegisterBlockActionHandler<ButtonAction, AddBookingButtonHandler>(BookingActionTypes.AddBooking)
                    .RegisterViewSubmissionHandler<AddBookingButtonHandler>(BookingCallbackTypes.AddBooking)
                    .RegisterBlockActionHandler<ButtonAction, EditBookingButtonHandler>(BookingActionTypes.EditBooking)
                    .RegisterViewSubmissionHandler<EditBookingButtonHandler>(BookingCallbackTypes.EditBooking);

                options
                    .RegisterBlockActionHandler<ButtonAction, SendUsFeedbackButtonHandler>(CommonActionTypes
                        .SendUsFeedback)
                    .RegisterViewSubmissionHandler<SendUsFeedbackButtonHandler>(CommonCallbackTypes.SendUsFeedback);

                options
                    .RegisterViewSubmissionHandler<ViewBillingButtonHandler>(BillingCallbackTypes.ViewBilling)
                    .RegisterViewSubmissionHandler<EditBillingButtonHandler>(BillingCallbackTypes.EditBilling);

                options
                    .RegisterBlockActionHandler<ButtonAction, AddLocationButtonHandler>(LocationActionTypes.AddLocation)
                    .RegisterViewSubmissionHandler<AddLocationButtonHandler>(LocationCallbackTypes.AddLocation)
                    .RegisterViewSubmissionHandler<EditLocationButtonHandler>(LocationCallbackTypes.EditLocation)
                    .RegisterViewSubmissionHandler<RemoveLocationButtonHandler>(LocationCallbackTypes.RemoveLocation);

                options
                    .RegisterBlockActionHandler<ButtonAction, AddTeamButtonHandler>(TeamActionTypes.AddTeam)
                    .RegisterViewSubmissionHandler<AddTeamButtonHandler>(TeamCallbackTypes.AddTeam)
                    .RegisterViewSubmissionHandler<EditTeamButtonHandler>(TeamCallbackTypes.EditTeam)
                    .RegisterViewSubmissionHandler<RemoveTeamButtonHandler>(TeamCallbackTypes.RemoveTeam);

                options
                    .RegisterBlockActionHandler<ButtonAction, AddZoneButtonHandler>(ZoneActionTypes.AddZone)
                    .RegisterViewSubmissionHandler<AddZoneButtonHandler>(ZoneCallbackTypes.AddZone)
                    .RegisterViewSubmissionHandler<EditZoneButtonHandler>(ZoneCallbackTypes.EditZone)
                    .RegisterViewSubmissionHandler<RemoveZoneButtonHandler>(ZoneCallbackTypes.RemoveZone);

                options
                    .RegisterBlockActionHandler<ButtonAction, AddDeskButtonHandler>(DeskActionTypes.AddDesk)
                    .RegisterViewSubmissionHandler<AddDeskButtonHandler>(DeskCallbackTypes.AddDesk)
                    .RegisterBlockActionHandler<ButtonAction, BulkAddDesksButtonHandler>(DeskActionTypes.BulkAddDesks)
                    .RegisterViewSubmissionHandler<BulkAddDesksButtonHandler>(DeskCallbackTypes.BulkAddDesks)
                    .RegisterViewSubmissionHandler<EditDeskButtonHandler>(DeskCallbackTypes.EditDesk)
                    .RegisterViewSubmissionHandler<RemoveDeskButtonHandler>(DeskCallbackTypes.RemoveDesk);

                Enumerable.Range(0, 31).ForEach(idx => options
                    .RegisterBlockActionHandler<ButtonAction, InstantAddBookingButtonHandler>(
                        $"{BookingActionTypes.InstantAddBooking}{idx}"));

                Enumerable.Range(0, 31).ForEach(idx => options
                    .RegisterBlockActionHandler<ButtonAction, CancelBookingButtonHandler>(
                        $"{BookingActionTypes.CancelBooking}{idx}"));
            })
            .AddUnityHubGrpcServices(Configuration)
            .AddPages();
    }

    public override void Configure(IApplicationBuilder app)
    {
        app.UseApplicationBuilderDefaults(
            Environment,
            Configuration,
            configureEndpointRouteBuilder: endpointRouteBuilder =>
            {
                endpointRouteBuilder.MapGrpcService<SlackGrpcService>();
            }
        );

        app.UseSlackNet(c => c.MapToPrefix("slack/api/v1"));
    }
}
