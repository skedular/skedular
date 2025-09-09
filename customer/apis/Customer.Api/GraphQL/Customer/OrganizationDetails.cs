using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Customer.Api.GraphQL.Customer;

[GraphQLName("OrganizationDetails")]
public class OrganizationDetails(string id) : Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = id;
}
