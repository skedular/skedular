using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string? Name { get; set; }
    public string? Website { get; set; }
    public string? LogoUrl { get; set; }
    public string? CustomerFacingTermsAndConditionsUrl { get; set; }
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Product> Products { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
}
