using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourceEdge")]
public class ResourceEdge(ResourceDetails node, string cursor) : Edge<ResourceDetails>(node, cursor);
