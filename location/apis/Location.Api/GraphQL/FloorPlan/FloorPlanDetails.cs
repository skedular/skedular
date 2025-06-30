using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using HotChocolate.Types.Relay;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("FloorPlanDetails")]
public class FloorPlanDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("image")] public CdnImageFile Image { get; set; } = new(null, null);
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionDetails> ResourcePositions { get; set; } = [];
    [GraphQLName("resourceCount")] public int ResourceCount { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}
