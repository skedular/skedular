using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public Organization? Organization { get; set; }
    public Location? PrimaryLocation { get; set; }
    public DateTimeOffset? SlackChannelDailyUpdateLastSentAt { get; set; }
    public WorkspaceChannel? DailyUpdateChannel { get; set; }
    public TeamPermissions Permissions { get; set; } = new();
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
