using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.ResourceAvailability;

[GraphQLName("ResourceDayViewDetails")]
public class ResourceDayViewDetails
{
    [GraphQLName("resourceId")] public string ResourceId { get; set; } = string.Empty;
    [GraphQLName("resourceName")] public string ResourceName { get; set; } = string.Empty;
    [GraphQLName("resourceType")] public string ResourceType { get; set; } = string.Empty;
    [GraphQLName("locationId")] public string LocationId { get; set; } = string.Empty;
    [GraphQLName("locationName")] public string LocationName { get; set; } = string.Empty;
    [GraphQLName("floorId")] public string? FloorId { get; set; }
    [GraphQLName("floorName")] public string? FloorName { get; set; }
    [GraphQLName("zoneId")] public string? ZoneId { get; set; }
    [GraphQLName("zoneName")] public string? ZoneName { get; set; }
    [GraphQLName("date")] public DateOnly Date { get; set; }
    [GraphQLName("status")] public ResourceAvailabilityClassification Status { get; set; }
    [GraphQLName("openingFrom")] public TimeOnly? OpeningFrom { get; set; }
    [GraphQLName("openingUntil")] public TimeOnly? OpeningUntil { get; set; }
    [GraphQLName("totalOpeningMinutes")] public int TotalOpeningMinutes { get; set; }
    [GraphQLName("bookedMinutes")] public int BookedMinutes { get; set; }
    [GraphQLName("bookingWindows")] public IEnumerable<BookingWindowDetails> BookingWindows { get; set; } = [];
}
