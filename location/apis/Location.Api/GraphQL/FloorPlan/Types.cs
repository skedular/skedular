using Api.Shared.Services.Models;
using Enterprise.Shared.GraphQL.Types;
using Enterprise.Shared.Pagination;
using HotChocolate;
using HotChocolate.Types.Pagination;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Resource;
using Location.Shared.Models;

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

[GraphQLName("UpdateFloorPlanInput")]
public class UpdateFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("image")] public required CdnImageFile Image { get; set; }
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionInput>? ResourcePositions { get; set; }
}

[GraphQLName("DeleteFloorPlanInput")]
public class DeleteFloorPlanInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("id")] public string Id { get; set; } = string.Empty;
}

[GraphQLName("FloorPlanWhereInput")]
public class FloorPlanWhereInput
{
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
}

[GraphQLName("FloorPlanOrderInput")]
public class FloorPlanOrderInput
{
    [GraphQLName("direction")] public OrderDirection Direction { get; set; }
    [GraphQLName("field")] public FloorPlanOrderField Field { get; set; }
}

[GraphQLName("FloorPlanConnection")]
public class FloorPlanConnection : Enterprise.Shared.GraphQL.Types.Connection<FloorPlanEdge>;

[GraphQLName("FloorPlanEdge")]
public class FloorPlanEdge(FloorPlanDetails node, string cursor) : Edge<FloorPlanDetails>(node, cursor);

[GraphQLName("FloorPlanDetails")]
public class FloorPlanDetails : Node
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("image")] public CdnImageFile Image { get; set; } = new(null, null);
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionDetails> ResourcePositions { get; set; } = [];
    [GraphQLName("resourceCount")] public int ResourceCount { get; set; }
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
}

[GraphQLName("FloorPlanPayload")]
public class FloorPlanPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("floorPlan")] public FloorPlanDetails FloorPlan { get; set; } = new();
}

[GraphQLName("ResourcePositionInput")]
public class ResourcePositionInput
{
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
}

[GraphQLName("UpdateResourcePositionsInput")]
public class UpdateResourcePositionsInput
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("floorPlanId")] public string FloorPlanId { get; set; } = string.Empty;
    [GraphQLName("resourcePositions")] public IEnumerable<ResourcePositionInput> ResourcePositions { get; set; } = [];
}

[GraphQLName("ResourcePositionDetails")]
public class ResourcePositionDetails
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}
