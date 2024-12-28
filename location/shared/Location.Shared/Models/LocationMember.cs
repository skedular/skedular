using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class LocationMember : ModelBaseWithDeleted
{
    public LocationMembershipType MembershipType { get; set; }
    public Location Location { get; set; }
    public Customer Customer { get; set; }
}
