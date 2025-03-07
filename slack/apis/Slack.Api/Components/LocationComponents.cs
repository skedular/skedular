using Enterprise.Shared;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ILocationComponents
{
    Task<ICollection<IActionElement>> GetAddLocationButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetLocationCardsAsync(
        WorkspaceMember workspaceMember,
        ICollection<Location> locations,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class LocationComponents(ICustomerService customerService, IOrganizationService organizationService)
    : ILocationComponents
{
    public async Task<ICollection<IActionElement>> GetAddLocationButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var permissions = await organizationService.GetPermissionsAsync(workspace, workspaceMember, cancellationToken);
        if (!permissions.CanModify)
        {
            return [];
        }

        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var context = new CommonPageContext(pageContext).Serialize();

        return
        [
            new Button { ActionId = LocationActionTypes.AddLocation, Text = "Add Location".ToPlainTextWithIcon(Icons.New), Value = context }
        ];
    }

    public async Task<ICollection<Block>> GetLocationCardsAsync(
        WorkspaceMember workspaceMember,
        ICollection<Location> locations,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var location in locations)
        {
            blocks.AddRange(GetLocationCard(location, customer, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetLocationCard(
        Location location,
        Customer customer,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var dailyUpdateChannel =
            location.DailyUpdateChannel is null ? string.Empty : location.DailyUpdateChannel.Name.ToSafeString();
        var deskCapacity = location.Desks.Count == 0 ? "No desk available" : location.Desks.Count.ToString();
        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {location.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*About*: {location.About.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Timezone*: {location.Timezone.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Desk capacity*: {deskCapacity}".ToMarkdown() },
            new SectionBlock { Text = $"*Daily update channel*: {dailyUpdateChannel}".ToMarkdown() }
        };

        var buttons = new List<IActionElement>();

        if (customer.PreferredLocations.Any(item => item.Id == location.Id))
        {
            buttons.Add(new Button
            {
                ActionId = LocationActionTypes.RemovePreferredLocation,
                Text = "Remove preferred location".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new ClearPreferredLocationContext(pageContext, location.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = LocationActionTypes.AddAsPreferredLocation,
                Text = "Add as preferred location".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new AddAsPreferredLocationContext(pageContext, location.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = LocationActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options =
            [
                new Option { Value = $"{BookingActionTypes.Bookings}{location.Id}", Text = "Bookings".ToOptionPlainTextWithIcon(Icons.Bookings) },
                new Option { Value = $"{ZoneActionTypes.Zones}{location.Id}", Text = "Zones".ToOptionPlainTextWithIcon(Icons.Zones) },
                new Option { Value = $"{CustomTagActionTypes.CustomTags}{location.Id}", Text = "Tags".ToOptionPlainTextWithIcon(Icons.CustomTags) },
                new Option { Value = $"{DeskActionTypes.Desks}{location.Id}", Text = "Desks".ToOptionPlainTextWithIcon(Icons.Desks) }
            ]
        };

        if (location.Permissions.CanModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{LocationActionTypes.EditLocation}{location.Id}", Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (location.Permissions.CanDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{LocationActionTypes.RemoveLocation}{location.Id}", Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove)
            });
        }

        buttons.Add(actionMenu);
        blocks.Add(new ActionsBlock { Elements = buttons });

        return blocks;
    }
}
