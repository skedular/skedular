using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductVersionDetails")]
public class ProductVersionDetails : Node
{
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("description")] public string? Description { get; set; }
    [GraphQLName("currency")] public CurrencyDetails Currency { get; set; } = new();
    [GraphQLName("productTagIds")] public IEnumerable<string> ProductTagIds { get; set; } = [];
    [GraphQLName("locationTagIds")] public IEnumerable<string> LocationTagIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile> FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
}

[ObjectType<ProductVersionDetails>]
public static partial class ProductVersionDetailsType
{
    static partial void Configure(IObjectTypeDescriptor<ProductVersionDetails> descriptor)
    {
        descriptor.Ignore(item => item.ProductTagIds);
        descriptor.Ignore(item => item.LocationTagIds);
    }

    public static IEnumerable<OrganizationTagDetails> GetProductTags([Parent] ProductVersionDetails item) =>
        item.ProductTagIds.Select(id => new OrganizationTagDetails(id));

    public static IEnumerable<OrganizationTagDetails> GetLocationTags([Parent] ProductVersionDetails item) =>
        item.LocationTagIds.Select(id => new OrganizationTagDetails(id));
}
