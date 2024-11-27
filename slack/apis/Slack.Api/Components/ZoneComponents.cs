using Enterprise.Shared;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface IZoneComponents
{
    Task<ICollection<IActionElement>> GetAddZoneButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetZoneCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationZone> zones,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class ZoneComponents(ICustomerService customerService, IOrganizationService organizationService)
    : IZoneComponents
{
    public async Task<ICollection<IActionElement>> GetAddZoneButtonAsync(
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
        var context = new AddZoneContext(pageContext).Serialize();

        return
        [
            new Button
            {
                ActionId = ZoneActionTypes.AddZone,
                Text = "Add Zone".ToPlainTextWithIcon(Icons.New),
                Value = context
            }
        ];
    }

    public async Task<ICollection<Block>> GetZoneCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationZone> zones,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var permissions = await organizationService.GetPermissionsAsync(workspace, workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var zone in zones)
        {
            blocks.AddRange(
                GetZoneCard(zone, customer, permissions.CanModify, permissions.CanDelete, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetZoneCard(
        OrganizationZone zone,
        Customer customer,
        bool canModify,
        bool canDelete,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {zone.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Description*: {zone.Description.ToSafeString()}".ToMarkdown() }
        };

        var buttons = new List<IActionElement>();

        if (customer.PreferredOrganizationTags.Any(item => item.Id == zone.Id))
        {
            buttons.Add(new Button
            {
                ActionId = ZoneActionTypes.RemovePreferredZone,
                Text = "Remove preferred zone".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredZoneContext(pageContext, zone.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = ZoneActionTypes.SetPreferredZone,
                Text = "Set preferred zone".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new SetPreferredZoneContext(pageContext, zone.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = ZoneActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options = []
        };

        if (canModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{ZoneActionTypes.EditZone}{zone.Id}",
                Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (canDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{ZoneActionTypes.RemoveZone}{zone.Id}",
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
