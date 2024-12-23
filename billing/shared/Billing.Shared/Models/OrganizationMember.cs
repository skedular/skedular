using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public string? MembershipType { get; set; }
    public string Status { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
