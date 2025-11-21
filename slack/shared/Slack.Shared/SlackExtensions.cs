using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;

namespace Slack.Shared;

public static class SlackExtensions
{
    extension(User user)
    {
        public bool IsAcceptableWorkspaceMemberType() =>
            user.Id != "USLACKBOT" && user is
                { IsBot: false, IsStranger: false, IsRestricted: false, IsUltraRestricted: false, Deleted: false };
    }

    extension(Workspace workspace)
    {
        public ISlackApiClient GetUserApiClient() =>
            new SlackServiceBuilder()
                .UseApiToken(workspace.AuthedUserAccessToken)
                .GetApiClient();

        public ISlackApiClient GetApiClient() =>
            new SlackServiceBuilder()
                .UseApiToken(workspace.BotUserAccessToken)
                .GetApiClient();
    }

    extension(Database.Entities.Workspace workspace)
    {
        public ISlackApiClient GetUserApiClient() =>
            new SlackServiceBuilder()
                .UseApiToken(workspace.AuthedUserAccessToken)
                .GetApiClient();

        public ISlackApiClient GetApiClient() =>
            new SlackServiceBuilder()
                .UseApiToken(workspace.BotUserAccessToken)
                .GetApiClient();
    }

    extension(IViewsApi view)
    {
        public async Task PublishAsync(string userId,
            HomeViewDefinition viewDefinition,
            string? hash,
            CancellationToken cancellationToken)
        {
            try
            {
                await view.Publish(userId, viewDefinition, hash, cancellationToken);
            }
            catch (SlackException ex) when (ex.ErrorMessages.Any(errorMessage => errorMessage.Contains("hash_conflict")) ||
                                            ex.ErrorCode.Contains("hash_conflict"))
            {
                await view.Publish(userId, viewDefinition, null, cancellationToken);
            }
        }
    }
}
