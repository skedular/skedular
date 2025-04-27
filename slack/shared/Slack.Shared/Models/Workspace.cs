using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Workspace : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? Domain { get; set; }
    public string? EmailDomain { get; set; }
    public string? EnterpriseId { get; set; }
    public string? EnterpriseName { get; set; }
    public string BotUserId { get; set; } = string.Empty;
    public string BotUserScope { get; set; } = string.Empty;
    public string BotUserAccessToken { get; set; } = string.Empty;
    public string BotRefreshToken { get; set; } = string.Empty;
    public string AuthedUserId { get; set; } = string.Empty;
    public string AuthedUserScope { get; set; } = string.Empty;
    public string AuthedUserAccessToken { get; set; } = string.Empty;
    public string AuthedRefreshToken { get; set; } = string.Empty;
    public DateTimeOffset? LastRefreshedAt { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public DateTimeOffset? ChannelsLastRefreshedAt { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<WorkspaceChannel> Channels { get; set; } = [];
    public ICollection<WorkspaceMember> WorkspaceMembers { get; set; } = [];
}
