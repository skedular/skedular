using Slack.Shared;
using SlackNet;
using SlackNet.WebApi;

namespace Slack.Api;

public static class SlackClientExtensions
{
    public static async Task<ViewResponse> ViewsOpenAsync(
        this ISlackApiClient slackApiClient,
        string triggerId,
        ViewDefinition viewDefinition,
        CancellationToken cancellationToken)
    {
        try
        {
            return await slackApiClient.Views.Open(triggerId, viewDefinition, cancellationToken);
        }
        catch (SlackException)
        {
            return await slackApiClient.Views.Open(string.Empty, viewDefinition, cancellationToken);
        }
    }
    
    public static async Task ViewsPublishAsync(
        this ISlackApiClient slackApiClient,
        string userId,
        HomeViewDefinition viewDefinition,
        string? hash,
        CancellationToken cancellationToken)
    {
        try
        {
            await slackApiClient.Views.PublishAsync(userId, viewDefinition, hash, cancellationToken);
        }
        catch (SlackException)
        {
            await slackApiClient.Views.PublishAsync(userId, viewDefinition, null, cancellationToken);
        }
    }
}
