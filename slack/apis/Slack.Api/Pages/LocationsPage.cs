using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using Button = SlackNet.Blocks.Button;
using LocationEdge = Slack.Shared.Models.LocationEdge;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface ILocationsPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class LocationsPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ICommonComponents commonComponents,
    IBookingsPage bookingsPage,
    IZonesPage zonesPage,
    ICustomTagsPage customTagsPage,
    IResourcesPage resourcesPage,
    ILocationComponents locationComponents,
    ILocationPermissionsService locationPermissionsService,
    IBookingPermissionsService bookingPermissionsService,
    IResourcesPageContextService resourcesPageContextService,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService,
    ICustomerService customerService,
    ILocationService locationService) :
    ILocationsPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<ButtonAction>,
    IBlockActionHandler<StaticSelectAction>
{
    private const int LocationsPageSize = 5;
    private const string LocationsCallback = "Locations";
    private const string FirstPageLocations = "Locations_FirstPageLocations";
    private const string PreviousPageLocations = "Locations_PreviousPageLocations";
    private const string NextPageLocations = "Locations_NextPageLocations";
    private const string LastPageLocations = "Locations_LastPageLocations";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case FirstPageLocations:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageLocations:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageLocations:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageLocations:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LocationActionTypes.AddAsPreferredLocation:
                await AddAsPreferredLocationAsync(
                    workspace,
                    workspaceMember,
                    AddAsPreferredLocationContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LocationActionTypes.RemovePreferredLocation:
                await RemovePreferredLocationAsync(
                    workspace,
                    workspaceMember,
                    ClearPreferredLocationContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(
        StaticSelectAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        if (action.SelectedOption.Value.StartsWith(BookingActionTypes.Bookings))
        {
            var locationId = action.SelectedOption.Value[BookingActionTypes.Bookings.Length..];
            var bookingPermissions =
                await bookingPermissionsService.GetOrganizationPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
            if (!bookingPermissions.CanViewBookings)
            {
                throw new UnauthorizedAccessException();
            }

            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.BookingsPage = bookingsPageContextService.GetDefaultBookingsPageContext();
            context.PageContext.BookingsPage.LocationIds = [locationId];
            context.PageContext.PushCurrentPageToVisitedPages();

            await bookingsPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(LocationActionTypes.EditLocation))
        {
            var locationId = action.SelectedOption.Value[LocationActionTypes.EditLocation.Length..];
            var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, locationId, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new UnauthorizedAccessException();
            }

            var context = EditLocationContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = locationId;

            await OpenEditLocationDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(LocationActionTypes.RemoveLocation))
        {
            var locationId = action.SelectedOption.Value[LocationActionTypes.RemoveLocation.Length..];
            var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, locationId, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new UnauthorizedAccessException();
            }

            var context = RemoveLocationContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = locationId;

            await OpenRemoveLocationDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ZoneActionTypes.Zones))
        {
            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.ZonesPage = new Shared.Context.ZonesPage(new PaginationContext());
            context.PageContext.PushCurrentPageToVisitedPages();

            await zonesPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(CustomTagActionTypes.CustomTags))
        {
            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.CustomTagsPage = new Shared.Context.CustomTagsPage(new PaginationContext());
            context.PageContext.PushCurrentPageToVisitedPages();

            await customTagsPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ResourceActionTypes.Resources))
        {
            var locationId = action.SelectedOption.Value[ResourceActionTypes.Resources.Length..];
            var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
            context.PageContext.ResourcesPage = resourcesPageContextService.GetDefaultDesksPageContext(locationId);
            context.PageContext.PushCurrentPageToVisitedPages();

            await resourcesPage.RenderWithContextAsync(
                workspace,
                workspaceMember,
                new CommonPageContext(context.PageContext),
                request.View.Hash,
                cancellationToken);
        }
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.ButtonActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.StaticSelectActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);
        if (commonPageContext.PageContext.LocationsPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.LocationsPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.LocationsPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.LocationsPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.LocationsPage.Pagination.CurrentLast,
                commonPageContext,
                hash,
                cancellationToken);
        }
    }

    private async Task RenderFirstPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            LocationsPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderPreviousPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.LocationsPage.Pagination.Before,
            LocationsPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderNextPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.LocationsPage.Pagination.After,
            LocationsPageSize,
            null,
            null,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderLastPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            LocationsPageSize,
            commonPageContext,
            hash,
            cancellationToken);
    }

    private async Task RenderInternalAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.LocationsPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Locations;

        var connection = await locationService.GetPaginatedLocationsAsync(
            workspaceMember.Id,
            workspace.Organization.Id,
            null,
            after,
            first,
            before,
            last,
            cancellationToken);

        var locations = connection.Edges.Select(item => item.Node).ToList();
        var locationIds = locations.Select(item => item.Id).ToList();
        var locationsWithChannel = await repositoryFactory.LocationRepository
            .GetActiveByIdsAsync(locationIds, cancellationToken);
        locations = locations.Select(item =>
        {
            var matchedLocation =
                locationsWithChannel.FirstOrDefault(replicatedLocation => replicatedLocation.Id == item.Id);
            if (matchedLocation is not null)
            {
                item.DailyUpdateChannel = mapper.MapTo(matchedLocation.DailyUpdateChannel);
            }

            return item;
        }).ToList();

        var asyncBlocks = await Task.WhenAll(
            GetToolbarAsync(workspace, workspaceMember, commonPageContext.PageContext, cancellationToken),
            locationComponents.GetLocationCardsAsync(
                workspaceMember,
                locations,
                commonPageContext.PageContext,
                cancellationToken));

        IReadOnlyList<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetLocationsSearchCriteriaAndPaginationBlocks(connection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = LocationsCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, LocationsPage>(LocationActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(FirstPageLocations)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(LastPageLocations)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(NextPageLocations)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(PreviousPageLocations)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(LocationActionTypes.AddAsPreferredLocation)
            .RegisterBlockActionHandler<ButtonAction, LocationsPage>(LocationActionTypes.RemovePreferredLocation);

    private static IReadOnlyList<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Locations*".ToMarkdown() }
    ];

    private async Task<IReadOnlyList<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var addLocationButton = await locationComponents.GetAddLocationButtonAsync(workspace, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addLocationButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private static List<Block> GetLocationsSearchCriteriaAndPaginationBlocks(Connection<LocationEdge> locationConnection, PageContext pageContext)
    {
        if (!locationConnection.Edges.Any())
        {
            return [new SectionBlock { Text = "No location found".ToMarkdown() }];
        }

        var totalLocationsCount =
            new SectionBlock { Text = $"Total locations: {locationConnection.TotalCount}".ToMarkdown() };
        if (locationConnection.TotalCount <= LocationsPageSize)
        {
            return [totalLocationsCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.LocationsPage);

        var paginationButtons = new List<IActionElement>();
        if (locationConnection.PageInfo.HasPreviousPage)
        {
            pageContext.LocationsPage.Pagination.First = LocationsPageSize;
            pageContext.LocationsPage.Pagination.After = null;
            pageContext.LocationsPage.Pagination.Before = null;
            pageContext.LocationsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageLocations, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.LocationsPage.Pagination.First = null;
            pageContext.LocationsPage.Pagination.After = null;
            pageContext.LocationsPage.Pagination.Before = locationConnection.PageInfo.StartCursor;
            pageContext.LocationsPage.Pagination.Last = LocationsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageLocations, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (locationConnection.PageInfo.HasNextPage)
        {
            pageContext.LocationsPage.Pagination.First = LocationsPageSize;
            pageContext.LocationsPage.Pagination.After = locationConnection.PageInfo.EndCursor;
            pageContext.LocationsPage.Pagination.Before = null;
            pageContext.LocationsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageLocations, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.LocationsPage.Pagination.First = null;
            pageContext.LocationsPage.Pagination.After = null;
            pageContext.LocationsPage.Pagination.Before = null;
            pageContext.LocationsPage.Pagination.Last = LocationsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageLocations, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalLocationsCount, paginationActionBlock];
    }

    private async Task OpenEditLocationDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditLocationContext context,
        CancellationToken cancellationToken)
    {
        var location = await locationService.GetAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        var name = new InputBlock
        {
            BlockId = LocationActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = LocationActionTypes.Name, InitialValue = location.Name.ToSafeString() },
            Optional = false
        };

        var about = new InputBlock
        {
            BlockId = LocationActionTypes.About,
            Label = "About".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = LocationActionTypes.About, InitialValue = location.ListingMetadata.About.ToSafeString(), Multiline = true
            },
            Optional = true
        };

        var timezone = new InputBlock
        {
            BlockId = OptionLoaderKeys.TimezoneKey,
            Label = "Timezone".ToPlainText(),
            Element = new ExternalSelectMenu
            {
                ActionId = OptionLoaderKeys.TimezoneKey,
                InitialOption = string.IsNullOrWhiteSpace(location.Timezone)
                    ? null
                    : new Option { Text = location.Timezone.ToOptionText(), Value = location.Timezone },
                MinQueryLength = 3
            },
            Optional = false
        };

        var locationEntity = await repositoryFactory.LocationRepository.GetByIdAsync(location.Id, cancellationToken);

        var updateChannel = new InputBlock
        {
            BlockId = LocationActionTypes.SlackUpdateChannel,
            Label = "Slack update channel".ToPlainText(),
            Element = new ChannelSelectMenu
            {
                ActionId = LocationActionTypes.SlackUpdateChannel, InitialChannel = locationEntity?.DailyUpdateChannel?.Id
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = LocationCallbackTypes.EditLocation,
                Title = "Edit Location",
                Close = "Cancel",
                Submit = "Save",
                Blocks = [name, about, timezone, updateChannel],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveLocationDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveLocationContext context,
        CancellationToken cancellationToken)
    {
        var location = await locationService.GetAsync(workspaceMember.Id, context.LocationId, cancellationToken);
        var confirmationMessage = new SectionBlock { Text = $"Are you sure you want to remove the location {location.Name.ToSafeString()}?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = LocationCallbackTypes.RemoveLocation,
                Title = "Remove Location",
                Close = "No",
                Submit = "Yes",
                Blocks = [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddAsPreferredLocationAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        AddAsPreferredLocationContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerService.AddPreferredLocationAsync(workspaceMember.Id, context.LocationId, cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task RemovePreferredLocationAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ClearPreferredLocationContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerService.RemovePreferredLocationAsync(workspaceMember.Id, context.LocationId, cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }
}
