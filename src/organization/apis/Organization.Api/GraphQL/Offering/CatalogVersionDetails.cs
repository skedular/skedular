using HotChocolate;
using HotChocolate.Types.Composite;
using Organization.Shared.Models.PricingCatalog;

namespace Organization.Api.GraphQL.Offering;

[GraphQLName("CatalogVersionDetails")]
[Shareable]
public class CatalogVersionDetails
{
    [GraphQLName("type")] public CatalogVersion Type { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
