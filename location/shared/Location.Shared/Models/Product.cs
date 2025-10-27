using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class Product : ReplicatedModelBaseWithDeleted
{
    public bool Inactive { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
