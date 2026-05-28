using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Team.Shared.Models;

public class Team : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public string? About { get; set; }
    public string? Timezone { get; set; }
    public IReadOnlyList<CdnImageFile> FeatureImages { get; set; } = [];
    public Organization Organization { get; set; } = new();
    public Location? PrimaryLocation { get; set; }
    public IReadOnlyList<TeamMember> TeamMembers { get; set; } = [];
    public IReadOnlyList<JoinInvitation> JoinInvitations { get; set; } = [];
    public Permissions Permissions { get; set; } = new();
}
