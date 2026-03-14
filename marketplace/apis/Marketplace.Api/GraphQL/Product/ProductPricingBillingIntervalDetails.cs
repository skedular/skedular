using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductPricingBillingIntervalDetails")]
public class ProductPricingBillingIntervalDetails
{
    [GraphQLName("type")] public ProductPricingBillingInterval Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
