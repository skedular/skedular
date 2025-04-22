using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class OrganizationStripeConnectAccount : ModelBaseWithDeleted
{
    public string Name { get; set; } = string.Empty;
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public string DefaultCurrency { get; set; } = string.Empty;
    public string BusinessType { get; set; } = string.Empty;
    public string CompanyName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string CapabilitiesTransfers { get; set; } = string.Empty;
    public string CapabilitiesCardPayments { get; set; } = string.Empty;
    public string OnboardingUrl { get; set; } = string.Empty;

    public Organization Organization { get; set; }
}
