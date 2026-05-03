using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class PrecomputedLocationProduct : ModelBase
{
    public Organization Organization { get; set; } = new();
    public Location Location { get; set; } = new();
    public Product Product { get; set; } = new();
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];
}
