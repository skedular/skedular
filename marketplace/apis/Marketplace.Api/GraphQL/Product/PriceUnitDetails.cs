using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("PriceUnitDetails")]
public class PriceUnitDetails
{
    [GraphQLName("type")] public PriceUnit Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
