using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("UpdateResourcePositionsInput")]
public class UpdateResourcePositionsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("floorPlanId")] public string FloorPlanId { get; set; } = string.Empty;
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionInput> ResourcePositions { get; set; } = [];
}
