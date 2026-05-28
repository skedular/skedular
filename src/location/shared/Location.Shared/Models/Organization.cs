using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? CustomDomain { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public bool? IsOwnershipVerified { get; set; }
    public IReadOnlyList<OrganizationTag> Tags { get; set; } = [];
    public IReadOnlyList<OrganizationMember> OrganizationMembers { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public IReadOnlyList<Product> Products { get; set; } = [];
    public IReadOnlyList<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
