using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("ProductDetails")]
public class ProductDetails(string id) : Node(id);
