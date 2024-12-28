using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public OrganizationMembershipType? MembershipType { get; set; }
    public OrganizationMemberStatus Status { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
