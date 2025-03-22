using Enterprise.Shared;
using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ITeamComponents
{
    Task<ICollection<IActionElement>> GetAddTeamButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<ICollection<Block>> GetTeamCardsAsync(
        WorkspaceMember workspaceMember,
        ICollection<Team> teams,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class TeamComponents(ICustomerService customerService, IOrganizationService organizationService)
    : ITeamComponents
{
    public async Task<ICollection<IActionElement>> GetAddTeamButtonAsync(
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
            new Button { ActionId = TeamActionTypes.AddTeam, Text = "Add Team".ToPlainTextWithIcon(Icons.New), Value = context }
        ];
    }

    public async Task<ICollection<Block>> GetTeamCardsAsync(
        WorkspaceMember workspaceMember,
        ICollection<Team> teams,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var customer = await customerService.GetAsync(workspaceMember, cancellationToken);
        var blocks = new List<Block>();
        foreach (var team in teams)
        {
            blocks.AddRange(GetTeamCard(team, customer, pageContext));
            blocks.Add(new DividerBlock());
        }

        return blocks.SkipLast(1).ToList();
    }

    private static List<Block> GetTeamCard(Team team, Customer customer, PageContext pageContext)
    {
        pageContext = pageContext.Clone();

        var dailyUpdateChannel =
            team.DailyUpdateChannel is null ? string.Empty : team.DailyUpdateChannel.Name.ToSafeString();
        var primaryLocation = team.PrimaryLocation is null ? "N/A" : team.PrimaryLocation.Name;
        var blocks = new List<Block>
        {
            new SectionBlock { Text = $"*Name*: {team.Name.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*About*: {team.About.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Timezone*: {team.Timezone.ToSafeString()}".ToMarkdown() },
            new SectionBlock { Text = $"*Daily update channel*: {dailyUpdateChannel}".ToMarkdown() },
            new SectionBlock { Text = $"*Primary Location*: {primaryLocation}".ToMarkdown() }
        };

        var buttons = new List<IActionElement>();

        if (customer.PreferredTeams.Any(item => item.Id == team.Id))
        {
            buttons.Add(new Button
            {
                ActionId = TeamActionTypes.RemovePreferredTeam,
                Text = "Remove preferred team".ToPlainTextWithIcon(Icons.ClearDefault),
                Value = new RemovePreferredTeamContext(pageContext, team.Id).Serialize()
            });
        }
        else
        {
            buttons.Add(new Button
            {
                ActionId = TeamActionTypes.AddAsPreferredTeam,
                Text = "Add as preferred team".ToPlainTextWithIcon(Icons.SetAsDefault),
                Value = new AddAsPreferredTeamContext(pageContext, team.Id).Serialize()
            });
        }

        var actionMenu = new StaticSelectMenu
        {
            ActionId = TeamActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options =
            [
                new Option { Value = $"{BookingActionTypes.Bookings}{team.Id}", Text = "Bookings".ToOptionPlainTextWithIcon(Icons.Bookings) }
            ]
        };

        if (team.Permissions.CanModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{TeamActionTypes.EditTeam}{team.Id}", Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit)
            });
        }

        if (team.Permissions.CanDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{TeamActionTypes.RemoveTeam}{team.Id}", Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove)
            });
        }

        buttons.Add(actionMenu);
        blocks.Add(new ActionsBlock { Elements = buttons });

        return blocks;
    }
}
