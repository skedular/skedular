using Enterprise.Shared;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface IResourceComponents
{
    Task<ICollection<IActionElement>> GetAddResourceButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetResourceCardsAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        ICollection<Resource> resources,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class ResourceComponents(ICustomerService customerService, ILocationPermissionsService locationPermissionsService) : IResourceComponents
{
    public async Task<ICollection<IActionElement>> GetAddResourceButtonAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, locationId, cancellationToken);
        if (!permissions.CanModify)
        {
            return [];
        }

        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        var context = new AddResourceContext(pageContext, locationId).Serialize();

        return
        [
            new Button { ActionId = ResourceActionTypes.AddResource, Text = "Add Resource".ToPlainTextWithIcon(Icons.New), Value = context }
        ];
    }

    public async Task<ICollection<Block>> GetResourceCardsAsync(
        string locationId,
        WorkspaceMember workspaceMember,
        ICollection<Resource> resources,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember.Id, cancellationToken);
        var permissions = await locationPermissionsService.GetPermissionsAsync(workspaceMember.Id, locationId, cancellationToken);
        var blocks = new List<Block>();
        foreach (var resource in resources)
        {
            blocks.AddRange(GetResourceCard(resource, customer, permissions.CanModify, permissions.CanDelete, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetResourceCard(Resource resource, Customer customer, bool canModify, bool canDelete, PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Type*: {resource.ResourceType.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Name*: {resource.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Inactive*: {resource.Inactive}".ToMarkdown() },
            new SectionBlock { Text = $"*Capacity*: {resource.Capacity}".ToMarkdown() },
            new SectionBlock { Text = $"*RequireBookingApproval*: {resource.RequireBookingApproval}".ToMarkdown() }
        };

        if (resource.OrganizationCustomTags.Count != 0)
        {
            blocks.Add(new SectionBlock
            {
                Text = string
                    .Join(", ", resource.OrganizationCustomTags.OrderBy(item => item.Name).Select(item => item.Name))
                    .ToMarkdownWithIcon(Icons.CustomTags)
            });
        }

        if (resource.OrganizationZones.Count != 0)
        {
            blocks.Add(new SectionBlock
            {
                Text = string
                    .Join(", ", resource.OrganizationZones.OrderBy(item => item.Name).Select(item => item.Name))
                    .ToMarkdownWithIcon(Icons.Zones)
            });
        }

        var buttons = new List<IActionElement>();

        if (customer.PreferredResources.Any(item => item.Id == resource.Id))
        {
            buttons.Add(new Button
            {
                ActionId = ResourceActionTypes.RemovePreferredResource,
                Text = "Remove preferred resource".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredResourceContext(pageContext, resource.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = ResourceActionTypes.SetPreferredResource,
                Text = "Set preferred resource".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new SetPreferredResourceContext(pageContext, resource.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = ResourceActionTypes.ActionsMenu, Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto), Options = []
        };

        if (canModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{ResourceActionTypes.EditResource}{resource.Id}", Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (canDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{ResourceActionTypes.RemoveResource}{resource.Id}", Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove)
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
