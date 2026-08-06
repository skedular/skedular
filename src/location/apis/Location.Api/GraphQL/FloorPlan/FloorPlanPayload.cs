using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanPayload")]
public class FloorPlanPayload
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("floorPlan")]
    public FloorPlanDetails FloorPlan { get; set; } = new();
}
