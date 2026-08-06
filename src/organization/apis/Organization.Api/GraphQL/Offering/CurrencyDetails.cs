using Api.Shared.Services.Models;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("CurrencyDetails")]
[Shareable]
public class CurrencyDetails
{
    [GraphQLName("type")]
    public Currency Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
