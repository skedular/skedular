using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class WorkspaceMember : ModelBaseWithDeleted
{
    public string Email { get; set; }
    public string Designation { get; set; }
    public string Name { get; set; }
    public string GivenName { get; set; }
    public string FamilyName { get; set; }
    public string Timezone { get; set; }
    public bool IsAdmin { get; set; }
    public bool IsOwner { get; set; }
    public bool IsPrimaryOwner { get; set; }
    public string Locale { get; set; }
    public string? PhotoUrl { get; set; }
    public string? PhotoUrl24 { get; set; }
    public string? PhotoUrl32 { get; set; }
    public string? PhotoUrl48 { get; set; }
    public string? PhotoUrl72 { get; set; }
    public string? PhotoUrl192 { get; set; }
    public string? PhotoUrl512 { get; set; }
    public DateTimeOffset? LastProfileStatusUpdatedAt { get; set; }
    public bool? AutomaticallyUpdateProfileStatus { get; set; }
    public Workspace Workspace { get; set; }
}
