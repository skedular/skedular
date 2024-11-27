using Enterprise.Shared;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface IDeskComponents
{
    Task<ICollection<IActionElement>> GetAddDeskButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<IActionElement>> GetBulkAddDesksButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetDeskCardsAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        ICollection<Desk> desks,
        ICollection<Booking> bookings,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class DeskComponents(ICustomerService customerService, ILocationService locationService) : IDeskComponents
{
    public async Task<ICollection<IActionElement>> GetAddDeskButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var permissions = await locationService.GetPermissionsAsync(locationId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            return [];
        }

        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var context = new AddDeskContext(pageContext, locationId).Serialize();

        return
        [
            new Button
            {
                ActionId = DeskActionTypes.AddDesk,
                Text = "Add Desk".ToPlainTextWithIcon(Icons.New),
                Value = context
            }
        ];
    }

    public async Task<ICollection<IActionElement>> GetBulkAddDesksButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var permissions = await locationService.GetPermissionsAsync(locationId, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            return [];
        }

        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var context = new BulkAddDesksContext(pageContext, locationId).Serialize();

        return
        [
            new Button
            {
                ActionId = DeskActionTypes.BulkAddDesks,
                Text = "Bulk Add Desks".ToPlainTextWithIcon(Icons.New),
                Value = context
            }
        ];
    }

    public async Task<ICollection<Block>> GetDeskCardsAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        ICollection<Desk> desks,
        ICollection<Booking> bookings,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var permissions = await locationService.GetPermissionsAsync(locationId, workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var desk in desks)
        {
            blocks.AddRange(GetDeskCard(desk, customer, bookings, permissions.CanModify, permissions.CanDelete,
                pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetDeskCard(
        Desk desk,
        Customer customer,
        ICollection<Booking> bookings,
        bool canModify,
        bool canDelete,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var bookingWithSameDesk =
            bookings.FirstOrDefault(item => item.Desks.Any(bookedDesk => bookedDesk.Id == desk.Id));
        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {desk.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Deactivated*: {desk.Deactivated}".ToMarkdown() },
            new SectionBlock { Text = $"*RequireBookingApproval*: {desk.RequireBookingApproval}".ToMarkdown() }
        };

        if (desk.Tags.Count != 0)
        {
            blocks.Add(new SectionBlock
            {
                Text = string.Join(
                        ", ",
                        desk.Tags.OrderBy(item => item.Name).Select(item => item.Name))
                    .ToMarkdownWithIcon(Icons.Zones)
            });
        }

        if (desk.OrganizationDeskTypes.Count != 0)
        {
            blocks.Add(new SectionBlock
            {
                Text = string.Join(
                        ", ",
                        desk.OrganizationDeskTypes.OrderBy(item => item.Name).Select(item => item.Name))
                    .ToMarkdownWithIcon(Icons.DeskTypes)
            });
        }

        if (desk.OrganizationZones.Count != 0)
        {
            blocks.Add(new SectionBlock
            {
                Text = string.Join(
                        ", ",
                        desk.OrganizationZones.OrderBy(item => item.Name).Select(item => item.Name))
                    .ToMarkdownWithIcon(Icons.Zones)
            });
        }

        blocks.Add(new SectionBlock
        {
            Text = (bookingWithSameDesk is null
                ? "Desk is *available*"
                : $"Desk booked by *{bookingWithSameDesk.Customer.GetCustomerName()}*").ToMarkdown()
        });

        var buttons = new List<IActionElement>();

        if (customer.PreferredDesks.Any(item => item.Id == desk.Id))
        {
            buttons.Add(new Button
            {
                ActionId = DeskActionTypes.RemovePreferredDesk,
                Text = "Remove preferred desk".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredDeskContext(pageContext, desk.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = DeskActionTypes.SetPreferredDesk,
                Text = "Set preferred desk".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new SetPreferredDeskContext(pageContext, desk.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = DeskActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options = []
        };

        if (canModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{DeskActionTypes.EditDesk}{desk.Id}", Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (canDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{DeskActionTypes.RemoveDesk}{desk.Id}",
                Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove)
            });
        }

        if (actionMenu.Options.Count != 0)
        {
            buttons.Add(actionMenu);
        }

        blocks.Add(new ActionsBlock { Elements = buttons });

        return blocks;
    }
}
