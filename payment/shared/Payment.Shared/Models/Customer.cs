using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public StripeCustomer? StripeCustomer { get; set; }
    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
