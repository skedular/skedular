using Enterprise.Shared.Pagination;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourceOrderInput")]
public class ResourceOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public ResourceOrderField Field { get; set; }
}
