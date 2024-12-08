using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public OldOrganizationMembershipType? MembershipType { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
