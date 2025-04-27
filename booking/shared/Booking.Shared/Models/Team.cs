using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Team : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }

    public Organization? Organization { get; set; }
    public ICollection<TeamMember> TeamMembers { get; set; } = [];
    public ICollection<Customer> DefaultedByCustomers { get; set; } = [];
    public ICollection<Booking> InvolvedBookings { get; set; } = [];
}
