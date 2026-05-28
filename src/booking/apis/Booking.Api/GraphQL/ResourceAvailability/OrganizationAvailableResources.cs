using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("OrganizationAvailableResources")]
public class OrganizationAvailableResources
{
    [GraphQLName("resourcesCount")] public int ResourcesCount { get; set; }

    [GraphQLName("availableResourcesCount")]
    public int AvailableResourcesCount { get; set; }
}
