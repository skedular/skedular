using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeProduct : ModelBaseWithDeleted
{
    public string StripeProductId { get; set; } = string.Empty;
    public string StripeAccountId { get; set; } = string.Empty;
    public ProductVersion ProductVersion { get; set; } = new();
    public StripePrice? StripePrice { get; set; }
}
