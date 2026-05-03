using Slack.Api.Services;
using Slack.Shared.Constants;
using Slack.Shared.Context;
using SlackNet.Blocks;

namespace Slack.Api.Components;

public interface ICommonComponents
{
    IReadOnlyList<IActionElement> GetHomeAndBackButtons(PageContext pageContext, string timezone);
    IReadOnlyList<IActionElement> GetFeedbackButton(PageContext pageContext);
    IReadOnlyList<IActionElement> GetBackButton(PageContext pageContext);
}

public class CommonComponents(IHomePageContextService homePageContextService) : ICommonComponents
{
    public IReadOnlyList<IActionElement> GetHomeAndBackButtons(PageContext pageContext, string timezone) =>
        GetHomeButtons(pageContext).Concat(GetBackButton(pageContext)).ToList();

    public IReadOnlyList<IActionElement> GetFeedbackButton(PageContext pageContext)
    {
        pageContext = pageContext.Clone();
        var context = new CommonPageContext(pageContext).Serialize();

        return
        [
            new Button { ActionId = CommonActionTypes.SendUsFeedback, Text = "Send us feedback".ToPlainTextWithIcon(Icons.Feedback), Value = context }
        ];
    }

    public IReadOnlyList<IActionElement> GetBackButton(PageContext pageContext)
    {
        if (pageContext.VisitedPagesHistory.Count == 0)
        {
            return [];
        }

        pageContext = pageContext.Clone();
        var context = new CommonPageContext(pageContext).Serialize();

        return [new Button { ActionId = CommonActionTypes.Back, Text = "Back".ToPlainTextWithIcon(Icons.Back), Value = context }];
    }

    private IReadOnlyList<IActionElement> GetHomeButtons(PageContext pageContext)
    {
        pageContext = pageContext.PushCurrentPageToVisitedPagesAndClone();
        pageContext.HomePage ??= homePageContextService.GetDefaultHomePageContext();
        var context = new CommonPageContext(pageContext).Serialize();

        return [new Button { ActionId = HomeActionTypes.Home, Text = Icons.Home, Value = context }];
    }
}
