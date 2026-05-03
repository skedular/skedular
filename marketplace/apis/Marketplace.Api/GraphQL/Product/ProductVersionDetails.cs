using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductVersionDetails")]
[EntityKey("id")]
[Shareable]
public class ProductVersionDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    [GraphQLName("type")] public ProductTypeDetails Type { get; set; } = new();
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("organization")] public OrganizationDetails Organization { get; set; } = new();
}
