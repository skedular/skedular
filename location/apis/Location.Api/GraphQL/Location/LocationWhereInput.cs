using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("LocationWhereInput")]
public class LocationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }
    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; } = [];
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
}
