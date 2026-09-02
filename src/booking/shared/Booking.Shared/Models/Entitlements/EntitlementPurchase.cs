using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models.Entitlements;

public class EntitlementPurchase : ModelBase
{
    public EntitlementPurchaseLifecycleState LifecycleState { get; set; }
    public string PaymentStatus { get; set; } = string.Empty;
    public DateTimeOffset? PaymentConfirmedAt { get; set; }
    public DateTimeOffset? CompletedAt { get; set; }
    public string? FailureReason { get; set; }
    public string PaymentMethod { get; set; } = string.Empty;
    public DateTimeOffset PaymentExpiry { get; set; }
    public DateTimeOffset ServiceStartAt { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    /// <summary>Immutable pricing terms captured when the purchase was created.</summary>
    public ProductPricing ProductPricing { get; set; } = ProductPricing.Empty(string.Empty);

    public string CustomerId { get; set; } = string.Empty;
    public string OrganizationId { get; set; } = string.Empty;
    public string ProductVersionId { get; set; } = string.Empty;
    public string? EntitlementId { get; set; }
    public string? CheckoutReturnUrl { get; set; }
    public string? InvoiceNumber { get; set; }
    public string? InvoiceUrl { get; set; }
    public string? PaymentInstructions { get; set; }
    public string? StripeCheckoutSessionId { get; set; }
    public string? StripeCheckoutUrl { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public string? StripeAccountId { get; set; }
    public string? PaymentAction { get; set; }
    public IReadOnlyList<string> InvoiceEmailList { get; set; } = [];
}
