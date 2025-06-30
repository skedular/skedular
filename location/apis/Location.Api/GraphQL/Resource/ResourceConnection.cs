using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourceConnection")]
public class ResourceConnection : Connection<ResourceEdge>;
