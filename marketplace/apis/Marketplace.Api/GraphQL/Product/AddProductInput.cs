using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("AddProductInput")]
public class AddProductInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("currency")] public Currency Currency { get; set; }
    [GraphQLName("tagIds")] public IEnumerable<string> TagIds { get; set; } = [];
    [GraphQLName("featureImages")] public IEnumerable<CdnImageFile>? FeatureImages { get; set; } = [];
    [GraphQLName("pricingOptions")] public IEnumerable<ProductPricing> PricingOptions { get; set; } = [];
    [GraphQLName("listingMetadata")] public ListingMetadata? ListingMetadata { get; set; } = ListingMetadata.Empty;
}
