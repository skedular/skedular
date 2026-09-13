using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripePrice : ModelBaseWithDeleted
{
    public string StripePriceId { get; set; } = string.Empty;
    public bool IsRecurring { get; set; }
    public string? ProductPricingId { get; set; }
    public string? StripeAccountId { get; set; }
    public string? Currency { get; set; }
    public decimal? UnitAmount { get; set; }
    public MembershipTerm? MembershipTerm { get; set; }
    public ProductPricingBillingMode? BillingMode { get; set; }
    public bool? IsTaxInclusive { get; set; }
    public StripeProduct? StripeProduct { get; set; }
}
