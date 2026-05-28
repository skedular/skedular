using HotChocolate;

namespace Location.Api.GraphQL.Resource;

[GraphQLName("ResourceWhereInput")]
public class ResourceWhereInput
{
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("productTagIds")] public IEnumerable<string>? ProductTagIds { get; set; }
    [GraphQLName("floorPlanId")] public string? FloorPlanId { get; set; }
}
