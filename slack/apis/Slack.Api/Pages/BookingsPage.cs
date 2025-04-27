using Api.Shared.Services.Grpc.Skedular.Booking.V1;
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
using Icons = Slack.Shared.Constants.Icons;
using BookingService = Api.Shared.Services.Grpc.Skedular.Booking.V1.BookingService;
using Button = SlackNet.Blocks.Button;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IBookingsPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class BookingsPage(
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfiguration slackConfiguration,
    IRepositoryFactory repositoryFactory,
    IWorkspaceMemberService workspaceMemberService,
    ICommonComponents commonComponents,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IBookingService bookingService,
    IBookingComponents bookingComponents,
    IBookingsPageContextService bookingsPageContextService,
    IMapper mapper,
    TimeProvider timeProvider) :
    IBookingsPage,
    IAsyncPageRenderingCallbacks,
    IBlockActionHandler<ButtonAction>,
    IBlockActionHandler<CheckboxGroupAction>,
    IBlockActionHandler<DatePickerAction>
{
    private const int BookingsPageSize = 5;
    private const string BookingsCallback = "Bookings";
    private const string BookingsFromDatePickerKey = "BookingsFromDatePicker";
    private const string BookingsUntilDatePickerKey = "BookingsUntilDatePicker";
    private const string FirstPageBookings = "Bookings_FirstPageBookings";
    private const string PreviousPageBookings = "Bookings_PreviousPageBookings";
    private const string NextPageBookings = "Bookings_NextPageBookings";
    private const string LastPageBookings = "Bookings_LastPageBookings";
    private const string IncludeMyBookingsOnly = "Bookings_IncludeMyBookingsOnly";

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case FirstPageBookings:
                await RenderFirstPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case PreviousPageBookings:
                await RenderPreviousPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case NextPageBookings:
                await RenderNextPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

            case LastPageBookings:
                await RenderLastPageAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;
        }
    }

    public async Task HandleAsync(
        CheckboxGroupAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceEntity =
            await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case IncludeMyBookingsOnly:
                var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                var selectedOption = action.SelectedOptions.FirstOrDefault();
                context.PageContext.BookingsPage ??= bookingsPageContextService.GetDefaultBookingsPageContext();
                context.PageContext.BookingsPage.IncludeMyBookingsOnly = selectedOption is not null && selectedOption.Value == IncludeMyBookingsOnly;
                await RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    request.View.Hash,
                    cancellationToken);

                break;
        }
    }

    public async Task HandleAsync(
        DatePickerAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken);
        if (workspaceEntity is null)
        {
            throw new SlackWorkspaceNotFound();
        }

        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = mapper.MapTo(workspaceEntity);
        var workspaceMember = mapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case BookingsFromDatePickerKey:
                await HandleBookingsFromDatePickerKeyAsync(
                    workspace,
                    workspaceMember,
                    action,
                    request,
                    cancellationToken);
                break;

            case BookingsUntilDatePickerKey:
                await HandleBookingsUntilDatePickerKeyAsync(
                    workspace,
                    workspaceMember,
                    action,
                    request,
                    cancellationToken);
                break;
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

    public async Task Handle(CheckboxGroupAction action, BlockActionRequest request)
    {
        if (slackConfiguration.EnableAsyncMode)
        {
            asyncPageRenderingService.CheckboxGroupActionHandlerStream.OnNext((GetType(), action, request));
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

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        if (commonPageContext.PageContext.BookingsPage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.BookingsPage.Pagination.CurrentAfter,
                commonPageContext.PageContext.BookingsPage.Pagination.CurrentFirst,
                commonPageContext.PageContext.BookingsPage.Pagination.CurrentBefore,
                commonPageContext.PageContext.BookingsPage.Pagination.CurrentLast,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            BookingsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.BookingsPage.Pagination.Before,
            BookingsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.BookingsPage.Pagination.After,
            BookingsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            null,
            BookingsPageSize,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Bookings;
        commonPageContext.PageContext.BookingsPage.Pagination.CurrentAfter = after;
        commonPageContext.PageContext.BookingsPage.Pagination.CurrentFirst = first;
        commonPageContext.PageContext.BookingsPage.Pagination.CurrentBefore = before;
        commonPageContext.PageContext.BookingsPage.Pagination.CurrentLast = last;

        var bookingsDateRange = commonPageContext.PageContext.BookingsPage.BookingsDateRange;
        var from = bookingsDateRange.From?.StartOfDay() ?? timeProvider.GetUtcNow().StartOfDay();
        var until = bookingsDateRange.To?.EndOfDay() ?? from.AddMonths(1).StartOfDay();
        var response = await Task.WhenAll(GetPaginatedBookingsAsync(
                workspace,
                workspaceMember,
                after,
                first,
                before,
                last,
                from,
                until,
                commonPageContext,
                cancellationToken),
            GetMyBookingsAsync(workspace, workspaceMember, from, until, commonPageContext, cancellationToken));
        var bookingConnection = response.First();
        var bookings = bookingConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var myBookings = response.Last().Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var permissions = await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            GetToolbar(commonPageContext.PageContext, workspaceMember.Timezone),
            GetBookingsSearchCriteriaAndPaginationBlocks(
                bookingConnection,
                from,
                until,
                commonPageContext.PageContext),
            await bookingComponents.GetBookingCardsAsync(
                workspace,
                workspaceMember,
                bookings,
                myBookings,
                commonPageContext.PageContext,
                cancellationToken)
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = BookingsCallback,
                Blocks = blocks.SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()])).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterBlockActionHandler<DatePickerAction, BookingsPage>(BookingsFromDatePickerKey)
            .RegisterBlockActionHandler<DatePickerAction, BookingsPage>(BookingsUntilDatePickerKey)
            .RegisterBlockActionHandler<ButtonAction, BookingsPage>(FirstPageBookings)
            .RegisterBlockActionHandler<ButtonAction, BookingsPage>(LastPageBookings)
            .RegisterBlockActionHandler<ButtonAction, BookingsPage>(NextPageBookings)
            .RegisterBlockActionHandler<ButtonAction, BookingsPage>(PreviousPageBookings)
            .RegisterBlockActionHandler<CheckboxGroupAction, BookingsPage>(IncludeMyBookingsOnly);

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Bookings*".ToMarkdown() }
    ];

    private ICollection<Block> GetToolbar(PageContext pageContext, string timezone)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, timezone);
        var addBookingButton = bookingComponents.GetAddBookingButton(pageContext);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(addBookingButton)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }

    private async Task<BookingConnection> GetPaginatedBookingsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        DateTimeOffset from,
        DateTimeOffset until,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            After = after.ToSafeString(),
            First = first.ToNullInt(),
            Before = before.ToSafeString(),
            Last = last.ToNullInt(),
            Where = new BookingWhereInput
            {
                FromGTE = from.ToTimestamp(),
                FromLTE = until.ToTimestamp(),
                IncludeMineOnly = commonPageContext.PageContext.BookingsPage.IncludeMyBookingsOnly
            }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);

        if (commonPageContext.PageContext.BookingsPage.LocationIds.Count != 0)
        {
            getPaginatedBookingsInput.Where.LocationIds.AddRange(commonPageContext.PageContext.BookingsPage.LocationIds);
        }

        if (commonPageContext.PageContext.BookingsPage.TeamIds.Count != 0)
        {
            getPaginatedBookingsInput.Where.TeamIds.AddRange(commonPageContext.PageContext.BookingsPage.TeamIds);
        }

        getPaginatedBookingsInput.OrderBy.AddRange([new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }]);

        return await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private async Task<BookingConnection> GetMyBookingsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DateTimeOffset from,
        DateTimeOffset until,
        CommonPageContext commonPageContext,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BookingsPage);
        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            First = -1,
            Last = -1,
            Where = new BookingWhereInput { FromGTE = from.ToTimestamp(), FromLTE = until.ToTimestamp(), IncludeMineOnly = true }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        if (commonPageContext.PageContext.BookingsPage.LocationIds.Count != 0)
        {
            getPaginatedBookingsInput.Where.LocationIds.AddRange(commonPageContext.PageContext.BookingsPage.LocationIds);
        }

        if (commonPageContext.PageContext.BookingsPage.TeamIds.Count != 0)
        {
            getPaginatedBookingsInput.Where.TeamIds.AddRange(commonPageContext.PageContext.BookingsPage.TeamIds);
        }

        getPaginatedBookingsInput.OrderBy.AddRange([new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }]);

        return await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private List<Block> GetBookingsSearchCriteriaAndPaginationBlocks(
        BookingConnection bookingConnection,
        DateTimeOffset from,
        DateTimeOffset until,
        PageContext pageContext)
    {
        ArgumentNullException.ThrowIfNull(pageContext.BookingsPage);

        var fromDatePicker = new DatePicker { ActionId = BookingsFromDatePickerKey, InitialDate = from.ToDateTime() };
        var untilDatePicker = new DatePicker { ActionId = BookingsUntilDatePickerKey, InitialDate = until.ToDateTime() };

        List<Block> dateRangeActionBlock =
        [
            new ActionsBlock { Elements = [fromDatePicker, untilDatePicker] },
            bookingComponents.GetOnlyShowMyBookingCheckbox(IncludeMyBookingsOnly, pageContext.BookingsPage.IncludeMyBookingsOnly),
            new DividerBlock()
        ];

        if (bookingConnection.Edges.Count == 0)
        {
            return dateRangeActionBlock.Concat([new SectionBlock { Text = "No booking found".ToMarkdown() }]).ToList();
        }

        var totalBookingsCount = new SectionBlock { Text = $"Total bookings: {bookingConnection.TotalCount}".ToMarkdown() };
        if (bookingConnection.TotalCount <= BookingsPageSize)
        {
            return dateRangeActionBlock.Concat([totalBookingsCount]).ToList();
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.BookingsPage);

        var paginationButtons = new List<IActionElement>();
        if (bookingConnection.PageInfo.HasPreviousPage)
        {
            pageContext.BookingsPage.Pagination.First = BookingsPageSize;
            pageContext.BookingsPage.Pagination.After = null;
            pageContext.BookingsPage.Pagination.Before = null;
            pageContext.BookingsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageBookings, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.BookingsPage.Pagination.First = null;
            pageContext.BookingsPage.Pagination.After = null;
            pageContext.BookingsPage.Pagination.Before = bookingConnection.PageInfo.StartCursor;
            pageContext.BookingsPage.Pagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageBookings, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (bookingConnection.PageInfo.HasNextPage)
        {
            pageContext.BookingsPage.Pagination.First = BookingsPageSize;
            pageContext.BookingsPage.Pagination.After = bookingConnection.PageInfo.EndCursor;
            pageContext.BookingsPage.Pagination.Before = null;
            pageContext.BookingsPage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageBookings, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.BookingsPage.Pagination.First = null;
            pageContext.BookingsPage.Pagination.After = null;
            pageContext.BookingsPage.Pagination.Before = null;
            pageContext.BookingsPage.Pagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageBookings, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return dateRangeActionBlock.Concat([totalBookingsCount, paginationActionBlock]).ToList();
    }

    private async Task HandleBookingsFromDatePickerKeyAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DatePickerAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);

        context.PageContext.BookingsPage ??= bookingsPageContextService.GetDefaultBookingsPageContext();
        context.PageContext.BookingsPage.BookingsDateRange.From = action.SelectedDate?.ToDateTimeOffset() ?? timeProvider.GetUtcNow().StartOfDay();

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            context,
            request.View.Hash,
            cancellationToken);
    }

    private async Task HandleBookingsUntilDatePickerKeyAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DatePickerAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);

        context.PageContext.BookingsPage ??= bookingsPageContextService.GetDefaultBookingsPageContext();
        context.PageContext.BookingsPage.BookingsDateRange.To = action.SelectedDate?.ToDateTimeOffset() ?? timeProvider.GetUtcNow().StartOfDay();

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            context,
            request.View.Hash,
            cancellationToken);
    }
}
