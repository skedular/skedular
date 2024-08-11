using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Tenant : ModelBaseWithDeleted
{
    public string? Name { get; set; }
    public DateTimeOffset? EntitiesLastRefreshedAt { get; set; }
    
    public ICollection<TenantMember> TenantMembers { get; set; } = [];
}
