using Api.Shared.Services.Models;
using HotChocolate;

namespace Location.Api.GraphQL.Location;

[GraphQLName("MarketplaceLocationWhereInput")]
public class MarketplaceLocationWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("locationIds")] public IEnumerable<string>? LocationIds { get; set; } = [];
    [GraphQLName("nameContains")] public string? NameContains { get; set; }
    [GraphQLName("customTagIds")] public IEnumerable<string>? CustomTagIds { get; set; }
    [GraphQLName("zoneIds")] public IEnumerable<string>? ZoneIds { get; set; }
    [GraphQLName("searchBoundaries")] public Polygon? SearchBoundaries { get; set; }
    [GraphQLName("resourceType")] public OrganizationTagType? ResourceType { get; set; }
}
