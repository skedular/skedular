using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("UpdateProductInput")]
public class UpdateProductInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("currency")] public Currency Currency { get; set; }
    [GraphQLName("tagIds")] public IEnumerable<string> TagIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata? ListingMetadata { get; set; } = ListingMetadata.Empty();
}
