using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("Booking_OrganizationZoneDetails")]
public class OrganizationZoneDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string? Name { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
}
