using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Product : ModelBaseWithDeleted
{
    public bool Inactive { get; set; }
    public Organization Organization { get; set; } = new();
    public IReadOnlyList<ProductVersion> ProductVersions { get; set; } = [];
}
