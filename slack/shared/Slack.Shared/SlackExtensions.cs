using Slack.Shared.Models;
using SlackNet;

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
}
