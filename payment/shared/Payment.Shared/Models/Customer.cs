using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
