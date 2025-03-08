using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class ResourceBookingSlot : ModelBase
{
    public DateTime Start { get; set; }
    public bool Available { get; set; }
    public Resource Resource { get; set; }
    public ICollection<Customer> Customers { get; set; }
}
