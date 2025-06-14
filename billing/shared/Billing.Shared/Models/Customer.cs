using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
