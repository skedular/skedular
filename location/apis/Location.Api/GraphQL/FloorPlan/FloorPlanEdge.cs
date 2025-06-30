using HotChocolate;
using HotChocolate.Types.Pagination;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanEdge")]
public class FloorPlanEdge(FloorPlanDetails node, string cursor) : Edge<FloorPlanDetails>(node, cursor);
