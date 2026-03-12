using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<Resource> Resources { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
    public ICollection<ProductVersion> ProductVersionOrganizationTags { get; set; } = [];
    public ICollection<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
