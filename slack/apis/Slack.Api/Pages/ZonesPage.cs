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
using Slack.Shared.Models;
using Slack.Shared.Repositories;
using Slack.Shared.Services.CrossDomains;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Interaction;
using Button = SlackNet.Blocks.Button;
using Icons = Slack.Shared.Constants.Icons;
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
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingPermissionsService bookingPermissionsService,
    IOrganizationPermissionsService organizationPermissionsService,
    IZoneComponents zoneComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService,
    ICustomerService customerService,
    IOrganizationZoneService organizationZoneService) :
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

    public async Task HandleAsync(StaticSelectAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
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
        else if (action.SelectedOption.Value.StartsWith(ZoneActionTypes.EditZone))
        {
            var context = EditZoneContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ZonesPage);

            var zoneId = action.SelectedOption.Value[ZoneActionTypes.EditZone.Length..];
            var permissions =
                await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new UnauthorizedAccessException();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.ZoneId = zoneId;

            await OpenEditZoneDialogAsync(workspace, workspaceMember, request.TriggerId, context, cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(ZoneActionTypes.RemoveZone))
        {
            var context = RemoveZoneContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.ZonesPage);

            var zoneId = action.SelectedOption.Value[ZoneActionTypes.RemoveZone.Length..];
            var permissions =
                await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new UnauthorizedAccessException();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.ZoneId = zoneId;

            await OpenRemoveZoneDialogAsync(workspace, workspaceMember, request.TriggerId, context, cancellationToken);
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.ZonesPage);
        if (commonPageContext.PageContext.ZonesPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.ZonesPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.ZonesPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.ZonesPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.ZonesPage.Pagination.CurrentLast,
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
            commonPageContext.PageContext.ZonesPage.Pagination.Before,
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
            commonPageContext.PageContext.ZonesPage.Pagination.After,
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

        var zoneConnection = await organizationZoneService.GetPaginatedZonesAsync(
            workspaceMember.Id,
            workspace.Organization.Id,
            null,
            after,
            first,
            before,
            last,
            cancellationToken);
        var asyncBlocks = await Task.WhenAll(GetToolbarAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), zoneComponents.GetZoneCardsAsync(
            workspace,
            workspaceMember,
            zoneConnection.Edges.Select(item => item.Node).ToList(),
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
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = ZonesCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
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
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var addZoneButton = await zoneComponents.GetAddZoneButtonAsync(workspace, workspaceMember, pageContext, cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>().Concat(homeAndBackButtons).Concat(addZoneButton).Concat(feedbackButton).ToList()
            }
        ];
    }

    private static List<Block> GetZonesSearchCriteriaAndPaginationBlocks(Connection<OrganizationZoneEdge> zoneConnection, PageContext pageContext)
    {
        if (!zoneConnection.Edges.Any())
        {
            return [new SectionBlock { Text = "No zone found".ToMarkdown() }];
        }

        var totalZonesCount = new SectionBlock { Text = $"Total zones: {zoneConnection.TotalCount}".ToMarkdown() };
        if (zoneConnection.TotalCount <= ZonesPageSize)
        {
            return [totalZonesCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.ZonesPage);

        var paginationButtons = new List<IActionElement>();
        if (zoneConnection.PageInfo.HasPreviousPage)
        {
            pageContext.ZonesPage.Pagination.First = ZonesPageSize;
            pageContext.ZonesPage.Pagination.After = null;
            pageContext.ZonesPage.Pagination.Before = null;
            pageContext.ZonesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageZones, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ZonesPage.Pagination.First = null;
            pageContext.ZonesPage.Pagination.After = null;
            pageContext.ZonesPage.Pagination.Before = zoneConnection.PageInfo.StartCursor;
            pageContext.ZonesPage.Pagination.Last = ZonesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageZones, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (zoneConnection.PageInfo.HasNextPage)
        {
            pageContext.ZonesPage.Pagination.First = ZonesPageSize;
            pageContext.ZonesPage.Pagination.After = zoneConnection.PageInfo.EndCursor;
            pageContext.ZonesPage.Pagination.Before = null;
            pageContext.ZonesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageZones, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.ZonesPage.Pagination.First = null;
            pageContext.ZonesPage.Pagination.After = null;
            pageContext.ZonesPage.Pagination.Before = null;
            pageContext.ZonesPage.Pagination.Last = ZonesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageZones, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
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
        var zone = await organizationZoneService.GetAsync(workspaceMember.Id, context.ZoneId, cancellationToken);
        var name = new InputBlock
        {
            BlockId = ZoneActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = ZoneActionTypes.Name, InitialValue = zone.Name.ToSafeString() },
            Optional = false
        };

        var description = new InputBlock
        {
            BlockId = ZoneActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = ZoneActionTypes.Description, InitialValue = zone.Description.ToSafeString(), Multiline = true
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ZoneCallbackTypes.EditZone,
                Title = "Edit Zone",
                Close = "Cancel",
                Submit = "Save",
                Blocks = [name, description],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveZoneDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveZoneContext context,
        CancellationToken cancellationToken)
    {
        var zone = await organizationZoneService.GetAsync(workspaceMember.Id, context.ZoneId, cancellationToken);
        var confirmationMessage = new SectionBlock { Text = $"Are you sure you want to remove the zone {zone.Name.ToSafeString()}?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = ZoneCallbackTypes.RemoveZone,
                Title = "Remove Zone",
                Close = "No",
                Submit = "Yes",
                Blocks = [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddPreferredZoneAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredZoneContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerService.AddPreferredOrganizationTagAsync(workspaceMember.Id, context.ZoneId, cancellationToken);

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
        await customerService.RemovePreferredOrganizationTagAsync(workspaceMember.Id, context.ZoneId, cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }
}
