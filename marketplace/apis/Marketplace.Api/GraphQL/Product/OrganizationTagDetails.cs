using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Marketplace.Api.GraphQL.Product;

[GraphQLName("OrganizationTagDetails")]
public class OrganizationTagDetails(string id) : Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = id;
}
