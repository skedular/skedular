using Enterprise.Shared.Database;

namespace Location.Shared.Models;

public class Desk : EntityBaseWithDeleted
{
    public string Name { get; set; }
    public bool Deactivated { get; set; }
    public bool RequireBookingApproval { get; set; }

    public Location Location { get; set; }
    public ICollection<Tag> Tags { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
