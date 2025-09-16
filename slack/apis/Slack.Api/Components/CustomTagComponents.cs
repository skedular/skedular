using Enterprise.Shared;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ICustomTagComponents
{
    Task<ICollection<IActionElement>> GetAddCustomTagButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetCustomTagCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationCustomTag> customTags,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class CustomTagComponents(ICustomerService customerService, IOrganizationPermissionsService organizationPermissionsService)
    : ICustomTagComponents
{
    public async Task<ICollection<IActionElement>> GetAddCustomTagButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var permissions = await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        if (!permissions.CanModify)
        {
            return [];
        }

        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var context = new AddCustomTagContext(pageContext).Serialize();

        return
        [
            new Button { ActionId = CustomTagActionTypes.AddCustomTag, Text = "Add Tag".ToPlainTextWithIcon(Icons.New), Value = context }
        ];
    }

    public async Task<ICollection<Block>> GetCustomTagCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationCustomTag> customTags,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember.Id, cancellationToken);
        var permissions = await organizationPermissionsService.GetPermissionsAsync(workspaceMember.Id, workspace.Organization.Id, cancellationToken);
        var blocks = new List<Block>();
        foreach (var customTag in customTags)
        {
            blocks.AddRange(GetCustomTagCard(customTag, customer, permissions.CanModify, permissions.CanDelete, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetCustomTagCard(
        OrganizationCustomTag customTag,
        Customer customer,
        bool canModify,
        bool canDelete,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {customTag.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Description*: {customTag.Description.ToSafeString()}".ToMarkdown() }
        };

        var buttons = new List<IActionElement>();

        if (customer.PreferredOrganizationTags.Any(item => item.Id == customTag.Id))
        {
            buttons.Add(new Button
            {
                ActionId = CustomTagActionTypes.RemovePreferredCustomTag,
                Text = "Remove preferred tag".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredCustomTagContext(pageContext, customTag.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = CustomTagActionTypes.SetPreferredCustomTag,
                Text = "Set preferred tag".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new SetPreferredCustomTagContext(pageContext, customTag.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = CustomTagActionTypes.ActionsMenu, Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto), Options = []
        };

        if (canModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{CustomTagActionTypes.EditCustomTag}{customTag.Id}", Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (canDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{CustomTagActionTypes.RemoveCustomTag}{customTag.Id}", Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove)
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
