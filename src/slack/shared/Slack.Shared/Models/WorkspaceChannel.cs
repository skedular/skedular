using Enterprise.Shared.Models;

namespace Slack.Shared.Models;

public class WorkspaceChannel : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string Topic { get; set; } = string.Empty;
    public string Purpose { get; set; } = string.Empty;
    public bool IsPrivate { get; set; }
    public bool IsGeneral { get; set; }
    public bool IsGroup { get; set; }
    public bool IsShared { get; set; }
    public bool IsMember { get; set; }
    public Workspace Workspace { get; set; } = new();
    public IReadOnlyList<Organization> OrganizationDailyUpdateChannels { get; set; } = [];
    public IReadOnlyList<Location> LocationDailyUpdateChannels { get; set; } = [];
    public IReadOnlyList<Team> TeamDailyUpdateChannels { get; set; } = [];
}
