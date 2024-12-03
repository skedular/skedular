using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
using Api.Shared.Services.Grpc.UnityHub.Customer.V1;
using Api.Shared.Services.Grpc.UnityHub.Location.V1;
using Api.Shared.Services.Grpc.UnityHub.Organization.V1;
using Enterprise.Shared;
using Enterprise.Shared.Exceptions;
using Enterprise.Shared.Grpc;
using Enterprise.Shared.Time;
using Google.Protobuf.WellKnownTypes;
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
using OrganizationService = Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrganizationService;
using BookingService = Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingService;
using Button = SlackNet.Blocks.Button;
using CustomerService = Api.Shared.Services.Grpc.UnityHub.Customer.V1.CustomerService;
using Icons = Slack.Shared.Constants.Icons;
using LocationService = Api.Shared.Services.Grpc.UnityHub.Location.V1.LocationService;
using Option = SlackNet.Blocks.Option;
using OrderDirection = Api.Shared.Services.Grpc.UnityHub.Location.V1.OrderDirection;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IDesksPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class DesksPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    LocationConfiguration locationConfiguration,
    LocationService.LocationServiceClient locationServiceClient,
    CustomerConfiguration customerConfiguration,
    CustomerService.CustomerServiceClient customerServiceClient,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    OrganizationConfiguration organizationConfiguration,
    OrganizationService.OrganizationServiceClient organizationServiceClient,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    IBookingsPage bookingsPage,
    IBookingService bookingService,
    ILocationService locationService,
    IDeskComponents deskComponents,
    ICommonComponents commonComponents,
    IMapper mapper,
    TimeProvider timeProvider,
    IBookingsPageContextService bookingsPageContextService) :
    IDesksPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<ButtonAction>,
    IBlockActionHandler<DatePickerAction>
{
    private const int DesksPageSize = 5;
    private const string DesksCallback = "Desks";
    private const string BookingDatePicker = "Desks_BookingDatePicker";
    private const string FirstPageDesks = "Desks_FirstPageDesks";
    private const string PreviousPageDesks = "Desks_PreviousPageDesks";
    private const string NextPageDesks = "Desks_NextPageDesks";
    private const string LastPageDesks = "Desks_LastPageDesks";

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
            case FirstPageDesks:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageDesks:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageDesks:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageDesks:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case DeskActionTypes.SetPreferredDesk:
                await AddPreferredDeskAsync(
                    workspace,
                    workspaceMember,
                    SetPreferredDeskContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case DeskActionTypes.RemovePreferredDesk:
                await RemovePreferredDeskAsync(
                    workspace,
                    workspaceMember,
                    RemovePreferredDeskContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(DatePickerAction action, BlockActionRequest request,
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

        switch (action.ActionId)
        {
            case BookingDatePicker:
                await HandleDatePickerChangedAsync(workspace, workspaceMember, action, request, cancellationToken);

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
        else if (action.SelectedOption.Value.StartsWith(DeskActionTypes.EditDesk))
        {
            var context = EditDeskContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.DesksPage);

            var deskId = action.SelectedOption.Value[DeskActionTypes.EditDesk.Length..];
            var permissions = await locationService.GetPermissionsAsync(
                context.PageContext.DesksPage.LocationId,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanModify)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.DesksPage.LocationId;
            context.DeskId = deskId;

            await OpenEditDeskDialogAsync(
                workspace,
                workspaceMember,
                request.TriggerId,
                context,
                cancellationToken);
        }
        else if (action.SelectedOption.Value.StartsWith(DeskActionTypes.RemoveDesk))
        {
            var context = RemoveDeskContext.Deserialize(request.View.PrivateMetadata);
            ArgumentNullException.ThrowIfNull(context.PageContext.DesksPage);

            var deskId = action.SelectedOption.Value[DeskActionTypes.RemoveDesk.Length..];
            var permissions = await locationService.GetPermissionsAsync(
                context.PageContext.DesksPage.LocationId,
                workspaceMember,
                cancellationToken);
            if (!permissions.CanDelete)
            {
                throw new Unauthorized();
            }

            context.PageContext.PushCurrentPageToVisitedPages();
            context.LocationId = context.PageContext.DesksPage.LocationId;
            context.DeskId = deskId;

            await OpenRemoveDeskDialogAsync(
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

    public async Task Handle(DatePickerAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
        {
            asyncPageRenderingService.DatePickerActionHandlerStream.OnNext((GetType(), action, request));
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        if (commonPageContext.PageContext.DesksPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.DesksPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.DesksPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.DesksPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.DesksPage.Pagination.CurrentLast,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            DesksPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.DesksPage.Pagination.Before,
            DesksPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.DesksPage.Pagination.After,
            DesksPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            DesksPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Desks;

        var deskConnection = await GetPaginatedDesksAsync(
            workspaceMember,
            after,
            first,
            before,
            last,
            commonPageContext,
            cancellationToken);
        var desks = deskConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var bookingConnection =
            await GetBookingsAsync(workspace, workspaceMember, commonPageContext.PageContext, cancellationToken);
        var bookings = bookingConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var asyncBlocks = await Task.WhenAll(GetToolbarAsync(
            commonPageContext.PageContext.DesksPage.LocationId,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), deskComponents.GetDeskCardsAsync(
            commonPageContext.PageContext.DesksPage.LocationId,
            workspaceMember,
            desks,
            bookings,
            commonPageContext.PageContext,
            cancellationToken));

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            asyncBlocks[0],
            GetBookingsDatePicker(commonPageContext.PageContext),
            GetDesksSearchCriteriaAndPaginationBlocks(deskConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = DesksCallback,
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
            .RegisterBlockActionHandler<StaticSelectAction, DesksPage>(DeskActionTypes.ActionsMenu)
            .RegisterBlockActionHandler<DatePickerAction, DesksPage>(BookingDatePicker)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(FirstPageDesks)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(LastPageDesks)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(NextPageDesks)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(PreviousPageDesks)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(DeskActionTypes.SetPreferredDesk)
            .RegisterBlockActionHandler<ButtonAction, DesksPage>(DeskActionTypes.RemovePreferredDesk);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Desks*".ToMarkdown() }
    ];

    private static ICollection<Block> GetBookingsDatePicker(PageContext pageContext)
    {
        ArgumentNullException.ThrowIfNull(pageContext.DesksPage);

        return
        [
            new ActionsBlock
            {
                Elements =
                [
                    new DatePicker
                    {
                        ActionId = BookingDatePicker, InitialDate = pageContext.DesksPage.SelectedDate.ToDateTime()
                    }
                ]
            }
        ];
    }

    private async Task<ICollection<Block>> GetToolbarAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext);
        var addDeskButton =
            await deskComponents.GetAddDeskButtonAsync(locationId, workspaceMember, pageContext, cancellationToken);
        var bulkAddDeskButton =
            await deskComponents.GetBulkAddDesksButtonAsync(locationId, workspaceMember, pageContext,
                cancellationToken);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addDeskButton)
                    .Concat(bulkAddDeskButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private async Task<DeskConnection> GetPaginatedDesksAsync(
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.DesksPage);
        var getPaginatedDesksInput = new GetPaginatedDesksInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new DeskWhereInput { LocationId = commonPageContext.PageContext.DesksPage.LocationId }
        };

        getPaginatedDesksInput.OrderBy.AddRange([
            new DeskOrderInput { Direction = OrderDirection.Ascending, Field = DeskOrderField.DeskName }
        ]);

        return await locationServiceClient.GetPaginatedDesksAsync(
            getPaginatedDesksInput,
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static List<Block> GetDesksSearchCriteriaAndPaginationBlocks(
        DeskConnection deskConnection,
        PageContext pageContext)
    {
        if (deskConnection.Edges.Count == 0)
        {
            return [new SectionBlock { Text = "No desk found".ToMarkdown() }];
        }

        var totalDesksCount =
            new SectionBlock { Text = $"Total desks: {deskConnection.TotalCount}".ToMarkdown() };
        if (deskConnection.TotalCount <= DesksPageSize)
        {
            return [totalDesksCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.DesksPage);

        var paginationButtons = new List<IActionElement>();
        if (deskConnection.PageInfo.HasPreviousPage)
        {
            pageContext.DesksPage.Pagination.First = DesksPageSize;
            pageContext.DesksPage.Pagination.After = null;
            pageContext.DesksPage.Pagination.Before = null;
            pageContext.DesksPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageDesks,
                Text = Icons.FirstPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.DesksPage.Pagination.First = null;
            pageContext.DesksPage.Pagination.After = null;
            pageContext.DesksPage.Pagination.Before = deskConnection.PageInfo.StartCursor;
            pageContext.DesksPage.Pagination.Last = DesksPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageDesks,
                Text = Icons.PreviousPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (deskConnection.PageInfo.HasNextPage)
        {
            pageContext.DesksPage.Pagination.First = DesksPageSize;
            pageContext.DesksPage.Pagination.After = deskConnection.PageInfo.EndCursor;
            pageContext.DesksPage.Pagination.Before = null;
            pageContext.DesksPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageDesks,
                Text = Icons.NextPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.DesksPage.Pagination.First = null;
            pageContext.DesksPage.Pagination.After = null;
            pageContext.DesksPage.Pagination.Before = null;
            pageContext.DesksPage.Pagination.Last = DesksPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageDesks,
                Text = Icons.LastPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return [totalDesksCount, paginationActionBlock];
    }

    private async Task OpenEditDeskDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        EditDeskContext context,
        CancellationToken cancellationToken)
    {
        var desk = await locationServiceClient.GetDeskAsync(
            new GetDeskInput { Id = context.DeskId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var name = new InputBlock
        {
            BlockId = DeskActionTypes.Name,
            Label = "Name".ToPlainText(),
            Element = new PlainTextInput
            {
                ActionId = DeskActionTypes.Name, InitialValue = desk.Name.ToSafeString()
            },
            Optional = false
        };

        var deactivated = new InputBlock
        {
            BlockId = DeskActionTypes.Deactivated,
            Label = "Activation Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = DeskActionTypes.Deactivated,
                    Options = new List<Option>
                    {
                        new() { Text = "Deactivated".ToPlainText(), Value = DeskActionTypes.Deactivated }
                    }
                },
            Optional = true
        };

        var requireBookingApproval = new InputBlock
        {
            BlockId = DeskActionTypes.RequireBookingApproval,
            Label = "Booking Approval Status".ToPlainText(),
            Element =
                new CheckboxGroup
                {
                    ActionId = DeskActionTypes.RequireBookingApproval,
                    Options = new List<Option>
                    {
                        new()
                        {
                            Text = "Require Booking Approval".ToPlainText(),
                            Value = DeskActionTypes.RequireBookingApproval
                        }
                    }
                },
            Optional = true
        };

        var blocks = new List<Block> { name, deactivated, requireBookingApproval };

        var deskTypeConnection = await GetDeskTypesAsync(workspace, workspaceMember, cancellationToken);
        if (deskTypeConnection.Edges.Count != 0)
        {
            blocks.Add(new InputBlock
            {
                BlockId = DeskTypeActionTypes.DeskTypes,
                Label = "Desk Types".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = DeskTypeActionTypes.DeskTypes,
                    Options = deskTypeConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description =
                            string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList(),
                    InitialOptions = deskTypeConnection.Edges.Select(item => item.Node)
                        .Where(item => desk.OrganizationDeskTypes.Select(tag => tag.Id).Contains(item.Id)).Select(item =>
                            new Option
                            {
                                Text = item.Name.ToOptionText(),
                                Value = item.Id,
                                Description =
                                    string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                            }).ToList()
                },
                Optional = true
            });
        }

        var zoneConnection = await GetZonesAsync(workspace, workspaceMember, cancellationToken);
        if (zoneConnection.Edges.Count != 0)
        {
            blocks.Add(new InputBlock
            {
                BlockId = ZoneActionTypes.Zones,
                Label = "Zones".ToPlainText(),
                Element = new StaticMultiSelectMenu
                {
                    ActionId = ZoneActionTypes.Zones,
                    Options = zoneConnection.Edges.Select(item => item.Node).Select(item => new Option
                    {
                        Text = item.Name.ToOptionText(),
                        Value = item.Id,
                        Description =
                            string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                    }).ToList(),
                    InitialOptions = zoneConnection.Edges.Select(item => item.Node)
                        .Where(item => desk.OrganizationZones.Select(tag => tag.Id).Contains(item.Id)).Select(item =>
                            new Option
                            {
                                Text = item.Name.ToOptionText(),
                                Value = item.Id,
                                Description =
                                    string.IsNullOrWhiteSpace(item.Description) ? null : item.Description.ToPlainText()
                            }).ToList()
                },
                Optional = true
            });
        }

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = DeskCallbackTypes.EditDesk,
                Title = "Edit Desk",
                Close = "Cancel",
                Submit = "Save",
                Blocks = blocks,
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task OpenRemoveDeskDialogAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string triggerId,
        RemoveDeskContext context,
        CancellationToken cancellationToken)
    {
        var desk = await locationServiceClient.GetDeskAsync(
            new GetDeskInput { Id = context.DeskId },
            locationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        var confirmationMessage = new SectionBlock
        {
            Text = $"Are you sure you want to remove the desk {desk.Name.ToSafeString()}?"
        };

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsOpenAsync(
            triggerId,
            new ModalViewDefinition
            {
                CallbackId = DeskCallbackTypes.RemoveDesk,
                Title = "Remove Desk",
                Close = "No",
                Submit = "Yes",
                Blocks =
                    [confirmationMessage],
                PrivateMetadata = context.Serialize()
            },
            cancellationToken);
    }

    private async Task AddPreferredDeskAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        SetPreferredDeskContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.AddPreferredDeskAsync(
            new AddPreferredDeskInput { DeskId = context.DeskId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task RemovePreferredDeskAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        RemovePreferredDeskContext context,
        string? hash,
        CancellationToken cancellationToken)
    {
        await customerServiceClient.RemovePreferredDeskAsync(
            new RemovePreferredDeskInput { DeskId = context.DeskId },
            customerConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(context.PageContext),
            hash,
            cancellationToken);
    }

    private async Task<DeskTypeConnection> GetDeskTypesAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedDeskTypesInput = new GetPaginatedDeskTypesInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new DeskTypeWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedDeskTypesInput.OrderBy.AddRange([
            new DeskTypeOrderInput
            {
                Direction = global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrderDirection.Ascending,
                Field = DeskTypeOrderField.DeskTypeName
            }
        ]);

        return await organizationServiceClient.GetPaginatedDeskTypesAsync(
            getPaginatedDeskTypesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private async Task<ZoneConnection> GetZonesAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CancellationToken cancellationToken)
    {
        var getPaginatedZonesInput = new GetPaginatedZonesInput
        {
            After = string.Empty,
            First = -1,
            Before = string.Empty,
            Last = -1,
            Where = new ZoneWhereInput { OrganizationId = workspace.Organization.Id }
        };

        getPaginatedZonesInput.OrderBy.AddRange([
            new ZoneOrderInput
            {
                Direction = global::Api.Shared.Services.Grpc.UnityHub.Organization.V1.OrderDirection.Ascending,
                Field = ZoneOrderField.ZoneName
            }
        ]);

        return await organizationServiceClient.GetPaginatedZonesAsync(
            getPaginatedZonesInput,
            organizationConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private async Task HandleDatePickerChangedAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DatePickerAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);

        ArgumentNullException.ThrowIfNull(context.PageContext.DesksPage);

        context.PageContext.DesksPage.SelectedDate = action.SelectedDate?.ToDateTimeOffset() ??
                                                     timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            context,
            request.View.Hash,
            cancellationToken);
    }

    private async Task<BookingConnection> GetBookingsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(pageContext.DesksPage);

        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            First = -1,
            Last = -1,
            Where = new BookingWhereInput
            {
                FromGTE = pageContext.DesksPage.SelectedDate.ToTimestamp(),
                FromLTE = pageContext.DesksPage.SelectedDate.EndOfDay().ToTimestamp()
            }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.OrderBy.AddRange([
            new BookingOrderInput
            {
                Direction = global::Api.Shared.Services.Grpc.UnityHub.Booking.V1.OrderDirection.Ascending,
                Field = BookingOrderField.From
            }
        ]);

        return await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }
}
