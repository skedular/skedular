using Enterprise.Shared;
using Enterprise.Shared.Time;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;
using IWorkspaceMemberService = Slack.Shared.Services.IWorkspaceMemberService;

namespace Slack.Api.Components;

public interface IBookingComponents
{
    Block GetOnlyShowMyBookingCheckbox(string actionId, bool initialValue);
    IReadOnlyList<IActionElement> GetAddBookingButton(PageContext pageContext);

    Task<IReadOnlyList<Block>> GetBookingCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        IReadOnlyList<Booking> bookings,
        IReadOnlyList<Booking> myBookings,
        PageContext pageContext,
        CancellationToken cancellationToken);

    public IReadOnlyList<Block> GetBookingCard(
        Workspace workspace,
        Booking booking,
        IReadOnlyList<Booking> myBookings,
        string loggedInCustomerId,
        bool includeActionButtons,
        PageContext pageContext);
}

public class BookingComponents(
    IWorkspaceMemberService sharedWorkspaceMemberService,
    ICustomerService customerService,
    Shared.Components.IBookingComponents bookingComponents)
    : IBookingComponents
{
    public Block GetOnlyShowMyBookingCheckbox(string actionId, bool initialValue)
    {
        var onlyShowMyBookingOption = new Option
        {
            Text = "Only show my bookings".ToPlainText(),
            Value = actionId,
        };
        return new ActionsBlock
        {
            Elements =
            [
                new CheckboxGroup
                {
                    ActionId = actionId,
                    Options = new List<Option>
                    {
                        onlyShowMyBookingOption,
                    },
                    InitialOptions = initialValue ? [onlyShowMyBookingOption] : [],
                },
            ],
        };
    }

    public IReadOnlyList<IActionElement> GetAddBookingButton(PageContext pageContext)
    {
        pageContext = pageContext.Clone();
        var context = new AddBookingContext(pageContext, null, null, null).Serialize();

        return
        [
            new Button
            {
                ActionId = BookingActionTypes.AddBooking,
                Text = "Make a booking".ToPlainTextWithIcon(Icons.New),
                Value = context,
            },
        ];
    }

    public async Task<IReadOnlyList<Block>> GetBookingCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        IReadOnlyList<Booking> bookings,
        IReadOnlyList<Booking> myBookings,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember.Id, cancellationToken);
        var blocks = new List<Block>();
        foreach (var booking in bookings)
        {
            blocks.AddRange(GetBookingCard(workspace, booking, myBookings, customer.Id, true, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    public IReadOnlyList<Block> GetBookingCard(
        Workspace workspace,
        Booking booking,
        IReadOnlyList<Booking> myBookings,
        string loggedInCustomerId,
        bool includeActionButtons,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock
            {
                Text = booking.From.ToShortDateWithoutYear().ToPlainTextWithIcon(Icons.Calendar),
            },
        };

        blocks.AddRange(booking.InvolvedCustomers.Select(item => new SectionBlock
        {
            Text = sharedWorkspaceMemberService
                .GetMentionedCustomerNameInSlackFormat(workspace, item.Identities.Select(identity => identity.Id).ToList(), item)
                .ToMarkdown(),
        }));

        if (!string.IsNullOrWhiteSpace(booking.Notes))
        {
            blocks.Add(new SectionBlock
            {
                Text = $"Notes: {booking.Notes}",
            });
        }

        blocks.AddRange(booking.InvolvedLocations.Select(item =>
            new SectionBlock
            {
                Text = item.Name.ToSafeString().ToPlainTextWithIcon(Icons.Location),
            }));

        blocks.AddRange(booking.InvolvedTeams.Select(item => new SectionBlock
        {
            Text = item.Name.ToSafeString().ToPlainTextWithIcon(Icons.Team),
        }));
        blocks.AddRange(bookingComponents.GetResourcesLines(booking));

        if (!includeActionButtons)
        {
            return blocks;
        }

        var buttons = new List<IActionElement>();
        if (booking.InvolvedCustomers.Select(item => item.Id).Contains(loggedInCustomerId))
        {
            buttons.Add(new Button
            {
                ActionId = BookingActionTypes.EditBooking,
                Text = "Edit".ToPlainTextWithIcon(Icons.Edit),
                Value = new EditBookingContext(pageContext.PushCurrentPageToVisitedPagesAndClone(), booking.Id).Serialize(),
            });

            buttons.Add(new Button
            {
                ActionId = BookingActionTypes.CancelBooking,
                Text = "Cancel".ToPlainTextWithIcon(Icons.Cancel),
                Value = new CancelBookingContext(pageContext, booking.Id).Serialize(),
            });
        }
        else
        {
            buttons.AddRange([
                new Button
                {
                    ActionId = BookingActionTypes.EditBooking,
                    Text = "Edit".ToPlainTextWithIcon(Icons.Edit),
                    Value = new EditBookingContext(pageContext.PushCurrentPageToVisitedPagesAndClone(), booking.Id).Serialize(),
                },
                new Button
                {
                    ActionId = BookingActionTypes.CancelBooking,
                    Text = "Cancel".ToPlainTextWithIcon(Icons.Cancel),
                    Value = new CancelBookingContext(pageContext, booking.Id).Serialize(),
                },
            ]);

            if (!myBookings.Any(item => item.From == booking.From && item.Until == booking.Until))
            {
                buttons.Add(new Button
                {
                    ActionId = BookingActionTypes.JoinBooking,
                    Text = "Join".ToPlainTextWithIcon(Icons.Join),
                    Value = new JoinBookingContext(pageContext, booking.Id).Serialize(),
                });
            }
        }

        if (buttons.Count != 0)
        {
            blocks.Add(new ActionsBlock
            {
                Elements = buttons,
            });
        }

        return blocks;
    }
}
