using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductDetails")]
public class ProductDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("amenities")] public IEnumerable<OrganizationTagDetails> Amenities { get; set; } = [];

    [GraphQLName("latestProductVersionId")]
    public string LatestProductVersionId { get; set; } = string.Empty;

    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string OrganizationUniqueAlphanumericName { get; set; } = string.Empty;

    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty();
}

[ObjectType<ProductDetails>]
public static partial class ProductDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ProductDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationUniqueAlphanumericName);
    }

    public static OrganizationDetails GetOrganization([Parent] ProductDetails item) =>
        new(item.OrganizationId, item.OrganizationUniqueAlphanumericName);
}
