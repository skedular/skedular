using Api.Shared.Services.Models;
using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("CurrencyDetails")]
public class CurrencyDetails
{
    [GraphQLName("type")] public Currency Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
