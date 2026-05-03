using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public Organization? Organization { get; set; }
    public IReadOnlyList<TeamMember> TeamMembers { get; set; } = [];
    public IReadOnlyList<Booking> InvolvedBookings { get; set; } = [];
    public IReadOnlyList<RecurringBooking> InvolvedRecurringBooking { get; set; } = [];
}
