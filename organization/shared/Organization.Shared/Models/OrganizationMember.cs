using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationMember : ModelBaseWithDeleted
{
    public OrganizationMembershipType MembershipType { get; set; } = OrganizationMembershipType.Member;
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
    public ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
