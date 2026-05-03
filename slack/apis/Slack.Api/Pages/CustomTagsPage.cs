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

public interface ICustomTagsPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class CustomTagsPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingPermissionsService bookingPermissionsService,
    IOrganizationPermissionsService organizationPermissionsService,
    ICustomTagComponents customTagComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService,
    ICustomerService customerService,
    IOrganizationCustomTagService organizationCustomTagService) :
    ICustomTagsPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>
{
    private const int CustomTagsPageSize = 5;
    private const string CustomTagsCallback = "CustomTags";
    private const string FirstPageCustomTags = "CustomTags_FirstPageCustomTags";
    private const string PreviousPageCustomTags = "CustomTags_PreviousPageCustomTags";
    private const string NextPageCustomTags = "CustomTags_NextPageCustomTags";
    private const string LastPageCustomTags = "CustomTags_LastPageCustomTags";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
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

        switch (action.ActionId)
        {
            case FirstPageCustomTags:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageCustomTags:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageCustomTags:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageCustomTags:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case CustomTagActionTypes.SetPreferredCustomTag:
                await AddPreferredCustomTagAsync(
                    workspace,
                    workspaceMember,
                    SetPreferredCustomTagContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case CustomTagActionTypes.RemovePreferredCustomTag:
                await RemovePreferredCustomTagAsync(
                    workspace,
                    workspaceMember,
                    RemovePreferredCustomTagContext.Deserialize(action.Value),
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
        else if (action.SelectedOption.Value.StartsWith(CustomTagActionTypes.EditCustomTag))
        {
            var context = EditCustomTagContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.CustomTagsPage);

            var customTagId = action.SelectedOption.Value[CustomTagActionTypes.EditCustomTag.Length..];
            var permissions =
                await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
            if (!permissions.CanModify)
            {
                throw new UnauthorizedAccessException();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.CustomTagId = customTagId;

            await OpenEditCustomTagDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(CustomTagActionTypes.RemoveCustomTag))
        {
            var context = RemoveCustomTagContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.CustomTagsPage);

            var customTagId = action.SelectedOption.Value[CustomTagActionTypes.RemoveCustomTag.Length..];
            var permissions =
                await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new UnauthorizedAccessException();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.CustomTagId = customTagId;

            await OpenRemoveCustomTagDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);
        if (commonPageContext.PageContext.CustomTagsPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.CustomTagsPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.CustomTagsPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.CustomTagsPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.CustomTagsPage.Pagination.CurrentLast,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            CustomTagsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.CustomTagsPage.Pagination.Before,
            CustomTagsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.CustomTagsPage.Pagination.After,
            CustomTagsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            CustomTagsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.CustomTagsPage);

        commonPageContext.PageContext.CurrentPageType = PageType.CustomTags;

        var customTagConnection = await organizationCustomTagService.GetPaginatedCustomTagsAsync(
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
            cancellationToken), customTagComponents.GetCustomTagCardsAsync(
            workspace,
            workspaceMember,
            customTagConnection.Edges.Select(item => item.Node).ToList(),
            commonPageContext.PageContext,
            cancellationToken));

        IReadOnlyList<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetCustomTagsSearchCriteriaAndPaginationBlocks(customTagConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = CustomTagsCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<StaticSelectAction, CustomTagsPage>(CustomTagActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(FirstPageCustomTags)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(LastPageCustomTags)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(NextPageCustomTags)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(PreviousPageCustomTags)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(CustomTagActionTypes.SetPreferredCustomTag)
            .RegisterBlockActionHandler<ButtonAction, CustomTagsPage>(CustomTagActionTypes.RemovePreferredCustomTag);

    private static IReadOnlyList<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Tags*".ToMarkdown() }
    ];

    private async Task<IReadOnlyList<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, workspaceMember.Timezone);
        var addCustomTagButton =
            await customTagComponents.GetAddCustomTagButtonAsync(workspace, workspaceMember, pageContext,
                cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addCustomTagButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private static List<Block> GetCustomTagsSearchCriteriaAndPaginationBlocks(
        Connection<OrganizationCustomTagEdge> customTagConnection,
        PageContext pageContext)
    {
        if (customTagConnection.Edges.Any())
        {
            return [new SectionBlock { Text = "No tag found".ToMarkdown() }];
        }

        var totalCustomTagsCount =
            new SectionBlock { Text = $"Total tags: {customTagConnection.TotalCount}".ToMarkdown() };
        if (customTagConnection.TotalCount <= CustomTagsPageSize)
        {
            return [totalCustomTagsCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.CustomTagsPage);

        var paginationButtons = new List<IActionElement>();
        if (customTagConnection.PageInfo.HasPreviousPage)
        {
            pageContext.CustomTagsPage.Pagination.First = CustomTagsPageSize;
            pageContext.CustomTagsPage.Pagination.After = null;
            pageContext.CustomTagsPage.Pagination.Before = null;
            pageContext.CustomTagsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageCustomTags, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.CustomTagsPage.Pagination.First = null;
            pageContext.CustomTagsPage.Pagination.After = null;
            pageContext.CustomTagsPage.Pagination.Before = customTagConnection.PageInfo.StartCursor;
            pageContext.CustomTagsPage.Pagination.Last = CustomTagsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageCustomTags, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (customTagConnection.PageInfo.HasNextPage)
        {
            pageContext.CustomTagsPage.Pagination.First = CustomTagsPageSize;
            pageContext.CustomTagsPage.Pagination.After = customTagConnection.PageInfo.EndCursor;
            pageContext.CustomTagsPage.Pagination.Before = null;
            pageContext.CustomTagsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageCustomTags, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.CustomTagsPage.Pagination.First = null;
            pageContext.CustomTagsPage.Pagination.After = null;
            pageContext.CustomTagsPage.Pagination.Before = null;
            pageContext.CustomTagsPage.Pagination.Last = CustomTagsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageCustomTags, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalCustomTagsCount, paginationActionBlock];
    }

    private async Task OpenEditCustomTagDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditCustomTagContext context,
        CancellationToken cancellationToken)
    {
        var customTag = await organizationCustomTagService.GetAsync(workspaceMember.Id, context.CustomTagId, cancellationToken);
        var name = new InputBlock
        {
            BlockId = CustomTagActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput { ActionId = CustomTagActionTypes.Name, InitialValue = customTag.Name.ToSafeString() },
            Optional = false
        };

        var description = new InputBlock
        {
            BlockId = CustomTagActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = CustomTagActionTypes.Description, InitialValue = customTag.Description.ToSafeString(), Multiline = true
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = CustomTagCallbackTypes.EditCustomTag,
                Title = "Edit Tag",
                Close = "Cancel",
                Submit = "Save",
                Blocks =
                [
                    name, description
                ],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveCustomTagDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveCustomTagContext context,
        CancellationToken cancellationToken)
    {
        var customTag = await organizationCustomTagService.GetAsync(workspaceMember.Id, context.CustomTagId, cancellationToken);
        var confirmationMessage = new SectionBlock { Text = $"Are you sure you want to remove the tag {customTag.Name.ToSafeString()}?" };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = CustomTagCallbackTypes.RemoveCustomTag,
                Title = "Remove Tag",
                Close = "No",
                Submit = "Yes",
                Blocks = [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddPreferredCustomTagAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredCustomTagContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerService.AddPreferredOrganizationTagAsync(workspaceMember.Id, context.CustomTagId, cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task RemovePreferredCustomTagAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        RemovePreferredCustomTagContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerService.RemovePreferredOrganizationTagAsync(workspaceMember.Id, context.CustomTagId, cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }
}
