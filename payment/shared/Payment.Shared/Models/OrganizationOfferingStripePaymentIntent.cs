using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class OrganizationOfferingStripePaymentIntent : ModelBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public OrganizationStripePaymentMethod OrganizationStripePaymentMethod { get; set; }
    public OrganizationOffering OrganizationOffering { get; set; }
}
