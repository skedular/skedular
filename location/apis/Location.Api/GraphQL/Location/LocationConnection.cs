using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationConnection")]
public class LocationConnection : Connection<LocationEdge>;
