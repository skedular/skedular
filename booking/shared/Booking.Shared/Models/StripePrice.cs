using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripePrice : ModelBaseWithDeleted
{
    public string StripePriceId { get; set; } = string.Empty;
    public ProductVersion ProductVersion { get; set; } = new();
    public StripeProduct StripeProduct { get; set; } = new();
}
