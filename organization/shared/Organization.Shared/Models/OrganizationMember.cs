using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationMember : ModelBaseWithDeleted
{
    public OldOrganizationMembershipType MembershipType { get; set; } = OldOrganizationMembershipType.Member;
    public bool? IsOrganizationOnboardingDone { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
    public ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
