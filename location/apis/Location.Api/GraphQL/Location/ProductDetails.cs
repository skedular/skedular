using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Location.Api.GraphQL.Location;

[GraphQLName("ProductDetails")]
public class ProductDetails(string id) : Node
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = id;
}
