using Enterprise.Shared.GraphQL.Types;
using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanConnection")]
public class FloorPlanConnection : Connection<FloorPlanEdge>;
