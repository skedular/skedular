using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Team.Api.GraphQL.Team;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id, string uniqueAlphanumericName) : Node(id)
{
    [GraphQLName("uniqueAlphanumericName")]
    public string? UniqueAlphanumericName { get; set; } = uniqueAlphanumericName;
}
