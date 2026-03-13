using Api.Shared.Services.Models;
using Enterprise.Shared.Models;

namespace Booking.Shared.Models;

public class ProductVersion : ReplicatedModelBase
{
    public Currency Currency { get; set; }
    public ICollection<ProductPricing> PricingOptions { get; set; } = [];
    public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    public Product Product { get; set; } = new();
    public ICollection<OrganizationTag> OrganizationTags { get; set; } = [];
    public ICollection<OrganizationTag> ProductTags => OrganizationTags.Where(item => item.Type == OrganizationTagType.Product).ToList();

    public ICollection<OrganizationTag> Amenities =>
        OrganizationTags.Where(item => OrganizationTagTypeConstants.Amenities.Any(tagType => item.Type == tagType)).ToList();

    public ICollection<StripeProduct> StripeProducts { get; set; } = [];
}
