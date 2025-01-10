using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Workspace : ModelBaseWithDeleted
{
    public string Name { get; set; }
    public string? Domain { get; set; }
    public string? EmailDomain { get; set; }
    public string? EnterpriseId { get; set; }
    public string? EnterpriseName { get; set; }
    public string BotUserId { get; set; }
    public string BotUserScope { get; set; }
    public string BotUserAccessToken { get; set; }
    public string BotRefreshToken { get; set; }
    public string AuthedUserId { get; set; }
    public string AuthedUserScope { get; set; }
    public string AuthedUserAccessToken { get; set; }
    public string AuthedRefreshToken { get; set; }
    public DateTimeOffset? LastRefreshedAt { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public DateTimeOffset? ChannelsLastRefreshedAt { get; set; }
    public Organization Organization { get; set; }
    public ICollection<WorkspaceChannel> Channels { get; set; } = [];
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}
