using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductVersionDetails")]
public class ProductVersionDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("listingMetadata")] public ListingMetadata ListingMetadata { get; set; } = ListingMetadata.Empty;
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTags")] public IEnumerable<OrganizationTagDetails> ProductTags { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("organizationCustomDomain")] public string OrganizationCustomDomain { get; set; } = string.Empty;
}

[ObjectType<ProductVersionDetails>]
public static partial class ProductVersionDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ProductVersionDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationCustomDomain);
    }

    public static OrganizationDetails GetOrganization([Parent] ProductVersionDetails item) =>
        new(item.OrganizationId, item.OrganizationCustomDomain);
}
