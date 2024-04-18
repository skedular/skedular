using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
