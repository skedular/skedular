using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class LocationMember : ReplicatedModelBaseWithDeleted
{
    public LocationMemberRole? Role { get; set; }
    public Location Location { get; set; }
    public Customer Customer { get; set; }
}
