using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class LocationMember : ModelBaseWithDeleted
{
    public LocationMembershipType MembershipType { get; set; } = LocationMembershipType.Member;
    public Location Location { get; set; }
    public Customer Customer { get; set; }
}
