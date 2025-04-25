using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationType Type { get; set; }
    public OrganizationMemberVisibilityPolicy MemberVisibilityPolicy { get; set; }
    public string? ContactEmail { get; set; }
    public string? ContactPhone { get; set; }

    public Address? PhysicalAddress { get; set; }
    public StripeCustomer? StripeCustomer { get; set; }

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public ICollection<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public ICollection<OrganizationStripeConnectAccount> OrganizationStripeConnectAccounts { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public ICollection<Product> Products { get; set; } = [];
}
