using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Organization : ReplicatedModelBaseWithDeleted
{
    public string? UniqueAlphanumericName { get; set; }
    public string? Name { get; set; }
    public string? LogoUrl { get; set; }
    public Offering? Offering { get; set; }
    public OrganizationType Type { get; set; }
    public ICollection<OrganizationTag> Tags { get; set; } = [];
    public ICollection<OrganizationMember> OrganizationMembers { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public OrganizationSsoSetting? OrganizationSsoSettings { get; set; }
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
