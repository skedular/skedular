using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class StripePaymentIntent : ModelBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public StripePaymentMethod StripePaymentMethod { get; set; }
    public OrganizationOffering? OrganizationOffering { get; set; }
}
