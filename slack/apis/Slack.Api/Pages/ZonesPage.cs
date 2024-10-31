using Api.Shared.Models;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Slack.Api.Components;
using Slack.Api.Mappers;
using Slack.Api.Services;
using Slack.Shared;
using Slack.Shared.Configurations;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Repositories;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Button = SlackNet.Blocks.Button;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;
using Icons = Slack.Shared.Constants.Icons;
using LocationService = Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationService;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IZonesPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class ZonesPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    LocationConfiguration locationConfiguration,
    CustomerConfiguration customerConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    CustomerService.CustomerServiceClient customerServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingService bookingService,
    ILocationService locationService,
    IZoneComponents zoneComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService) :
    IZonesPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>
{
    private const int ZonesPageSize = 5;
    private const string ZonesCallback = "Zones";
    private const string FirstPageZones = "Zones_FirstPageZones";
    private const string PreviousPageZones = "Zones_PreviousPageZones";
    private const string NextPageZones = "Zones_NextPageZones";
    private const string LastPageZones = "Zones_LastPageZones";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) =
            await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                workspaceEntity,
                request.User.Id,
                cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case FirstPageZones:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageZones:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageZones:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageZones:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case ZoneActionTypes.SetPreferredZone:
                await AddPreferredZoneAsync(
                    workspace,
                    workspaceMember,
                    SetPreferredZoneContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case ZoneActionTypes.RemovePreferredZone:
                await RemovePreferredZoneAsync(
                    workspace,
                    workspaceMember,
                    RemovePreferredZoneContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(StaticSelectAction action, BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

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
                await bookingService.GetLocationPermissionsAsync(locationId, workspaceMember, cancellationToken);
            if (!bookingPermissions.CanViewBookings)
            {
                throw new Unauthorized();
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
        else if (action.SelectedOption.Value.StartsWith(ZoneActionTypes.EditZone))
        {
            var context = EditZoneContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ZonesPage);

            var zoneId = action.SelectedOption.Value[ZoneActionTypes.EditZone.Length..];
            var permissions = await locationService.GetPermissionsAsync(
                context.PageContext.ZonesPage.LocationId,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanModify)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.ZonesPage.LocationId;
            context.ZoneId = zoneId;

            await OpenEditZoneDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ZoneActionTypes.RemoveZone))
        {
            var context = RemoveZoneContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ZonesPage);

            var zoneId = action.SelectedOption.Value[ZoneActionTypes.RemoveZone.Length..];
            var permissions = await locationService.GetPermissionsAsync(
                context.PageContext.ZonesPage.LocationId,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.ZonesPage.LocationId;
            context.ZoneId = zoneId;

            await OpenRemoveZoneDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
    }

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
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
        if (slackConfiguration.EnableAsyncMode)
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        if (commonPageContext.PageContext.ZonesPage.ZonesPagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.ZonesPage.ZonesPagination.CurrentAfter,
                commonPageContext.PageContext.ZonesPage.ZonesPagination.CurrentFirst,
                commonPageContext.PageContext.ZonesPage.ZonesPagination.CurrentBefore,
                commonPageContext.PageContext.ZonesPage.ZonesPagination.CurrentLast,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            ZonesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.ZonesPage.ZonesPagination.Before,
            ZonesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.ZonesPage.ZonesPagination.After,
            ZonesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            ZonesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Zones;

        var zoneConnection = await GetPaginatedZonesAsync(
            workspaceMember,
            after,
            first,
            before,
            last,
            commonPageContext,
            cancellationToken);
        var zones = zoneConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var asyncBlocks = await Task.WhenAll(GetToolbarAsync(
            commonPageContext.PageContext.ZonesPage.LocationId,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), zoneComponents.GetZoneCardsAsync(
            commonPageContext.PageContext.ZonesPage.LocationId,
            workspaceMember,
            zones,
            commonPageContext.PageContext,
            cancellationToken));

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetZonesSearchCriteriaAndPaginationBlocks(zoneConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.PublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = ZonesCallback,
                Blocks = blocks
                    .SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()]))
                    .SkipLast(1)
                    .ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, ZonesPage>(ZoneActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(FirstPageZones)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(LastPageZones)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(NextPageZones)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(PreviousPageZones)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(ZoneActionTypes.SetPreferredZone)
            .RegisterBlockActionHandler<ButtonAction, ZonesPage>(ZoneActionTypes.RemovePreferredZone);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Zones*".ToMarkdown() }
    ];

    private async Task<ICollection<Block>> GetToolbarAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext);
        var addZoneButton =
            await zoneComponents.GetAddZoneButtonAsync(locationId, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addZoneButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private async Task<TagConnection> GetPaginatedZonesAsync(
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        var getPaginatedTagsInput = new GetPaginatedTagsInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new TagWhereInput
            {
                LocationId = commonPageContext.PageContext.ZonesPage.LocationId, Type = LocationTagType.Zone
            }
        };

        getPaginatedTagsInput.OrderBy.AddRange([
            new TagOrderInput { Direction = OrderDirection.Ascending, Field = TagOrderField.TagName }
        ]);

        return await locationServiceClient.GetPaginatedTagsAsync(
            getPaginatedTagsInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static List<Block> GetZonesSearchCriteriaAndPaginationBlocks(
        TagConnection tagConnection,
        PageContext pageContext)
    {
        if (tagConnection.Edges.Count == 0)
        {
            return [new SectionBlock { Text = "No zone found".ToMarkdown() }];
        }

        var totalZonesCount =
            new SectionBlock { Text = $"Total zones: {tagConnection.TotalCount}".ToMarkdown() };
        if (tagConnection.TotalCount <= ZonesPageSize)
        {
            return [totalZonesCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.ZonesPage);

        var paginationButtons = new List<IActionElement>();
        if (tagConnection.PageInfo.HasPreviousPage)
        {
            pageContext.ZonesPage.ZonesPagination.First = ZonesPageSize;
            pageContext.ZonesPage.ZonesPagination.After = null;
            pageContext.ZonesPage.ZonesPagination.Before = null;
            pageContext.ZonesPage.ZonesPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageZones,
                Text = Icons.FirstPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ZonesPage.ZonesPagination.First = null;
            pageContext.ZonesPage.ZonesPagination.After = null;
            pageContext.ZonesPage.ZonesPagination.Before = tagConnection.PageInfo.StartCursor;
            pageContext.ZonesPage.ZonesPagination.Last = ZonesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageZones,
                Text = Icons.PreviousPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (tagConnection.PageInfo.HasNextPage)
        {
            pageContext.ZonesPage.ZonesPagination.First = ZonesPageSize;
            pageContext.ZonesPage.ZonesPagination.After = tagConnection.PageInfo.EndCursor;
            pageContext.ZonesPage.ZonesPagination.Before = null;
            pageContext.ZonesPage.ZonesPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageZones,
                Text = Icons.NextPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ZonesPage.ZonesPagination.First = null;
            pageContext.ZonesPage.ZonesPagination.After = null;
            pageContext.ZonesPage.ZonesPagination.Before = null;
            pageContext.ZonesPage.ZonesPagination.Last = ZonesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageZones,
                Text = Icons.LastPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalZonesCount, paginationActionBlock];
    }

    private async Task OpenEditZoneDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditZoneContext context,
        CancellationToken cancellationToken)
    {
        var zone = await locationServiceClient.GetTagAsync(
            new GetTagInput { Id = context.ZoneId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var name = new InputBlock
        {
            BlockId = ZoneActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = ZoneActionTypes.Name, InitialValue = zone.Name.ToSafeString()
            },
            Optional = false
        };

        var description = new InputBlock
        {
            BlockId = ZoneActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = ZoneActionTypes.Description,
                InitialValue = zone.Description.ToSafeString(),
                Multiline = true
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ZoneCallbackTypes.EditZone,
                Title = "Edit Zone",
                Close = "Cancel",
                Submit = "Save",
                Blocks =
                [
                    name, description
                ],
                PrivateMetadata = context.Serialize()
            });
    }

    private async Task OpenRemoveZoneDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveZoneContext context,
        CancellationToken cancellationToken)
    {
        var zone = await locationServiceClient.GetTagAsync(
            new GetTagInput { Id = context.ZoneId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var confirmationMessage = new SectionBlock
        {
            Text = $"Are you sure you want to remove the zone {zone.Name.ToSafeString()}?"
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ZoneCallbackTypes.RemoveZone,
                Title = "Remove Zone",
                Close = "No",
                Submit = "Yes",
                Blocks =
                    [confirmationMessage],
                PrivateMetadata = context.Serialize()
            });
    }

    private async Task AddPreferredZoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredZoneContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.AddPreferredLocationTagAsync(
            new AddPreferredLocationTagInput { LocationTagId = context.ZoneId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task RemovePreferredZoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        RemovePreferredZoneContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.RemovePreferredLocationTagAsync(
            new RemovePreferredLocationTagInput { LocationTagId = context.ZoneId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }
}
