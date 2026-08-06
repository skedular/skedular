using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPricingBillingModeDetails")]
public class ProductPricingBillingModeDetails
{
    [GraphQLName("type")]
    public ProductPricingBillingMode Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
