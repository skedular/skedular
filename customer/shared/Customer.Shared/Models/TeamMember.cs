using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class TeamMember : ReplicatedModelBaseWithDeleted
{
    public string? MembershipType { get; set; }
    public Team? Team { get; set; }
    public Customer Customer { get; set; }
    public OrganizationMember? OrganizationMember { get; set; }
}
