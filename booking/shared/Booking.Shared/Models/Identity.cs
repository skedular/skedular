using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class Identity : ReplicatedModelBase
{
    public string? Email { get; set; }
    public bool? EmailVerified { get; set; }
    public Customer Customer { get; set; }
}
