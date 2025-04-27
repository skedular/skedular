using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class AzureTenant : ModelBaseWithDeleted
{
    public string? Name { get; set; }
    public DateTimeOffset? MembersLastRefreshedAt { get; set; }
    public string InstalledByUserId { get; set; } = string.Empty;
    public ICollection<AzureTenantMember> AzureTenantMembers { get; set; } = [];
    public Organization Organization { get; set; } = new();
}
