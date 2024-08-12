using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Tenant : ModelBaseWithDeleted
{
    public string? Name { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public string InstalledByUserId { get; set; } = string.Empty;
    public ICollection<TenantMember> TenantMembers { get; set; } = [];
    public Organization Organization { get; set; }
}
