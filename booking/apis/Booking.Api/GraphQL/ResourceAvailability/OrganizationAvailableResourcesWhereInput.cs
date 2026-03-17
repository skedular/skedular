using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("OrganizationAvailableResourcesWhereInput")]
public class OrganizationAvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationCustomDomain")]
    public string? OrganizationCustomDomain { get; set; }

    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
}
