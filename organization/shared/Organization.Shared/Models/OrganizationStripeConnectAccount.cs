using Enterprise.Shared.Models;

namespace Organization.Shared.Models;

public class OrganizationStripeConnectAccount : ModelBaseWithDeleted
{
    public bool IsDefault { get; set; }
    public string StripeAccountId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public bool ChargesEnabled { get; set; }
    public bool PayoutsEnabled { get; set; }
    public string Type { get; set; } = string.Empty;
    public string? Country { get; set; }
    public string? DefaultCurrency { get; set; }
    public string? BusinessType { get; set; }
    public string? CompanyName { get; set; }
    public string? Url { get; set; }
    public string? SupportUrl { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }
    public bool DetailsSubmitted { get; set; }
    public string CapabilitiesTransfers { get; set; } = string.Empty;
    public string CapabilitiesCardPayments { get; set; } = string.Empty;
    public string OnboardingUrl { get; set; } = string.Empty;

    public Organization Organization { get; set; } = new();
    public ICollection<OrganizationStripeConnectAccountRefreshCode> OrganizationStripeConnectAccountRefreshCodes { get; set; } = [];
    public OrganizationStripeConnectAccountAuthorization? OrganizationStripeConnectAccountAuthorization { get; set; }
}
