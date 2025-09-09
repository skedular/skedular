using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("TeamDetails")]
public class TeamDetails(string id) : Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = id;
}
