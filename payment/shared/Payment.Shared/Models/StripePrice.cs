using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripePrice : ModelBaseWithDeleted
{
    public string StripePriceId { get; set; } = string.Empty;
    public ProductVersion? ProductVersion { get; set; }
}
