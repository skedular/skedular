using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("ProductTypeDetails")]
public class ProductTypeDetails
{
    [GraphQLName("type")]
    public ProductType Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
