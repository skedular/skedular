using Enterprise.Shared.Models;

namespace MsTeams.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<AzureTenant> AzureTenants { get; set; } = [];
}
