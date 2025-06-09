using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class StripePaymentIntent : ModelBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public StripePaymentMethod StripePaymentMethod { get; set; } = new();
    public OrganizationOffering? OrganizationOffering { get; set; }
}
