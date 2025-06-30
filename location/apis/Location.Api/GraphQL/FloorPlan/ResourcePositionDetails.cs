using HotChocolate;
using HotChocolate.Types.Relay;
using Location.Api.GraphQL.Resource;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("ResourcePositionDetails")]
public class ResourcePositionDetails
{
    [GraphQLName("id")] [ID] public string Id { get; set; } = string.Empty;
    [GraphQLName("x")] public int X { get; set; }
    [GraphQLName("y")] public int Y { get; set; }
    [GraphQLName("resource")] public ResourceDetails Resource { get; set; } = new();
}
