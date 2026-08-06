using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPricingCadenceDetails")]
public class ProductPricingCadenceDetails
{
    [GraphQLName("type")]
    public ProductPricingCadence Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
