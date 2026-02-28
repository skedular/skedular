using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripePrice : ModelBaseWithDeleted
{
    public string StripePriceId { get; set; } = string.Empty;
    public StripeProduct StripeProduct { get; set; } = new();
}
