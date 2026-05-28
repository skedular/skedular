using Api.Shared.Services.Models;
using HotChocolate;

// ReSharper disable ClassNeverInstantiated.Global

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("AddFloorPlanInput")]
public class AddFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string? Id { get; set; }
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("image")] public required CdnImageFile Image { get; set; }
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionInput>? ResourcePositions { get; set; }
}
