using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Desk : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }

    public Location? Location { get; set; }
    public ICollection<LocationTag> Tags { get; set; } = [];
    public ICollection<Customer> PreferredByCustomers { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; }
}
