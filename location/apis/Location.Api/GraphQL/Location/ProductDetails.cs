using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Location.Api.GraphQL.Location;

[GraphQLName("ProductDetails")]
[EntityKey("id")]
public class ProductDetails(string id) : Node(id);
