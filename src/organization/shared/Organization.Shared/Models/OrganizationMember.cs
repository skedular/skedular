using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationMember : ModelBaseWithDeleted
{
    public OrganizationMemberRole Role { get; set; }
    public OrganizationMemberStatus Status { get; set; }
    public bool? IsOrganizationOnboardingDone { get; set; }
    public Organization Organization { get; set; } = new();
    public Customer Customer { get; set; } = new();
    public IReadOnlyList<OrganizationOfferingActiveMember> OrganizationOfferingActiveMembers { get; set; } = [];
}
