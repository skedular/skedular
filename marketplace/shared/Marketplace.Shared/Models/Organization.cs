using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public OrganizationType Type { get; set; }
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
