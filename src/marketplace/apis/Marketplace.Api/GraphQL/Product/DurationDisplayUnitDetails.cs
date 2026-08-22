using Api.Shared.Services.Models;
using HotChocolate;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("DurationDisplayUnitDetails")]
public sealed class DurationDisplayUnitDetails
{
    [GraphQLName("type")]
    public DurationDisplayUnit Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
