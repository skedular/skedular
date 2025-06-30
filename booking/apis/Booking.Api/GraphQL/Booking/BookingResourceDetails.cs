using HotChocolate;
using HotChocolate.Types.Relay;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingResourceDetails")]
public class BookingResourceDetails
{
    [GraphQLName("uniqueId")] [ID] public string UniqueId { get; set; } = string.Empty;
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("inactive")] public bool Inactive { get; set; }
    [GraphQLName("color")] public string? Color { get; set; }
    [GraphQLName("capacity")] public int Capacity { get; set; }

    [GraphQLName("requireBookingApproval")]
    public bool RequireBookingApproval { get; set; }

    [GraphQLName("location")] public LocationDetails? Location { get; set; }
    [GraphQLName("customTags")] public IEnumerable<OrganizationCustomTagDetails> CustomTags { get; set; } = [];
    [GraphQLName("zones")] public IEnumerable<OrganizationZoneDetails> Zones { get; set; } = [];
    [GraphQLName("customers")] public IEnumerable<CustomerDetails> Customers { get; set; } = [];
}
