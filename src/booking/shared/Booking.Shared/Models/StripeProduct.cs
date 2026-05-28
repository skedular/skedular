using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeProduct : ModelBaseWithDeleted
{
    public string ProductPricingId { get; set; } = string.Empty;
    public ProductPricingCadence PricingCadence { get; set; }
    public ProductPricingBillingMode BillingMode { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public string StripeProductId { get; set; } = string.Empty;
    public string StripeAccountId { get; set; } = string.Empty;
    public ProductVersion ProductVersion { get; set; } = new();
    public StripePrice? StripePrice { get; set; }
}
