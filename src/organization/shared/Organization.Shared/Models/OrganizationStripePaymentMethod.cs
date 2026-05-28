using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripePaymentMethod : ModelBaseWithDeleted
{
    public string? SetupIntentId { get; set; }
    public string? PaymentMethodId { get; set; }
    public string? CardBrand { get; set; }
    public string? CardCountry { get; set; }
    public string? CardDescription { get; set; }
    public byte? CardExpiryMonth { get; set; }
    public short? CardExpiryYear { get; set; }
    public string? CardFingerprint { get; set; }
    public string? CardFunding { get; set; }
    public string? CardIssuer { get; set; }
    public string? CardLastFourDigit { get; set; }

    public Organization? Organization { get; set; }
    public IReadOnlyList<OrganizationStripePaymentIntent> OrganizationStripePaymentIntents { get; set; } = [];
}
