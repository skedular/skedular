using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Location.Shared.Models;

public class ProductVersion : ModelBase
{
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public ICollection<OrganizationTag> ProductTags => OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).ToList();

    public ICollection<OrganizationTag> Amenities =>
        OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType)).ToList();
}
