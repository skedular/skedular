using Slack.Api.Components;
using Slack.Shared;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using Workspace = Slack.Shared.Models.Workspace;
using WorkspaceMember = Slack.Shared.Models.WorkspaceMember;

namespace Slack.Api.Pages;

public interface IBillingPage
{
    Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken);
}

public class BillingPage(ICommonComponents commonComponents) : IBillingPage
{
    private const string BillingCallback = "Billing";

    public async Task RenderWithContextAsync(
        Workspace workspace,
        WorkspaceMember workspaceMember,
        CommonPageContext commonPageContext,
        string? hash,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(commonPageContext.PageContext.BillingPage);

        commonPageContext.PageContext.CurrentPageType = PageType.Billing;

        ICollection<Block>[] blocks =
        [
            GetTitle(),
            GetToolbar(commonPageContext.PageContext)
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.Views.PublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = BillingCallback,
                Blocks = blocks
                    .SelectMany(item => item.Count == 0 ? item : item.Concat([new DividerBlock()]))
                    .SkipLast(1)
                    .ToList(),
                PrivateMetadata = commonPageContext.Serialize()
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) { }

    private static ICollection<Block> GetTitle() =>
    [
        new SectionBlock { Text = "*Billing*".ToMarkdown() }
    ];

    private ICollection<Block> GetToolbar(PageContext pageContext)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements = new List<IActionElement>()
                    .Concat(homeAndBackButtons)
                    .Concat(feedbackButton)
                    .ToList()
            }
        ];
    }
}
