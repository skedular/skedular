using HotChocolate;
using Location.Api.Models;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("UpdateResourcePositionsInput")]
public class UpdateResourcePositionsInput
{
    [GraphQLName("clientMutationId")]
    public string? ClientMutationId { get; set; }

    [GraphQLName("floorPlanId")]
    public string FloorPlanId { get; set; } = string.Empty;

    [GraphQLName("fieldsToUpdate")]
    public HashSet<ResourcePositionsPatchField> FieldsToUpdate { get; set; } = [];

    [GraphQLName("resourcePositions")]
    public IEnumerable<ResourcePositionInput> ResourcePositions { get; set; } = [];
}
