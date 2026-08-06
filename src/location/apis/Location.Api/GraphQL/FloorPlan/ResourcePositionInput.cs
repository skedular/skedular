using HotChocolate;

namespace Location.Api.GraphQL.FloorPlan;

[GraphQLName("ResourcePositionInput")]
public class ResourcePositionInput
{
    [GraphQLName("resourceId")]
    public string ResourceId { get; set; } = string.Empty;

    [GraphQLName("x")]
    public int X { get; set; }

    [GraphQLName("y")]
    public int Y { get; set; }
}
