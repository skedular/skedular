using Enterprise.Shared;
using Enterprise.Shared.Cdn;
using Enterprise.Shared.Database;
using Enterprise.Shared.GraphQL;
using Enterprise.Shared.Kafka;
using Enterprise.Shared.Security;
using Enterprise.Shared.Security.Sso;
using Slack.Api.Grpc;
using Slack.Api.Handlers.ActionHandlers.Billing;
using Slack.Api.Handlers.ActionHandlers.Booking;
using Slack.Api.Handlers.ActionHandlers.Commons;
using Slack.Api.Handlers.ActionHandlers.CustomTag;
using Slack.Api.Handlers.ActionHandlers.Feedback;
using Slack.Api.Handlers.ActionHandlers.Location;
using Slack.Api.Handlers.ActionHandlers.Resource;
using Slack.Api.Handlers.ActionHandlers.Team;
using Slack.Api.Handlers.ActionHandlers.Zone;
using Slack.Api.Handlers.OptionProviders;
using Slack.Api.Pages;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Database;
using SlackNet.AspNetCore;
using SlackNet.Blocks;

namespace Slack.Api;

public class Program
{
    public static async Task Main(string[] args) => await CreateHostBuilder(args).RunWithGraphQLCommandsAsync(args);

    public static WebApplication CreateHostBuilder(string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(args)
            .AddDefaultServices<Program>();

        var services = builder.Services;
        var configuration = builder.Configuration;
        var environment = builder.Environment;

        var emailConfiguration = configuration.GetSection(EmailConfiguration.Key).Get<EmailConfiguration>();
        ArgumentNullException.ThrowIfNull(emailConfiguration);
        services.AddSingleton(emailConfiguration);

        services
            .AddKafka(configuration)
            .AddCdn(configuration)
            .AddSso()
            .AddSecurity()
            .WithPooledDbContextFactory<SlackDbContext>(configuration, environment, "slackdb")
            .AddGraphql(configuration, requestExecutorBuilder => { requestExecutorBuilder.AddApiTypes(); })
            .AddDomainSharedServices()
            .AddDomainSharedMappers()
            .AddMappers()
            .AddRepositoryFactory()
            .AddPublishers()
            .AddOutboxPublishers()
            .AddJobs()
            .AddServices()
            .AddSlack(
                configuration,
                options =>
                {
                    PageNavigator.RegisterHandlers(options);
                    HomePage.RegisterHandlers(options);
                    BookingsPage.RegisterHandlers(options);
                    LocationsPage.RegisterHandlers(options);
                    CustomTagsPage.RegisterHandlers(options);
                    TeamsPage.RegisterHandlers(options);
                    ZonesPage.RegisterHandlers(options);
                    ResourcesPage.RegisterHandlers(options);
                    SettingsPage.RegisterHandlers(options);
                    BillingPage.RegisterHandlers(options);

                    options
                        .RegisterBlockOptionProvider<CountryOptionProvider>(OptionLoaderKeys.CountryKey)
                        .RegisterBlockOptionProvider<TimezoneOptionProvider>(OptionLoaderKeys.TimezoneKey)
                        .RegisterBlockOptionProvider<OrganizationMemberOptionProvider>(OptionLoaderKeys.OrganizationMemberKey)
                        .RegisterBlockOptionProvider<OrganizationMemberAndCustomerPairOptionProvider>(
                            OptionLoaderKeys.OrganizationMemberAndCustomerPairKey)
                        .RegisterBlockOptionProvider<OrganizationLocationOptionProvider>(OptionLoaderKeys.OrganizationLocationKey)
                        .RegisterBlockOptionProvider<OrganizationTeamOptionProvider>(OptionLoaderKeys.OrganizationTeamKey)
                        .RegisterBlockOptionProvider<OrganizationResourceTypeOptionProvider>(OptionLoaderKeys.OrganizationResourceTypeKey);

                    options
                        .RegisterBlockActionHandler<ButtonAction, DismissSetupPreferredLocationButtonHandler>(
                            LocationActionTypes.DismissSetupPreferredLocation)
                        .RegisterBlockActionHandler<ButtonAction,
                            DismissSetupPreferredZonesButtonHandler>(ZoneActionTypes.DismissSetupPreferredZones);

                    options
                        .RegisterBlockActionHandler<ButtonAction, InstantAddBookingButtonHandler>(BookingActionTypes.InstantAddBooking)
                        .RegisterBlockActionHandler<ButtonAction, JoinBookingButtonHandler>(BookingActionTypes.JoinBooking)
                        .RegisterBlockActionHandler<ButtonAction, CancelBookingButtonHandler>(BookingActionTypes.CancelBooking)
                        .RegisterBlockActionHandler<ButtonAction, AddBookingButtonHandler>(BookingActionTypes.AddBooking)
                        .RegisterViewSubmissionHandler<AddBookingButtonHandler>(BookingCallbackTypes.AddBooking)
                        .RegisterBlockActionHandler<ButtonAction, EditBookingButtonHandler>(BookingActionTypes.EditBooking)
                        .RegisterViewSubmissionHandler<EditBookingButtonHandler>(BookingCallbackTypes.EditBooking);

                    options
                        .RegisterBlockActionHandler<ButtonAction, SendUsFeedbackButtonHandler>(CommonActionTypes.SendUsFeedback)
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
                        .RegisterBlockActionHandler<ButtonAction, AddCustomTagButtonHandler>(CustomTagActionTypes.AddCustomTag)
                        .RegisterViewSubmissionHandler<AddCustomTagButtonHandler>(CustomTagCallbackTypes.AddCustomTag)
                        .RegisterViewSubmissionHandler<EditCustomTagButtonHandler>(CustomTagCallbackTypes.EditCustomTag)
                        .RegisterViewSubmissionHandler<RemoveCustomTagButtonHandler>(CustomTagCallbackTypes.RemoveCustomTag);

                    options
                        .RegisterBlockActionHandler<ButtonAction, AddZoneButtonHandler>(ZoneActionTypes.AddZone)
                        .RegisterViewSubmissionHandler<AddZoneButtonHandler>(ZoneCallbackTypes.AddZone)
                        .RegisterViewSubmissionHandler<EditZoneButtonHandler>(ZoneCallbackTypes.EditZone)
                        .RegisterViewSubmissionHandler<RemoveZoneButtonHandler>(ZoneCallbackTypes.RemoveZone);

                    options
                        .RegisterBlockActionHandler<ButtonAction, AddResourceButtonHandler>(ResourceActionTypes.AddResource)
                        .RegisterViewSubmissionHandler<AddResourceButtonHandler>(ResourceCallbackTypes.AddResource)
                        .RegisterViewSubmissionHandler<EditResourceButtonHandler>(ResourceCallbackTypes.EditResource)
                        .RegisterViewSubmissionHandler<RemoveResourceButtonHandler>(ResourceCallbackTypes.RemoveResource);

                    Enumerable.Range(0, 31).ForEach(idx => options
                        .RegisterBlockActionHandler<ButtonAction, InstantAddBookingButtonHandler>($"{BookingActionTypes.InstantAddBooking}{idx}"));

                    Enumerable.Range(0, 31).ForEach(idx => options
                        .RegisterBlockActionHandler<ButtonAction, CancelBookingButtonHandler>($"{BookingActionTypes.CancelBooking}{idx}"));
                })
            .AddGrpcServices(configuration)
            .AddPages();

        services.AddGrpc();

        var app = builder
            .Build()
            .UseWebApplicationDefaults<Program>()
            .UseSso()
            .UseSecurity();

        app.MapGrpcService<SlackGrpcService>();
        app.UseSlackNet(c => c.MapToPrefix("v1/slack"));

        return app;
    }
}
