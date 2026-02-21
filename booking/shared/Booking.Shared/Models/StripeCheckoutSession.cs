using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeCheckoutSession : ModelBaseWithDeleted
{
    public string StripeCheckoutSessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public StripeCustomer StripeCustomer { get; set; } = new();
    public MarketplaceBooking MarketplaceBooking { get; set; } = new();
}
