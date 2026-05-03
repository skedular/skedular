using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Location.Api.GraphQL.Location;

[GraphQLName("OrganizationDetails")]
[Shareable]
public class OrganizationDetails(string id, string customDomain) : Node(id)
{
    [GraphQLName("customDomain")] public string? CustomDomain { get; set; } = customDomain;
}
