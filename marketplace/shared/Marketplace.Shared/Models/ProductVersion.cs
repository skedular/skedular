using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class ProductVersion : ModelBase
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public Currency Currency { get; set; }
    public ICollection<CdnImageFile> FeatureImages { get; set; } = [];
    public ICollection<ProductPricing> PricingOptions { get; set; } = [];
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public ICollection<OrganizationTag> ProductTags => OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).ToList();

    public ICollection<OrganizationTag> Amenities =>
        OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType)).ToList();
}
