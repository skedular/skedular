using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class ProductVersion : ModelBase
{
    public ProductType Type { get; set; }
    public Product Product { get; set; } = new();
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];
    public IReadOnlyList<OrganizationTag> ProductTags => [.. OrganizationTags.Where(item => item.Type == OrganizationTagType.Product)];

    public IReadOnlyList<OrganizationTag> Amenities =>
        [.. OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType))];
}
