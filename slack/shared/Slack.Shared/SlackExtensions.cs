using Slack.Shared.Models;
using SlackNet;
using SlackNet.WebApi;

namespace Slack.Shared;

public static class SlackExtensions
{
    public static bool IsAcceptableWorkspaceMemberType(this User user) =>
        user.Id != "USLACKBOT" && user is
            { IsBot: false, IsStranger: false, IsRestricted: false, IsUltraRestricted: false, Deleted: false };

    public static ISlackApiClient GetUserApiClient(this Workspace workspace) =>
        new SlackServiceBuilder()
            .UseApiToken(workspace.AuthedUserAccessToken)
            .GetApiClient();

    public static ISlackApiClient GetUserApiClient(this Database.Entities.Workspace workspace) =>
        new SlackServiceBuilder()
            .UseApiToken(workspace.AuthedUserAccessToken)
            .GetApiClient();

    public static ISlackApiClient GetApiClient(this Workspace workspace) =>
        new SlackServiceBuilder()
            .UseApiToken(workspace.BotUserAccessToken)
            .GetApiClient();

    public static ISlackApiClient GetApiClient(this Database.Entities.Workspace workspace) =>
        new SlackServiceBuilder()
            .UseApiToken(workspace.BotUserAccessToken)
            .GetApiClient();

    public static async Task PublishAsync(
        this IViewsApi view,
        string userId,
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
