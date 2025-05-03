using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripeCheckoutSession : ModelBaseWithDeleted
{
    public string StripeCheckoutSessionId { get; set; } = string.Empty;
    public string CheckoutUrl { get; set; } = string.Empty;
    public PaymentStatus PaymentStatus { get; set; }
    public StripeCustomer StripeCustomer { get; set; } = new();
    public Booking? Booking { get; set; } = new();
}
