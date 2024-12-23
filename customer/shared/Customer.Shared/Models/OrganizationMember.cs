using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public string? MembershipType { get; set; }
    public string Status { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
