using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("OrganizationAvailableResourcesWhereInput")]
public class OrganizationAvailableResourcesWhereInput
{
    [GraphQLName("organizationId")] public string? OrganizationId { get; set; }

    [GraphQLName("organizationUniqueAlphanumericName")]
    public string? OrganizationUniqueAlphanumericName { get; set; }

    [GraphQLName("from")] public DateTimeOffset From { get; set; }
    [GraphQLName("until")] public DateTimeOffset Until { get; set; }
}
