using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeCheckoutSession : ModelBaseWithDeleted
{
    public string StripeCheckoutSessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public decimal? AmountTotal { get; set; }
    public string? Currency { get; set; }
    public StripeCustomer StripeCustomer { get; set; } = new();
    public Booking Booking { get; set; } = new();
}
