using System.Text.Json;
using Api.Shared.Services.Grpc.UnityHub.Booking.V1;
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
using SlackNet.Events;
using SlackNet.Interaction;
using Booking = Slack.Shared.Models.Booking;
using BookingService = Api.Shared.Services.Grpc.UnityHub.Booking.V1.BookingService;
using Button = SlackNet.Blocks.Button;
using Icons = Slack.Shared.Constants.Icons;
using Option = SlackNet.Blocks.Option;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IHomePage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class HomePage(
    ILogger<HomePage> logger,
    BookingConfiguration bookingConfiguration,
    BookingService.BookingServiceClient bookingServiceClient,
    IWorkspaceMemberService workspaceMemberService,
    IRepositoryFactory repositoryFactory,
    IHomePageContextService homePageContextService,
    IBookingsPage bookingsPage,
    ILocationsPage locationsPage,
    ITeamsPage teamsPage,
    ISettingsPage settingsPage,
    IMapper mapper,
    ICommonComponents commonComponents,
    ISettingsComponents settingsComponents,
    IBookingComponents bookingComponents,
    ICustomerService customerService,
    IBookingService bookingService,
    TimeProvider timeProvider,
    IBookingsPageContextService bookingsPageContextService)
    : IHomePage, IBlockActionHandler<ButtonAction>, IBlockActionHandler<DatePickerAction>, IEventHandler<AppHomeOpened>,
        IBlockActionHandler<StaticSelectAction>, IBlockActionHandler<CheckboxGroupAction>
{
    private const int BookingsPageSize = 5;
    private const string HomeCallback = "Home";
    private const string ActionsMenu = "Home_ActionsMenu";
    private const string BookingDatePicker = "Home_BookingDatePicker";
    private const string FirstPageBookings = "Home_FirstPageBookings";
    private const string PreviousPageBookings = "Home_PreviousPageBookings";
    private const string NextPageBookings = "Home_NextPageBookings";
    private const string LastPageBookings = "Home_LastPageBookings";
    private const string IncludeMyBookingsOnly = "Home_IncludeMyBookingsOnly";

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;

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
            case HomeActionTypes.Home:
                await RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    CommonPageContext.Deserialize(action.Value),
                    request.View.Hash,
                    cancellationToken);
                break;

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

    public async Task Handle(CheckboxGroupAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;

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
            case IncludeMyBookingsOnly:
                var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                var selectedOption = action.SelectedOptions.FirstOrDefault();
                context.PageContext.HomePage ??= homePageContextService.GetDefaultHomePageContext();
                context.PageContext.HomePage.IncludeMyBookingsOnly = selectedOption is not null &&
                                                                     selectedOption.Value ==
                                                                     IncludeMyBookingsOnly;
                await RenderWithContextAsync(
                    workspace,
                    workspaceMember,
                    new CommonPageContext(context.PageContext),
                    request.View.Hash,
                    cancellationToken);

                break;
        }
    }

    public async Task Handle(DatePickerAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;
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

    public async Task Handle(StaticSelectAction action, BlockActionRequest request)
    {
        var cancellationToken = CancellationToken.None;

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

        switch (action.SelectedOption.Value)
        {
            case BookingActionTypes.Bookings:
                {
                    var permissions =
                        await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember,
                            cancellationToken);
                    if (!permissions.CanViewBookings)
                    {
                        throw new Unauthorized();
                    }

                    var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                    context.PageContext.BookingsPage = bookingsPageContextService.GetDefaultBookingsPageContext();
                    context.PageContext.PushCurrentPageToVisitedPages();

                    await bookingsPage.RenderWithContextAsync(
                        workspace,
                        workspaceMember,
                        new CommonPageContext(context.PageContext),
                        request.View.Hash,
                        cancellationToken);
                }

                break;

            case LocationActionTypes.Locations:
                {
                    var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                    context.PageContext.LocationsPage = new Shared.Context.LocationsPage(new PaginationContext());
                    context.PageContext.PushCurrentPageToVisitedPages();

                    await locationsPage.RenderWithContextAsync(
                        workspace,
                        workspaceMember,
                        new CommonPageContext(context.PageContext),
                        request.View.Hash,
                        cancellationToken);
                }

                break;

            case TeamActionTypes.Teams:
                {
                    var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                    context.PageContext.TeamsPage = new Shared.Context.TeamsPage(new PaginationContext());
                    context.PageContext.PushCurrentPageToVisitedPages();

                    await teamsPage.RenderWithContextAsync(
                        workspace,
                        workspaceMember,
                        new CommonPageContext(context.PageContext),
                        request.View.Hash,
                        cancellationToken);
                }

                break;

            case SettingsActionTypes.Settings:
                {
                    var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);
                    context.PageContext.SettingsPage = new Shared.Context.SettingsPage();
                    context.PageContext.PushCurrentPageToVisitedPages();

                    await settingsPage.RenderWithContextAsync(
                        workspace,
                        workspaceMember,
                        new CommonPageContext(context.PageContext),
                        request.View.Hash,
                        cancellationToken);
                }

                break;
        }
    }

    public async Task Handle(AppHomeOpened slackEvent)
    {
        ArgumentNullException.ThrowIfNull(slackEvent);

        logger.LogWarning(JsonSerializer.Serialize(slackEvent));

        var cancellationToken = CancellationToken.None;
        switch (slackEvent.Tab)
        {
            case AppHomeTab.Home:
                {
                    Shared.Database.Entities.Workspace? workspaceEntity;
                    if (slackEvent.View is null)
                    {
                        ArgumentException.ThrowIfNullOrWhiteSpace(slackEvent.User);
                        workspaceEntity =
                            await repositoryFactory.WorkspaceRepository.GetByWorkspaceMemberIdAsync(
                                slackEvent.User,
                                cancellationToken);
                    }
                    else
                    {
                        ArgumentException.ThrowIfNullOrWhiteSpace(slackEvent.View.TeamId);
                        workspaceEntity =
                            await repositoryFactory.WorkspaceRepository.GetByIdAsync(
                                slackEvent.View.TeamId,
                                cancellationToken);
                    }

                    if (workspaceEntity is null)
                    {
                        throw new SlackWorkspaceNotFound();
                    }

                    var (workspaceMemberEntity, _) =
                        await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                            workspaceEntity,
                            slackEvent.User,
                            cancellationToken);

                    var workspace = mapper.MapTo(workspaceEntity);
                    await RenderAsync(
                        workspace,
                        mapper.MapTo(workspaceMemberEntity, workspace),
                        slackEvent.View?.Hash,
                        cancellationToken);
                }
                break;

            case AppHomeTab.Messages:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);
        if (commonPageContext.PageContext.HomePage.BookingsPagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.HomePage.BookingsPagination.CurrentAfter,
                commonPageContext.PageContext.HomePage.BookingsPagination.CurrentFirst,
                commonPageContext.PageContext.HomePage.BookingsPagination.CurrentBefore,
                commonPageContext.PageContext.HomePage.BookingsPagination.CurrentLast,
                commonPageContext,
                hash,
                cancellationToken);
        }
    }

    private async Task RenderAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? hash,
        CancellationToken cancellationToken) =>
        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            new CommonPageContext(
                new PageContext { HomePage = homePageContextService.GetDefaultHomePageContext() }
            ),
            hash,
            cancellationToken);

    private async Task RenderFirstPageAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            null,
            null,
            commonPageContext.PageContext.HomePage.BookingsPagination.Before,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);
        await RenderInternalAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext.HomePage.BookingsPagination.After,
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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);
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

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) =>
        options
            .RegisterEventHandler<AppHomeOpened, HomePage>()
            .RegisterBlockActionHandler<DatePickerAction, HomePage>(BookingDatePicker)
            .RegisterBlockActionHandler<StaticSelectAction, HomePage>(ActionsMenu)
            .RegisterBlockActionHandler<ButtonAction, HomePage>(HomeActionTypes.Home)
            .RegisterBlockActionHandler<ButtonAction, HomePage>(FirstPageBookings)
            .RegisterBlockActionHandler<ButtonAction, HomePage>(LastPageBookings)
            .RegisterBlockActionHandler<ButtonAction, HomePage>(NextPageBookings)
            .RegisterBlockActionHandler<ButtonAction, HomePage>(PreviousPageBookings)
            .RegisterBlockActionHandler<CheckboxGroupAction, HomePage>(IncludeMyBookingsOnly);

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
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.HomePage);

        commonPageContext.PageContext.CurrentPageType = PageType.Home;
        commonPageContext.PageContext.HomePage.BookingsPagination.CurrentAfter = after;
        commonPageContext.PageContext.HomePage.BookingsPagination.CurrentFirst = first;
        commonPageContext.PageContext.HomePage.BookingsPagination.CurrentBefore = before;
        commonPageContext.PageContext.HomePage.BookingsPagination.CurrentLast = last;

        var from = commonPageContext.PageContext.HomePage.SelectedDate.StartOfWeek();
        var until = from.AddDays(6);
        var response = await Task.WhenAll(GetPaginatedBookingsAsync(
            workspace,
            workspaceMember,
            after,
            first,
            before,
            last,
            from,
            until,
            commonPageContext.PageContext.HomePage.IncludeMyBookingsOnly,
            cancellationToken), GetMyBookingsAsync(workspace, workspaceMember, from, until, cancellationToken));
        var bookingConnection = response.First();
        var bookings = bookingConnection.Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var myBookings = response.Last().Edges.Select(item => mapper.MapTo(item.Node)).ToList();
        var permissions =
            await bookingService.GetOrganizationPermissionsAsync(workspace, workspaceMember, cancellationToken);

        var asyncBlocks = await Task.WhenAll(settingsComponents.GetDefaultLocationOnboardingDoneAsync(
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), settingsComponents.GetPreferredZoneOnboardingDoneAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), settingsComponents.GetPreferredDeskOnboardingDoneAsync(
            workspace,
            workspaceMember,
            commonPageContext.PageContext,
            cancellationToken), GetBookingCalendarSettingBlocksAsync(
            workspaceMember,
            myBookings,
            commonPageContext.PageContext,
            cancellationToken), bookingComponents.GetBookingCardsAsync(
            workspace,
            workspaceMember,
            bookings,
            myBookings,
            permissions.CanUpdateBookingOnBehalf,
            permissions.CanDeleteBookingOnBehalf,
            commonPageContext.PageContext,
            cancellationToken));

        asyncBlocks =
        [
            asyncBlocks[0],
            asyncBlocks[1],
            asyncBlocks[2],
            GetTitle(),
            GetToolbar(commonPageContext.PageContext),
            asyncBlocks[3],
            GetBookingsSearchCriteriaAndPaginationBlocks(bookingConnection, commonPageContext.PageContext),
            asyncBlocks[4]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.Publish(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = HomeCallback,
                Blocks = asyncBlocks
                    .SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()]))
                    .SkipLast(1)
                    .ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash);
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
        bool includeMyBookingsOnly,
        CancellationToken cancellationToken)
    {
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
                IncludeMineOnly = includeMyBookingsOnly
            }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.OrderBy.AddRange([
            new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }
        ]);

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
        CancellationToken cancellationToken)
    {
        var getPaginatedBookingsInput = new GetPaginatedBookingsInput
        {
            First = -1,
            Last = -1,
            Where = new BookingWhereInput
            {
                FromGTE = from.ToTimestamp(), FromLTE = until.ToTimestamp(), IncludeMineOnly = true
            }
        };
        getPaginatedBookingsInput.Where.OrganizationIds.Add(workspace.Organization.Id);
        getPaginatedBookingsInput.OrderBy.AddRange([
            new BookingOrderInput { Direction = OrderDirection.Ascending, Field = BookingOrderField.From }
        ]);

        return await bookingServiceClient.GetPaginatedBookingsAsync(
            getPaginatedBookingsInput,
            bookingConfiguration.ApiKey.CreateMetadata(workspaceMember.Id),
            cancellationToken: cancellationToken);
    }

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Welcome to UnityHub*".ToMarkdown() }
    ];

    private ICollection<Block> GetToolbar(PageContext pageContext)
    {
        var backButton = commonComponents.GetBackButton(pageContext);
        var addBookingButton = bookingComponents.GetAddBookingButton(null, null, pageContext);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);
        var actionMenus = new StaticSelectMenu
        {
            ActionId = ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options =
            [
                new Option
                {
                    Value = BookingActionTypes.Bookings, Text = "Bookings".ToPlainTextWithIcon(Icons.Bookings)
                },
                new Option
                {
                    Value = LocationActionTypes.Locations, Text = "Locations".ToPlainTextWithIcon(Icons.Locations)
                },
                new Option { Value = TeamActionTypes.Teams, Text = "Teams".ToPlainTextWithIcon(Icons.Teams) },
                new Option
                {
                    Value = SettingsActionTypes.Settings, Text = "Settings".ToPlainTextWithIcon(Icons.Settings)
                }
            ]
        };

        return
        [
            new ActionsBlock
            {
                Elements = backButton
                    .Concat(addBookingButton)
                    .Concat(feedbackButton)
                    .Concat([actionMenus])
                    .ToList()
            }
        ];
    }

    private async Task<ICollection<Block>> GetBookingCalendarSettingBlocksAsync(
        WorkspaceMember workspaceMember,
        ICollection<Booking> myBookings,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        ArgumentNullException.ThrowIfNull(customer);

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.HomePage);

        var header = new SectionBlock { Text = "*Select a day to see the bookings for the week*".ToMarkdown() };
        var datePicker = new ActionsBlock
        {
            Elements =
            [
                new DatePicker
                {
                    ActionId = BookingDatePicker, InitialDate = pageContext.HomePage.SelectedDate.ToDateTime()
                }
            ]
        };

        const int DayCount = 7;
        var startOfWeek = pageContext.HomePage.SelectedDate.StartOfWeek();
        var bookingButtons = new ActionsBlock
        {
            Elements = Enumerable.Range(0, DayCount).Select(idx =>
            {
                var from = startOfWeek.AddDays(idx).ToDate();
                var matchingBookings = myBookings.Where(item =>
                {
                    var bookingFrom = item.From;
                    return from.Year == bookingFrom.Year && from.Month == bookingFrom.Month &&
                           from.Day == bookingFrom.Day;
                }).ToList();

                string actionId;
                string value;
                PlainText buttonText;
                if (matchingBookings.Count == 0)
                {
                    var to = from.EndOfDay();
                    actionId = $"{BookingActionTypes.InstantAddBooking}{idx}";
                    buttonText = $"{to.ToShortDateWithoutYear()} {Icons.New}".ToPlainTextWithIcon(Icons.Calendar);
                    value = new InstantAddBookingContext(
                            pageContext,
                            from,
                            to,
                            InitiationSource.App,
                            customer.Id,
                            null,
                            null)
                        .Serialize();
                }
                else
                {
                    actionId = $"{BookingActionTypes.CancelBooking}{idx}";
                    buttonText = $"{from.ToShortDateWithoutYear()} {Icons.Cancel}".ToPlainTextWithIcon(Icons.Calendar);
                    value = new CancelBookingContext(pageContext, matchingBookings.First().Id).Serialize();
                }

                return (IActionElement)new Button { ActionId = actionId, Text = buttonText, Value = value };
            }).ToList()
        };

        return
        [
            header,
            datePicker,
            bookingComponents.GetOnlyShowMyBookingCheckbox(
                IncludeMyBookingsOnly,
                pageContext.HomePage.IncludeMyBookingsOnly),
            bookingButtons
        ];
    }

    private static List<Block> GetBookingsSearchCriteriaAndPaginationBlocks(
        BookingConnection bookingConnection,
        PageContext pageContext)
    {
        if (bookingConnection.Edges.Count == 0)
        {
            return
            [
                new SectionBlock { Text = "No booking found".ToMarkdown() }
            ];
        }

        var totalBookingsCount = new SectionBlock
        {
            Text = $"Total bookings: {bookingConnection.TotalCount}".ToMarkdown()
        };
        if (bookingConnection.TotalCount <= BookingsPageSize)
        {
            return [totalBookingsCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.HomePage);

        var paginationButtons = new List<IActionElement>();

        if (bookingConnection.PageInfo.HasPreviousPage)
        {
            pageContext.HomePage.BookingsPagination.First = BookingsPageSize;
            pageContext.HomePage.BookingsPagination.After = null;
            pageContext.HomePage.BookingsPagination.Before = null;
            pageContext.HomePage.BookingsPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageBookings,
                Text = Icons.FirstPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.HomePage.BookingsPagination.First = null;
            pageContext.HomePage.BookingsPagination.After = null;
            pageContext.HomePage.BookingsPagination.Before = bookingConnection.PageInfo.StartCursor;
            pageContext.HomePage.BookingsPagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageBookings,
                Text = Icons.PreviousPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (bookingConnection.PageInfo.HasNextPage)
        {
            pageContext.HomePage.BookingsPagination.First = BookingsPageSize;
            pageContext.HomePage.BookingsPagination.After = bookingConnection.PageInfo.EndCursor;
            pageContext.HomePage.BookingsPagination.Before = null;
            pageContext.HomePage.BookingsPagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageBookings,
                Text = Icons.NextPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.HomePage.BookingsPagination.First = null;
            pageContext.HomePage.BookingsPagination.After = null;
            pageContext.HomePage.BookingsPagination.Before = null;
            pageContext.HomePage.BookingsPagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageBookings,
                Text = Icons.LastPage.ToPlainText(),
                Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        var paginationActionBlock = new ActionsBlock { Elements = paginationButtons };

        return
        [
            totalBookingsCount,
            paginationActionBlock
        ];
    }

    private async Task HandleDatePickerChangedAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DatePickerAction action,
        BlockActionRequest request,
        CancellationToken cancellationToken)
    {
        var context = CommonPageContext.Deserialize(request.View.PrivateMetadata);

        context.PageContext.HomePage ??= homePageContextService.GetDefaultHomePageContext();
        context.PageContext.HomePage.SelectedDate = action.SelectedDate?.ToDateTimeOffset() ??
                                                    timeProvider.GetUtcNow().StartOfDay(TimeZoneInfo.Utc);

        await RenderWithContextAsync(
            workspace,
            workspaceMember,
            context,
            request.View.Hash,
            cancellationToken);
    }
}
