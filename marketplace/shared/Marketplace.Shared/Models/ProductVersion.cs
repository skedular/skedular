using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Marketplace.Shared.Models;

public class ProductVersion : ModelBase
{
    public ProductType Type { get; set; }
    public Currency Currency { get; set; }
    public IReadOnlyList<CdnImageFile> FeatureImages { get; set; } = [];
    public IReadOnlyList<ProductPricing> PricingOptions { get; set; } = [];
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    public Product Product { get; set; } = new();
    public IReadOnlyList<OrganizationTag> OrganizationTags { get; set; } = [];
    public IReadOnlyList<OrganizationTag> ProductTags => OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).ToList();

    public IReadOnlyList<OrganizationTag> Amenities =>
        OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType)).ToList();
}
