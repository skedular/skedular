using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class OrganizationTag : ReplicatedModelBaseWithDeleted
{
    public string? Name { get; set; }
    public OrganizationTagType? Type { get; set; }
    public string? Color { get; set; }
    public Organization Organization { get; set; } = new();
    public ICollection<Product> ProductProductTag { get; set; } = [];
    public ICollection<Product> ProductLocationTags { get; set; } = [];
    public ICollection<ProductVersion> ProductVersionProductTag { get; set; } = [];
    public ICollection<Location> Locations { get; set; } = [];
}
