using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class StripeConnectAccount : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; } = string.Empty;

    public Organization Organization { get; set; } = new();
    public ICollection<Product> Products { get; set; } = [];
    public ICollection<ProductVersion> ProductVersions { get; set; } = [];
}
