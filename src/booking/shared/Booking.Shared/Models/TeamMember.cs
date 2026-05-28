using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class TeamMember : ReplicatedModelBaseWithDeleted
{
    public TeamMemberRole? Role { get; set; }
    public TeamMemberStatus Status { get; set; }
    public Team? Team { get; set; }
    public Customer Customer { get; set; } = new();
}
