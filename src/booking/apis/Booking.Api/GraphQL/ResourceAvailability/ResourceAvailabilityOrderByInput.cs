using Booking.Shared.Models;
using Enterprise.Shared.Pagination;
using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("ResourceAvailabilityOrderByInput")]
public class ResourceAvailabilityOrderByInput
{
    [GraphQLName("direction")]
    public OrderDirection Direction { get; set; }

    [GraphQLName("field")]
    public ResourceAvailabilityOrderByField Field { get; set; }
}
