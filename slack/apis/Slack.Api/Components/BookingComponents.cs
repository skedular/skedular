using Enterprise.Shared;
using Enterprise.Shared.Time;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;
using IWorkspaceMemberService = Slack.Shared.Services.IWorkspaceMemberService;

namespace Slack.Api.Components;

public interface IBookingComponents
{
    Block GetOnlyShowMyBookingCheckbox(string actionId, bool initialValue);

    ICollection<IActionElement> GetAddBookingButton(string? locationId, string? teamId, PageContext pageContext);

    Task<ICollection<Block>> GetBookingCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<Booking> bookings,
        ICollection<Booking> myBookings,
        bool canUpdateBookingOnBehalf,
        bool canDeleteBookingOnBehalf,
        PageContext pageContext,
        CancellationToken cancellationToken);

    public ICollection<Block> GetBookingCard(
        Workspace workspace,
        Booking booking,
        ICollection<Booking> myBookings,
        Customer customer,
        bool canUpdateBookingOnBehalf,
        bool canDeleteBookingOnBehalf,
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
        var onlyShowMyBookingOption = new Option { Text = "Only show my bookings".ToPlainText(), Value = actionId };
        return new ActionsBlock
        {
            Elements =
            [
                new CheckboxGroup
                {
                    ActionId = actionId,
                    Options = new List<Option> { onlyShowMyBookingOption },
                    InitialOptions = initialValue ? [onlyShowMyBookingOption] : []
                }
            ]
        };
    }

    public ICollection<IActionElement> GetAddBookingButton(string? locationId, string? teamId, PageContext pageContext)
    {
        pageContext = pageContext.Clone();
        var context = new AddBookingContext(pageContext, null, null, locationId, teamId).Serialize();

        return
        [
            new Button { ActionId = BookingActionTypes.AddBooking, Text = "Make a booking".ToPlainTextWithIcon(Icons.New), Value = context }
        ];
    }

    public async Task<ICollection<Block>> GetBookingCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<Booking> bookings,
        ICollection<Booking> myBookings,
        bool canUpdateBookingOnBehalf,
        bool canDeleteBookingOnBehalf,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var booking in bookings)
        {
            blocks.AddRange(
                GetBookingCard(
                    workspace,
                    booking,
                    myBookings,
                    customer,
                    canUpdateBookingOnBehalf,
                    canDeleteBookingOnBehalf,
                    true,
                    pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    public ICollection<Block> GetBookingCard(
        Workspace workspace,
        Booking booking,
        ICollection<Booking> myBookings,
        Customer customer,
        bool canUpdateBookingOnBehalf,
        bool canDeleteBookingOnBehalf,
        bool includeActionButtons,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock { Text = booking.From.ToShortDateWithoutYear().ToPlainTextWithIcon(Icons.Calendar) },
            new SectionBlock
            {
                Text = sharedWorkspaceMemberService.GetMentionedCustomerNameInSlackFormat(
                    workspace,
                    booking.Customer.Identities.Select(item => item.Id).ToList(),
                    booking.Customer).ToMarkdown()
            }
        };

        if (!string.IsNullOrWhiteSpace(booking.Notes))
        {
            blocks.Add(new SectionBlock { Text = $"Notes: {booking.Notes}" });
        }

        if (!string.IsNullOrWhiteSpace(booking.Location?.Id))
        {
            blocks.Add(new SectionBlock { Text = booking.Location.Name.ToSafeString().ToPlainTextWithIcon(Icons.Location) });
        }

        if (!string.IsNullOrWhiteSpace(booking.Team?.Id))
        {
            blocks.Add(new SectionBlock { Text = booking.Team.Name.ToSafeString().ToPlainTextWithIcon(Icons.Team) });
        }

        blocks.AddRange(bookingComponents.GetDesksLines(booking));

        if (!includeActionButtons)
        {
            return blocks;
        }

        var buttons = new List<IActionElement>();
        if (booking.Customer.Id == customer.Id)
        {
            buttons.Add(new Button
            {
                ActionId = BookingActionTypes.EditBooking,
                Text = "Edit".ToPlainTextWithIcon(Icons.Edit),
                Value = new EditBookingContext(
                        pageContext.PushCurrentPageToVisitedPagesAndClone(),
                        booking.Id)
                    .Serialize()
            });

            buttons.Add(new Button
            {
                ActionId = BookingActionTypes.CancelBooking,
                Text = "Cancel".ToPlainTextWithIcon(Icons.Cancel),
                Value = new CancelBookingContext(pageContext, booking.Id).Serialize()
            });
        }
        else
        {
            if (canUpdateBookingOnBehalf)
            {
                buttons.Add(new Button
                {
                    ActionId = BookingActionTypes.EditBooking,
                    Text = "Edit".ToPlainTextWithIcon(Icons.Edit),
                    Value = new EditBookingContext(
                            pageContext.PushCurrentPageToVisitedPagesAndClone(),
                            booking.Id)
                        .Serialize()
                });
            }

            if (canDeleteBookingOnBehalf)
            {
                buttons.Add(new Button
                {
                    ActionId = BookingActionTypes.CancelBooking,
                    Text = "Cancel".ToPlainTextWithIcon(Icons.Cancel),
                    Value = new CancelBookingContext(pageContext, booking.Id).Serialize()
                });
            }

            if (!myBookings.Any(item => item.From == booking.From && item.To == booking.To))
            {
                buttons.Add(new Button
                {
                    ActionId = BookingActionTypes.JoinBooking,
                    Text = "Join".ToPlainTextWithIcon(Icons.Join),
                    Value = new JoinBookingContext(pageContext, booking.Id).Serialize()
                });
            }
        }

        if (buttons.Count != 0)
        {
            blocks.Add(new ActionsBlock { Elements = buttons });
        }

        return blocks;
    }
}
