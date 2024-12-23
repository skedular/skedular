using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class OrganizationMember : ReplicatedModelBaseWithDeleted
{
    public string? MembershipType { get; set; }
    public bool Active { get; set; }
    public Organization Organization { get; set; }
    public Customer Customer { get; set; }
}
