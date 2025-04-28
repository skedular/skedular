using Enterprise.Shared.Models;
using Stripe.Checkout;

namespace Payment.Shared.Models;

public class StripeCheckoutSession : ModelBase
{
    public string StripeCheckoutSessionId { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? PaymentStatus { get; set; }
    public StripeCustomer StripeCustomer { get; set; } = new();
    public Booking Booking { get; set; } = new();
}
