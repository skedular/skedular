using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationEdge")]
public class LocationEdge(LocationDetails node, string cursor) : Edge<LocationDetails>(node, cursor);
