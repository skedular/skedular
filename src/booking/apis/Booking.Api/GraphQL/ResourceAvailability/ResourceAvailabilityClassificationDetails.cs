using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("ResourceAvailabilityClassificationDetails")]
public class ResourceAvailabilityClassificationDetails
{
    [GraphQLName("type")]
    public ResourceAvailabilityClassification Type { get; set; }

    [GraphQLName("name")]
    public string Name { get; set; } = string.Empty;
}
