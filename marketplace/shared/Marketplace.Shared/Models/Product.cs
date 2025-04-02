using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class Product : ModelBaseWithDeleted
{
    public bool Inactive { get; set; }
    public virtual Organization Organization { get; set; }
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
