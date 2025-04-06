using Enterprise.Shared.Models;

namespace Payment.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public string? StripeCustomerId { get; set; }

    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<OrganizationOffering> OrganizationOfferings { get; set; } = [];
    public ICollection<OrganizationStripePaymentMethod> OrganizationStripePaymentMethods { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
