using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Product : ReplicatedModelBaseWithDeleted
{
    public bool Inactive { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<ProductVersion> ProductVersions { get; set; } = [];
    public IReadOnlyList<PrecomputedLocationProduct> PrecomputedLocationProducts { get; set; } = [];
}
