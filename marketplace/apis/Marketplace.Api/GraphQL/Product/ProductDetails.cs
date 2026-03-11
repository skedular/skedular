using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductDetails")]
public class ProductDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];
    [GraphQLName("amenityIds")] public IEnumerable<string> AmenityIds { get; set; } = [];

    [GraphQLName("latestProductVersionId")]
    public string LatestProductVersionId { get; set; } = string.Empty;

    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string OrganizationUniqueAlphanumericName { get; set; } = string.Empty;

    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
}

[ObjectType<ProductDetails>]
public static partial class ProductDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ProductDetails> descriptor)
    {
        descriptor.Ignore(item => item.OrganizationId);
        descriptor.Ignore(item => item.OrganizationUniqueAlphanumericName);
        descriptor.Ignore(item => item.ProductTagIds);
        descriptor.Ignore(item => item.AmenityIds);
    }

    public static OrganizationDetails GetOrganization([Parent] ProductDetails item) =>
        new(item.OrganizationId, item.OrganizationUniqueAlphanumericName);

    public static IEnumerable<OrganizationTagDetails> GetProductTags([Parent] ProductDetails item) =>
        item.ProductTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetAmenities([Parent] ProductDetails item) =>
        item.AmenityIds.Select(id => new OrganizationTagDetails(id));
}
