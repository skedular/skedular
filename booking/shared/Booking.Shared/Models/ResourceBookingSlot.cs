using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class ResourceBookingSlot : ModelBase
{
    public DateTimeOffset Start { get; set; }
    public bool Available { get; set; }
    public Resource Resource { get; set; } = new();
    public ICollection<Customer> Customers { get; set; } = [];
    public ICollection<Booking> Bookings { get; set; } = [];
}
