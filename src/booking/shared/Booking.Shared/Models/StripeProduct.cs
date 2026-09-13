using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class StripeProduct : ModelBaseWithDeleted
{
    public string ProductPricingId { get; set; } = string.Empty;
    public MembershipTerm MembershipTerm { get; set; }
    public ProductPricingBillingMode BillingMode { get; set; }
    public int NumberOfResourcesToBook { get; set; }
    public string StripeProductId { get; set; } = string.Empty;
    public string StripeAccountId { get; set; } = string.Empty;
    public ProductVersion ProductVersion { get; set; } = new();
    public ICollection<StripePrice> StripePrices { get; set; } = [];
}
