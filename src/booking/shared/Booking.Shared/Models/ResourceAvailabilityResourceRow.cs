using Api.Shared.Services.Models;

namespace Booking.Shared.Models;

public sealed record ResourceAvailabilityResourceRow
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public required bool Inactive { get; set; }
    public required string LocationId { get; set; }
    public required string LocationName { get; set; }
    public required string OrganizationType { get; set; }
    public required OpeningHours? OpeningHours { get; set; }
    public required string? ZoneId { get; set; }
    public required string? ZoneName { get; set; }
    public required string ResourceType { get; set; }
    public required IReadOnlyList<ResourceBookingWindowRow> BookingWindows { get; set; }
}
