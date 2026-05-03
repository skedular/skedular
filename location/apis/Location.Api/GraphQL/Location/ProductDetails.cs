using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Composite;

namespace Location.Api.GraphQL.Location;

[GraphQLName("ProductDetails")]
[Shareable]
public class ProductDetails(string id) : Node(id);
