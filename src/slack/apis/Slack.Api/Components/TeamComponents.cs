using Enterprise.Shared;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using Slack.Shared.Models;
using Slack.Shared.Services.CrossDomains;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ITeamComponents
{
    Task<IReadOnlyList<IActionElement>> GetAddTeamButtonAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        PageContext pageContext,
        CancellationToken cancellationToken);

    Task<IReadOnlyList<Block>> GetTeamCardsAsync(
        WorkspaceMember workspaceMember,
        IReadOnlyList<Team> teams,
        PageContext pageContext,
        CancellationToken cancellationToken);
}

public class TeamComponents(IOrganizationPermissionsService organizationPermissionsService, ITeamPermissionsService teamPermissionsService)
    : ITeamComponents
{
    public async Task<IReadOnlyList<IActionElement>> GetAddTeamButtonAsync(
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
        var context = new CommonPageContext(pageContext).Serialize();

        return
        [
            new Button
            {
                ActionId = TeamActionTypes.AddTeam,
                Text = "Add Team".ToPlainTextWithIcon(Icons.New),
                Value = context,
            },
        ];
    }

    public async Task<IReadOnlyList<Block>> GetTeamCardsAsync(
        WorkspaceMember workspaceMember,
        IReadOnlyList<Team> teams,
        PageContext pageContext,
        CancellationToken cancellationToken)
    {
        var blocks = new List<Block>();
        foreach (var team in teams)
        {
            blocks.AddRange(await GetTeamCardAsync(workspaceMember, team, cancellationToken));
            blocks.Add(new DividerBlock());
        }

        return [.. blocks.SkipLast(1)];
    }

    private async Task<List<Block>> GetTeamCardAsync(WorkspaceMember workspaceMember, Team team, CancellationToken cancellationToken)
    {
        var dailyUpdateChannel = team.DailyUpdateChannel is null ? string.Empty : team.DailyUpdateChannel.Name.ToSafeString();
        var permissions = await teamPermissionsService.GetPermissionsAsync(workspaceMember.Id, team.Id, cancellationToken);
        var primaryLocation = team.PrimaryLocation is null ? "N/A" : team.PrimaryLocation.Name;
        var blocks = new List<Block>
        {
            new SectionBlock
            {
                Text = $"*Name*: {team.Name.ToSafeString()}".ToMarkdown(),
            },
            new SectionBlock
            {
                Text = $"*About*: {team.About.ToSafeString()}".ToMarkdown(),
            },
            new SectionBlock
            {
                Text = $"*Timezone*: {team.Timezone.ToSafeString()}".ToMarkdown(),
            },
            new SectionBlock
            {
                Text = $"*Daily update channel*: {dailyUpdateChannel}".ToMarkdown(),
            },
            new SectionBlock
            {
                Text = $"*Primary Location*: {primaryLocation}".ToMarkdown(),
            },
        };

        var buttons = new List<IActionElement>();
        var actionMenu = new StaticSelectMenu
        {
            ActionId = TeamActionTypes.ActionsMenu,
            Placeholder = "Go to...".ToPlainTextWithIcon(Icons.Goto),
            Options =
            [
                new Option
                {
                    Value = $"{BookingActionTypes.Bookings}{team.Id}",
                    Text = "Bookings".ToOptionPlainTextWithIcon(Icons.Bookings),
                },
            ],
        };

        if (permissions.CanModify)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{TeamActionTypes.EditTeam}{team.Id}",
                Text = "Edit".ToOptionPlainTextWithIcon(Icons.Edit),
            });
        }

        if (permissions.CanDelete)
        {
            actionMenu.Options.Add(new Option
            {
                Value = $"{TeamActionTypes.RemoveTeam}{team.Id}",
                Text = "Remove".ToOptionPlainTextWithIcon(Icons.Remove),
            });
        }

        buttons.Add(actionMenu);
        blocks.Add(new ActionsBlock
        {
            Elements = buttons,
        });

        return blocks;
    }
}
