using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("AvailableResourcesWhereInput")]
public class AvailableResourcesWhereInput
{
    [GraphQLName("organizationId")]
    public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("locationId")]
    public string? LocationId { get; set; }

    [GraphQLName("from")]
    public DateTimeOffset From { get; set; }

    [GraphQLName("until")]
    public DateTimeOffset Until { get; set; }

    [GraphQLName("customTagIds")]
    public IEnumerable<string>? CustomTagIds { get; set; }

    [GraphQLName("zoneIds")]
    public IEnumerable<string>? ZoneIds { get; set; }

    [GraphQLName("resourceIdsToInclude")]
    public IEnumerable<string>? ResourceIdsToInclude { get; set; }

    [GraphQLName("productId")]
    public string? ProductId { get; set; }
}
