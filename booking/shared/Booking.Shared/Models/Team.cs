using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public Organization? Organization { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<Booking> InvolvedBookings { get; set; } = [];
    public ICollection<RecurringBooking> InvolvedRecurringBooking { get; set; } = [];
}
