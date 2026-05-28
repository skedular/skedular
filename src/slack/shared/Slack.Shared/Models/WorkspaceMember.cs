using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class WorkspaceMember : ModelBaseWithDeleted
{
    public string Email { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string GivenName { get; set; } = string.Empty;
    public string FamilyName { get; set; } = string.Empty;
    public string Timezone { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; }
    public bool IsPrimaryOwner { get; set; }
    public string Locale { get; set; } = string.Empty;
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }
    public DateTimeOffset? LastProfileStatusUpdatedAt { get; set; }
    public bool? AutomaticallyUpdateProfileStatus { get; set; }
    public Workspace Workspace { get; set; } = new();
}
