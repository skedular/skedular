using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class ProductVersion : ModelBase
{
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> ProductTags { get; set; } = [];
}
