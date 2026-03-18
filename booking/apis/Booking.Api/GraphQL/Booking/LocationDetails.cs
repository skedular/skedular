using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("Booking_LocationDetails")]
public class LocationDetails
{
    [GraphQLName("uniqueId")] public string Id { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
}
