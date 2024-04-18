using Api.Shared.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class TeamMember : ReplicatedModelBaseWithDeleted
{
    public TeamMembershipType? MembershipType { get; set; }
    public Team? Team { get; set; }
    public Customer Customer { get; set; }
}
