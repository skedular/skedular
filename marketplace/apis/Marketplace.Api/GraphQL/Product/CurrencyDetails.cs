using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("CurrencyDetails")]
[Shareable]
public class CurrencyDetails
{
    [GraphQLName("type")] public Currency Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
