using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("DeleteFloorPlanInput")]
public class DeleteFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}
