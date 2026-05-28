using Api.Shared.Services;
using Enterprise.Shared;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Time;
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
using SlackNet.Events;
using SlackNet.Interaction;
using Booking = Slack.Shared.Models.Booking;
using BookingEdge = Slack.Shared.Models.BookingEdge;
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
    AsyncPageRenderingService asyncPageRenderingService,
    SlackConfigurationService slackConfigurationService,
    IWorkspaceMemberService workspaceMemberService,
    IRepositoryFactory repositoryFactory,
    IHomePageContextService homePageContextService,
    IBookingsPage bookingsPage,
    ILocationsPage locationsPage,
    ITeamsPage teamsPage,
    ICustomTagsPage customTagsPage,
    IZonesPage zonesPage,
    ISettingsPage settingsPage,
    IEntityMapper entityMapper,
    ICommonComponents commonComponents,
    IBookingComponents bookingComponents,
    ICustomerService customerService,
    IBookingPermissionsService bookingPermissionsService,
    TimeProvider timeProvider,
    IBookingsPageContextService bookingsPageContextService,
    IBookingService bookingService) :
    IHomePage,
    IAsyncPageRenderingCallbacks,
    IEventHandler<AppHomeOpened>,
    IBlockActionHandler<ButtonAction>,
    IBlockActionHandler<DatePickerAction>,
    IBlockActionHandler<StaticSelectAction>,
    IBlockActionHandler<CheckboxGroupAction>
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

    public async Task HandleAsync(AppHomeOpened appHomeOpenedEvent, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(appHomeOpenedEvent);

        switch (appHomeOpenedEvent.Tab)
        {
            case AppHomeTab.Home:
                {
                    Shared.Database.Entities.Workspace? workspaceEntity;
                    if (appHomeOpenedEvent.View is null)
                    {
                        ArgumentException.ThrowIfNullOrWhiteSpace(appHomeOpenedEvent.User);
                        workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByWorkspaceMemberIdAsync(
                            appHomeOpenedEvent.User,
                            cancellationToken);
                    }
                    else
                    {
                        ArgumentException.ThrowIfNullOrWhiteSpace(appHomeOpenedEvent.View.TeamId);
                        workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(appHomeOpenedEvent.View.TeamId, cancellationToken);
                    }

                    if (workspaceEntity is null)
                    {
                        throw new SlackWorkspaceNotFound();
                    }

                    var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
                        workspaceEntity,
                        appHomeOpenedEvent.User,
                        cancellationToken);

                    var workspace = entityMapper.MapTo(workspaceEntity);
                    await RenderAsync(
                        workspace,
                        entityMapper.MapTo(workspaceMemberEntity, workspace),
                        appHomeOpenedEvent.View?.Hash,
                        cancellationToken);
                }
                break;

            case AppHomeTab.Messages:
                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public async Task HandleAsync(ButtonAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

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

    public async Task HandleAsync(DatePickerAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.ActionId)
        {
            case BookingDatePicker:
                await HandleDatePickerChangedAsync(workspace, workspaceMember, action, request, cancellationToken);

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

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

        switch (action.SelectedOption.Value)
        {
            case BookingActionTypes.Bookings:
                {
                    var permissions = await bookingPermissionsService.GetOrganizationPermissionsAsync(
                        workspaceMember.Id,
                        workspace.Organization.Id,
                        cancellationToken);
                    if (!permissions.CanViewBookings)
                    {
                        throw new UnauthorizedAccessException();
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

            case CustomTagActionTypes.CustomTags:
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

                break;

            case ZoneActionTypes.Zones:
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

    public async Task HandleAsync(CheckboxGroupAction action, BlockActionRequest request, CancellationToken cancellationToken)
    {
        var workspaceEntity = await repositoryFactory.WorkspaceRepository.GetByIdAsync(request.Team.Id, cancellationToken) ??
                              throw new SlackWorkspaceNotFound();
        var (workspaceMemberEntity, _) = await workspaceMemberService.EnsureCustomerResourcesAllExistAsync(
            workspaceEntity,
            request.User.Id,
            cancellationToken);

        var workspace = entityMapper.MapTo(workspaceEntity);
        var workspaceMember = entityMapper.MapTo(workspaceMemberEntity, workspace);

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

    public async Task Handle(CheckboxGroupAction action, BlockActionRequest request)
    {
        if (slackConfigurationService.EnableAsyncMode)
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
        if (slackConfigurationService.EnableAsyncMode)
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
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.StaticSelectActionHandlerStream.OnNext((GetType(), action, request));
        }
        else
        {
            await HandleAsync(action, request, CancellationToken.None);
        }
    }

    public async Task Handle(AppHomeOpened slackEvent)
    {
        if (slackConfigurationService.EnableAsyncMode)
        {
            asyncPageRenderingService.EventHandlerStream.OnNext((GetType(), slackEvent));
        }
        else
        {
            await HandleAsync(slackEvent, CancellationToken.None);
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
        if (commonPageContext.PageContext.HomePage.Pagination.IsEmpty())
        {
            await RenderFirstPageAsync(workspace, workspaceMember, commonPageContext, hash, cancellationToken);
        }
        else
        {
            await RenderInternalAsync(
                workspace,
                workspaceMember,
                commonPageContext.PageContext.HomePage.Pagination.CurrentAfter,
                commonPageContext.PageContext.HomePage.Pagination.CurrentFirst,
                commonPageContext.PageContext.HomePage.Pagination.CurrentBefore,
                commonPageContext.PageContext.HomePage.Pagination.CurrentLast,
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
            new CommonPageContext(new PageContext { HomePage = homePageContextService.GetDefaultHomePageContext() }),
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
            commonPageContext.PageContext.HomePage.Pagination.Before,
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
            commonPageContext.PageContext.HomePage.Pagination.After,
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
        commonPageContext.PageContext.HomePage.Pagination.CurrentAfter = after;
        commonPageContext.PageContext.HomePage.Pagination.CurrentFirst = first;
        commonPageContext.PageContext.HomePage.Pagination.CurrentBefore = before;
        commonPageContext.PageContext.HomePage.Pagination.CurrentLast = last;

        var from = commonPageContext.PageContext.HomePage.SelectedDate.StartOfWeek(workspaceMember.ToDayOfWeek());
        var until = from.AddDays(7);
        var response = await Task.WhenAll(
            GetPaginatedBookingsAsync(
                workspace,
                workspaceMember,
                after,
                first,
                before,
                last,
                from,
                until,
                commonPageContext.PageContext.HomePage.IncludeMyBookingsOnly,
                cancellationToken),
            GetMyBookingsAsync(workspace, workspaceMember, from, until, cancellationToken));
        var bookingConnection = response.First();
        var bookings = bookingConnection.Edges.Select(item => item.Node).ToList();
        var myBookings = response.Last().Edges.Select(item => item.Node).ToList();

        var asyncBlocks = await Task.WhenAll(
            GetBookingCalendarSettingBlocksAsync(workspaceMember, myBookings, commonPageContext.PageContext, cancellationToken),
            bookingComponents.GetBookingCardsAsync(
                workspace,
                workspaceMember,
                bookings,
                myBookings,
                commonPageContext.PageContext,
                cancellationToken));

        asyncBlocks =
        [
            GetTitle(),
            GetToolbar(commonPageContext.PageContext),
            asyncBlocks[0],
            GetBookingsSearchCriteriaAndPaginationBlocks(bookingConnection, commonPageContext.PageContext),
            asyncBlocks[1]
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = HomeCallback,
                Blocks = asyncBlocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1).ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    private async Task<Connection<BookingEdge>> GetPaginatedBookingsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        string? after,
        int? first,
        string? before,
        int? last,
        DateTimeOffset from,
        DateTimeOffset until,
        bool includeMyBookingsOnly,
        CancellationToken cancellationToken) =>
        await bookingService.GetPaginatedBookingsAsync(
            workspaceMember.Id,
            new BookingSearchCriteria(
                null,
                from,
                null,
                until,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                [],
                includeMyBookingsOnly,
                null,
                workspace.Organization.Id,
                [],
                [],
                []),
            after,
            first,
            before,
            last,
            cancellationToken);

    private async Task<Connection<BookingEdge>> GetMyBookingsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        DateTimeOffset from,
        DateTimeOffset until,
        CancellationToken cancellationToken) =>
        await bookingService.GetPaginatedBookingsAsync(
            workspaceMember.Id,
            new BookingSearchCriteria(
                null,
                from,
                null,
                until,
                null,
                null,
                null,
                null,
                null,
                null,
                null,
                [],
                true,
                null,
                workspace.Organization.Id,
                [],
                [],
                []),
            string.Empty,
            ((int?)null).ToNullInt(),
            string.Empty,
            ((int?)null).ToNullInt(),
            cancellationToken);

    private static IReadOnlyList<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Welcome to Skedular*".ToMarkdown() }
    ];

    private IReadOnlyList<Block> GetToolbar(PageContext pageContext)
    {
        var backButton = commonComponents.GetBackButton(pageContext);
        var addBookingButton = bookingComponents.GetAddBookingButton(pageContext);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);
        var actionMenus = new StaticSelectMenu
        {
            ActionId = ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options =
            [
                new Option { Value = BookingActionTypes.Bookings, Text = "Bookings".ToPlainTextWithIcon(Icons.Bookings) },
                new Option { Value = LocationActionTypes.Locations, Text = "Locations".ToPlainTextWithIcon(Icons.Locations) },
                new Option { Value = TeamActionTypes.Teams, Text = "Teams".ToPlainTextWithIcon(Icons.Teams) },
                new Option { Value = CustomTagActionTypes.CustomTags, Text = "Tags".ToPlainTextWithIcon(Icons.CustomTags) },
                new Option { Value = ZoneActionTypes.Zones, Text = "Zones".ToPlainTextWithIcon(Icons.Zones) },
                new Option { Value = SettingsActionTypes.Settings, Text = "Settings".ToPlainTextWithIcon(Icons.Settings) }
            ]
        };

        return [new ActionsBlock { Elements = backButton.Concat(addBookingButton).Concat(feedbackButton).Append(actionMenus).ToList() }];
    }

    private async Task<IReadOnlyList<Block>> GetBookingCalendarSettingBlocksAsync(
        WorkspaceMember workspaceMember,
        IReadOnlyList<Booking> myBookings,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember.Id, cancellationToken) ?? throw new CustomerNotFound();
        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.HomePage);

        var header = new SectionBlock { Text = "*Select a day to see the bookings for the week*".ToMarkdown() };
        var datePicker = new ActionsBlock
        {
            Elements = [new DatePicker { ActionId = BookingDatePicker, InitialDate = pageContext.HomePage.SelectedDate.ToDateTime() }]
        };

        const int DayCount = 7;
        var startOfWeek = pageContext.HomePage.SelectedDate.StartOfWeek(workspaceMember.ToDayOfWeek());
        var bookingButtons = new ActionsBlock
        {
            Elements = Enumerable.Range(0, DayCount).Select(IActionElement (idx) =>
            {
                var from = startOfWeek.AddDays(idx).ToDate(TimeSpan.Zero);
                var matchingBookings = myBookings.Where(item =>
                {
                    var bookingFrom = item.From;
                    return from.Year == bookingFrom.Year && from.Month == bookingFrom.Month && from.Day == bookingFrom.Day;
                }).ToList();

                string actionId;
                string value;
                PlainText buttonText;
                if (matchingBookings.Count == 0)
                {
                    var until = from.EndOfDay();
                    actionId = $"{BookingActionTypes.InstantAddBooking}{idx}";
                    buttonText = $"{from.ToShortDateWithoutYear()} {Icons.New}".ToPlainTextWithIcon(Icons.Calendar);
                    value = new InstantAddBookingContext(pageContext, from, until, InitiationSource.App, customer.Id, null, null).Serialize();
                }
                else
                {
                    actionId = $"{BookingActionTypes.CancelBooking}{idx}";
                    buttonText = $"{from.ToShortDateWithoutYear()} {Icons.Cancel}".ToPlainTextWithIcon(Icons.Calendar);
                    value = new CancelBookingContext(pageContext, matchingBookings.First().Id).Serialize();
                }

                return new Button { ActionId = actionId, Text = buttonText, Value = value };
            }).ToList()
        };

        return
        [
            header,
            datePicker,
            bookingComponents.GetOnlyShowMyBookingCheckbox(IncludeMyBookingsOnly, pageContext.HomePage.IncludeMyBookingsOnly),
            bookingButtons
        ];
    }

    private static List<Block> GetBookingsSearchCriteriaAndPaginationBlocks(Connection<BookingEdge> bookingConnection, PageContext pageContext)
    {
        if (!bookingConnection.Edges.Any())
        {
            return [new SectionBlock { Text = "No booking found".ToMarkdown() }];
        }

        var totalBookingsCount = new SectionBlock { Text = $"Total bookings: {bookingConnection.TotalCount}".ToMarkdown() };
        if (bookingConnection.TotalCount <= BookingsPageSize)
        {
            return [totalBookingsCount];
        }

        pageContext = pageContext.Clone();
        ArgumentNullException.ThrowIfNull(pageContext.HomePage);

        var paginationButtons = new List<IActionElement>();

        if (bookingConnection.PageInfo.HasPreviousPage)
        {
            pageContext.HomePage.Pagination.First = BookingsPageSize;
            pageContext.HomePage.Pagination.After = null;
            pageContext.HomePage.Pagination.Before = null;
            pageContext.HomePage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = FirstPageBookings, Text = Icons.FirstPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.HomePage.Pagination.First = null;
            pageContext.HomePage.Pagination.After = null;
            pageContext.HomePage.Pagination.Before = bookingConnection.PageInfo.StartCursor;
            pageContext.HomePage.Pagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = PreviousPageBookings, Text = Icons.PreviousPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });
        }

        if (bookingConnection.PageInfo.HasNextPage)
        {
            pageContext.HomePage.Pagination.First = BookingsPageSize;
            pageContext.HomePage.Pagination.After = bookingConnection.PageInfo.EndCursor;
            pageContext.HomePage.Pagination.Before = null;
            pageContext.HomePage.Pagination.Last = null;

            paginationButtons.Add(new Button
            {
                ActionId = NextPageBookings, Text = Icons.NextPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
            });

            pageContext.HomePage.Pagination.First = null;
            pageContext.HomePage.Pagination.After = null;
            pageContext.HomePage.Pagination.Before = null;
            pageContext.HomePage.Pagination.Last = BookingsPageSize;

            paginationButtons.Add(new Button
            {
                ActionId = LastPageBookings, Text = Icons.LastPage.ToPlainText(), Value = new CommonPageContext(pageContext).Serialize()
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
        context.PageContext.HomePage.SelectedDate = action.SelectedDate?.ToDateTimeOffset() ?? timeProvider.GetUtcNow().StartOfDay();

        await RenderWithContextAsync(workspace, workspaceMember, context, request.View.Hash, cancellationToken);
    }
}
