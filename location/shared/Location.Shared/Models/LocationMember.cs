using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class LocationMember : ModelBaseWithDeleted
{
    public OldLocationMembershipType MembershipType { get; set; } = OldLocationMembershipType.Member;
    public Location Location { get; set; }
    public Customer Customer { get; set; }
}
