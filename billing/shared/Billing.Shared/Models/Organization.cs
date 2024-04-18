using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? BillingContactEmail { get; set; }
    public string? BillingContactAddressLine1 { get; set; }
    public string? BillingContactAddressLine2 { get; set; }
    public string? BillingContactSuburb { get; set; }
    public string? BillingContactCity { get; set; }
    public string? BillingContactProvince { get; set; }
    public string? BillingContactZipcode { get; set; }
    public string? BillingContactCountry { get; set; }

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
}
