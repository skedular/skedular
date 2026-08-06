using Slack.Shared;
using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;

namespace Slack.Api;

public static class SlackClientExtensions
{
    extension(ISlackApiClient slackApiClient)
    {
        public async Task<ViewResponse> ViewsOpenAsync(string triggerId,
            ViewDefinition viewDefinition,
            CancellationToken cancellationToken) =>
            await slackApiClient.Views.Open(triggerId, viewDefinition, cancellationToken);

        public async Task ViewsPublishAsync(string userId,
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

    extension(WorkspaceMember workspaceMember)
    {
        public DayOfWeek ToDayOfWeek() =>
            workspaceMember.Locale switch
            {
                "en-US" => DayOfWeek.Sunday,
                _ => DayOfWeek.Monday,
            };
    }
}
