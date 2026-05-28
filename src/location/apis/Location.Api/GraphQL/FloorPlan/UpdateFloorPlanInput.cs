using Api.Shared.Services.Models;
using HotChocolate;
using Location.Api.Models;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("UpdateFloorPlanInput")]
public class UpdateFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("fieldsToUpdate")] public HashSet<FloorPlanPatchField> FieldsToUpdate { get; set; } = [];
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("image")] public required CdnImageFile Image { get; set; }
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionInput>? ResourcePositions { get; set; }
}
