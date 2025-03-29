using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
