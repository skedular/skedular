using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductDetails")]
[Shareable]
public class ProductDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("type")] public ProductTypeDetails Type { get; set; } = new();
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("amenities")] public IEnumerable<OrganizationTagDetails> Amenities { get; set; } = [];
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;

    [GraphQLName("latestProductVersionId")]
    public string LatestProductVersionId { get; set; } = string.Empty;
}
