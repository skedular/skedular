using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPricingFulfillmentTypeDetails")]
public sealed class ProductPricingFulfillmentTypeDetails
{
    [GraphQLName("type")]
    public ProductPricingFulfillmentType Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
