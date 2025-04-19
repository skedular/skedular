using Enterprise.Shared.Models;

namespace Billing.Shared.Models;

public class Customer : ReplicatedModelBaseWithDeleted
{
    public string? Title { get; set; }
    public string? Name { get; set; }
    public string? GivenName { get; set; }
    public string? MiddleName { get; set; }
    public string? FamilyName { get; set; }

    public string? BillingContactCompanyName { get; set; }
    public string? BillingContactEmail { get; set; }
    public string? BillingContactAddressLine1 { get; set; }
    public string? BillingContactAddressLine2 { get; set; }
    public string? BillingContactSuburb { get; set; }
    public string? BillingContactCity { get; set; }
    public string? BillingContactProvince { get; set; }
    public string? BillingContactZipcode { get; set; }
    public string? BillingContactCountry { get; set; }

    public ICollection<Identity> Identities { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
}
