using Enterprise.Shared.GraphQL.Types;
using HotChocolate;
using Location.Api.GraphQL.Resource;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("ResourcePositionDetails")]
public class ResourcePositionDetails : Node
{
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}
