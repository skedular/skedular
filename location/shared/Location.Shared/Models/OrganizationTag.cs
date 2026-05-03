using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<Resource> Resources { get; set; } = [];
    public IReadOnlyList<Location> Locations { get; set; } = [];
    public IReadOnlyList<ProductVersion> ProductVersionOrganizationTags { get; set; } = [];
    public IReadOnlyList<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
