using Enterprise.Shared.Pagination;
using HotChocolate;
using Location.Shared.Models;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanOrderInput")]
public class FloorPlanOrderInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public FloorPlanOrderField Field { get; set; }
}
