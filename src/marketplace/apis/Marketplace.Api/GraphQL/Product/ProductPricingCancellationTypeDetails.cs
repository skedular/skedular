using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPricingCancellationTypeDetails")]
public class ProductPricingCancellationTypeDetails
{
    [GraphQLName("type")]
    public ProductPricingCancellationPolicyType Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
