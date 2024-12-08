using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Customer.Shared.Models;

public class LocationMember : ReplicatedModelBaseWithDeleted
{
    public OldLocationMembershipType? MembershipType { get; set; }
    public Location Location { get; set; }
    public Customer Customer { get; set; }
}
