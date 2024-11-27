using Enterprise.Shared;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface IDeskTypeComponents
{
    Task<ICollection<IActionElement>> GetAddDeskTypeButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetDeskTypeCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationDeskType> deskTypes,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class DeskTypeComponents(ICustomerService customerService, IOrganizationService organizationService)
    : IDeskTypeComponents
{
    public async Task<ICollection<IActionElement>> GetAddDeskTypeButtonAsync(
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
        var context = new AddDeskTypeContext(pageContext).Serialize();

        return
        [
            new Button
            {
                ActionId = DeskTypeActionTypes.AddDeskType,
                Text = "Add Desk Type".ToPlainTextWithIcon(Icons.New),
                Value = context
            }
        ];
    }

    public async Task<ICollection<Block>> GetDeskTypeCardsAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        ICollection<OrganizationDeskType> deskTypes,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var permissions = await organizationService.GetPermissionsAsync(workspace, workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var deskType in deskTypes)
        {
            blocks.AddRange(
                GetDeskTypeCard(deskType, customer, permissions.CanModify, permissions.CanDelete, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetDeskTypeCard(
        OrganizationDeskType deskType,
        Customer customer,
        bool canModify,
        bool canDelete,
        PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {deskType.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Description*: {deskType.Description.ToSafeString()}".ToMarkdown() }
        };

        var buttons = new List<IActionElement>();

        if (customer.PreferredOrganizationTags.Any(item => item.Id == deskType.Id))
        {
            buttons.Add(new Button
            {
                ActionId = DeskTypeActionTypes.RemovePreferredDeskType,
                Text = "Remove preferred desk type".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredDeskTypeContext(pageContext, deskType.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = DeskTypeActionTypes.SetPreferredDeskType,
                Text = "Set preferred desk type".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new SetPreferredDeskTypeContext(pageContext, deskType.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = DeskTypeActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options = []
        };

        if (canModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{DeskTypeActionTypes.EditDeskType}{deskType.Id}",
                Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (canDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{DeskTypeActionTypes.RemoveDeskType}{deskType.Id}",
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
