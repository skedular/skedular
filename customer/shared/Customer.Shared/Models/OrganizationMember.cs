using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public OrganizationMemberRole? Role { get; set; }
    public OrganizationMemberStatus Status { get; set; }
    public Organization Organization { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
}
