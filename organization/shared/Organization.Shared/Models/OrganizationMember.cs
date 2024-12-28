using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationMember : ModelBaseWithDeleted
{
    public OrganizationMembershipType MembershipType { get; set; }
    public OrganizationMemberStatus Status { get; set; }
    public bool? IsOrganizationOnboardingDone { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
    public ICollection<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
