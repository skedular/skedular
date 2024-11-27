using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
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
using OrderDirection = Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrderDirection;
using OrganizationService = Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrganizationService;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IDeskTypesPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class DeskTypesPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    OrganizationConfiguration organizationConfiguration,
    CustomerConfiguration customerConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    CustomerService.CustomerServiceClient customerServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingService bookingService,
    IOrganizationService organizationService,
    IDeskTypeComponents deskTypeComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    IBookingsPageContextService bookingsPageContextService) :
    IDeskTypesPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>
{
    private const int DeskTypesPageSize = 5;
    private const string DeskTypesCallback = "DeskTypes";
    private const string FirstPageDeskTypes = "DeskTypes_FirstPageDeskTypes";
    private const string PreviousPageDeskTypes = "DeskTypes_PreviousPageDeskTypes";
    private const string NextPageDeskTypes = "DeskTypes_NextPageDeskTypes";
    private const string LastPageDeskTypes = "DeskTypes_LastPageDeskTypes";

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
            case FirstPageDeskTypes:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageDeskTypes:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageDeskTypes:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageDeskTypes:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case DeskTypeActionTypes.SetPreferredDeskType:
                await AddPreferredDeskTypeAsync(
                    workspace,
                    workspaceMember,
                    SetPreferredDeskTypeContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case DeskTypeActionTypes.RemovePreferredDeskType:
                await RemovePreferredDeskTypeAsync(
                    workspace,
                    workspaceMember,
                    RemovePreferredDeskTypeContext.Deserialize(action.Value),
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
        else if (action.SelectedOption.Value.StartsWith(DeskTypeActionTypes.EditDeskType))
        {
            var context = EditDeskTypeContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.DeskTypesPage);

            var deskTypeId = action.SelectedOption.Value[DeskTypeActionTypes.EditDeskType.Length..];
            var permissions = await organizationService.GetPermissionsAsync(
                workspace,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanModify)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.DeskTypeId = deskTypeId;

            await OpenEditDeskTypeDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(DeskTypeActionTypes.RemoveDeskType))
        {
            var context = RemoveDeskTypeContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.DeskTypesPage);

            var deskTypeId = action.SelectedOption.Value[DeskTypeActionTypes.RemoveDeskType.Length..];
            var permissions = await organizationService.GetPermissionsAsync(
                workspace,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.DeskTypeId = deskTypeId;

            await OpenRemoveDeskTypeDialogAsync(
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        if (commonPageContext.PageContext.DeskTypesPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.DeskTypesPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.DeskTypesPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.DeskTypesPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.DeskTypesPage.Pagination.CurrentLast,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            DeskTypesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.DeskTypesPage.Pagination.Before,
            DeskTypesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.DeskTypesPage.Pagination.After,
            DeskTypesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            DeskTypesPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);

        commonPageContext.PageContext.CurrentPageType = PageType.DeskTypes;

        var deskTypeConnection = await GetPaginatedDeskTypesAsync(
            workspace,
            workspaceMember,
            after,
            first,
            before,
            last,
            commonPageContext,
            cancellationToken);
        var deskTypes = deskTypeConnection.Edges.Select(item => mapper.MapToOrganizationDeskType(item.Node)).ToList();
        var asyncBlocks = await Task.WhenAll(GetToolbarAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), deskTypeComponents.GetDeskTypeCardsAsync(
            workspace,
            workspaceMember,
            deskTypes,
            commonPageContext.PageContext,
            cancellationToken));

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetDeskTypesSearchCriteriaAndPaginationBlocks(deskTypeConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.PublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = DeskTypesCallback,
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
            .RegisterBlockActionHandler<StaticSelectAction, DeskTypesPage>(DeskTypeActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(FirstPageDeskTypes)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(LastPageDeskTypes)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(NextPageDeskTypes)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(PreviousPageDeskTypes)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(DeskTypeActionTypes.SetPreferredDeskType)
            .RegisterBlockActionHandler<ButtonAction, DeskTypesPage>(DeskTypeActionTypes.RemovePreferredDeskType);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Desk Types*".ToMarkdown() }
    ];

    private async Task<ICollection<Block>> GetToolbarAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext);
        var addDeskTypeButton =
            await deskTypeComponents.GetAddDeskTypeButtonAsync(workspace, workspaceMember, pageContext,
                cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addDeskTypeButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private async Task<DeskTypeConnection> GetPaginatedDeskTypesAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DeskTypesPage);
        var getPaginatedDeskTypesInput = new GetPaginatedDeskTypesInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new DeskTypeWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedDeskTypesInput.OrderBy.AddRange([
            new DeskTypeOrderInput { Direction = OrderDirection.Ascending, Field = DeskTypeOrderField.DeskTypeName }
        ]);

        return await organizationServiceClient.GetPaginatedDeskTypesAsync(
            getPaginatedDeskTypesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static List<Block> GetDeskTypesSearchCriteriaAndPaginationBlocks(
        DeskTypeConnection deskTypeConnection,
        PageContext pageContext)
    {
        if (deskTypeConnection.Edges.Count == 0)
        {
            return [new SectionBlock { Text = "No desk type found".ToMarkdown() }];
        }

        var totalDeskTypesCount =
            new SectionBlock { Text = $"Total desk types: {deskTypeConnection.TotalCount}".ToMarkdown() };
        if (deskTypeConnection.TotalCount <= DeskTypesPageSize)
        {
            return [totalDeskTypesCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.DeskTypesPage);

        var paginationButtons = new List<IActionElement>();
        if (deskTypeConnection.PageInfo.HasPreviousPage)
        {
            pageContext.DeskTypesPage.Pagination.First = DeskTypesPageSize;
            pageContext.DeskTypesPage.Pagination.After = null;
            pageContext.DeskTypesPage.Pagination.Before = null;
            pageContext.DeskTypesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageDeskTypes,
                Text = Icons.FirstPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.DeskTypesPage.Pagination.First = null;
            pageContext.DeskTypesPage.Pagination.After = null;
            pageContext.DeskTypesPage.Pagination.Before = deskTypeConnection.PageInfo.StartCursor;
            pageContext.DeskTypesPage.Pagination.Last = DeskTypesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageDeskTypes,
                Text = Icons.PreviousPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (deskTypeConnection.PageInfo.HasNextPage)
        {
            pageContext.DeskTypesPage.Pagination.First = DeskTypesPageSize;
            pageContext.DeskTypesPage.Pagination.After = deskTypeConnection.PageInfo.EndCursor;
            pageContext.DeskTypesPage.Pagination.Before = null;
            pageContext.DeskTypesPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageDeskTypes,
                Text = Icons.NextPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.DeskTypesPage.Pagination.First = null;
            pageContext.DeskTypesPage.Pagination.After = null;
            pageContext.DeskTypesPage.Pagination.Before = null;
            pageContext.DeskTypesPage.Pagination.Last = DeskTypesPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageDeskTypes,
                Text = Icons.LastPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalDeskTypesCount, paginationActionBlock];
    }

    private async Task OpenEditDeskTypeDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditDeskTypeContext context,
        CancellationToken cancellationToken)
    {
        var deskType = await organizationServiceClient.GetDeskTypeAsync(
            new GetDeskTypeInput { Id = context.DeskTypeId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var name = new InputBlock
        {
            BlockId = DeskTypeActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = DeskTypeActionTypes.Name, InitialValue = deskType.Name.ToSafeString()
            },
            Optional = false
        };

        var description = new InputBlock
        {
            BlockId = DeskTypeActionTypes.Description,
            Label = "Description".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = DeskTypeActionTypes.Description,
                InitialValue = deskType.Description.ToSafeString(),
                Multiline = true
            },
            Optional = true
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = DeskTypeCallbackTypes.EditDeskType,
                Title = "Edit Desk Type",
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

    private async Task OpenRemoveDeskTypeDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveDeskTypeContext context,
        CancellationToken cancellationToken)
    {
        var deskType = await organizationServiceClient.GetDeskTypeAsync(
            new GetDeskTypeInput { Id = context.DeskTypeId },
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var confirmationMessage = new SectionBlock
        {
            Text = $"Are you sure you want to remove the desk type {deskType.Name.ToSafeString()}?"
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Open(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = DeskTypeCallbackTypes.RemoveDeskType,
                Title = "Remove Desk Type",
                Close = "No",
                Submit = "Yes",
                Blocks =
                    [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddPreferredDeskTypeAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredDeskTypeContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.AddPreferredOrganizationTagAsync(
            new AddPreferredOrganizationTagInput { OrganizationTagId = context.DeskTypeId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task RemovePreferredDeskTypeAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        RemovePreferredDeskTypeContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.RemovePreferredOrganizationTagAsync(
            new RemovePreferredOrganizationTagInput { OrganizationTagId = context.DeskTypeId },
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
