using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripePaymentIntent : ModelBaseWithDeleted
{
    public long Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
    public OrganizationStripePaymentMethod OrganizationStripePaymentMethod { get; set; } = new();
    public OrganizationOffering? OrganizationOffering { get; set; }
}
