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

        IReadOnlyList<Block>[] blocks =
        [
            GetTitle(),
            GetToolbar(commonPageContext.PageContext, workspaceMember.Timezone),
        ];

        var slackApiClient = workspace.GetApiClient();
        await slackApiClient.ViewsPublishAsync(
            workspaceMember.Id,
            new HomeViewDefinition
            {
                CallbackId = BillingCallback,
                Blocks = [.. blocks.SelectMany(item => item.Count == 0 ? item : item.Append(new DividerBlock())).SkipLast(1)],
                PrivateMetadata = commonPageContext.Serialize(),
            },
            hash,
            cancellationToken);
    }

    public static void RegisterHandlers(AspNetSlackServiceConfiguration options) { }

    private static IReadOnlyList<Block> GetTitle() =>
    [
        new SectionBlock
        {
            Text = "*Billing*".ToMarkdown(),
        },
    ];

    private IReadOnlyList<Block> GetToolbar(PageContext pageContext, string timezone)
    {
        var homeAndBackButtons = commonComponents.GetHomeAndBackButtons(pageContext, timezone);
        var feedbackButton = commonComponents.GetFeedbackButton(pageContext);

        return
        [
            new ActionsBlock
            {
                Elements =
                [
                    .. homeAndBackButtons,
                    .. feedbackButton,
                ],
            },
        ];
    }
}
