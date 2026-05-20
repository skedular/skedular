using Api.Shared.Services.Models;

namespace Booking.Shared.Models;

public sealed record ResourceAvailabilityResourceRow
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required bool Inactive { get; init; }
    public required string LocationId { get; init; }
    public required string LocationName { get; init; }
    public required string OrganizationType { get; init; }
    public required OpeningHours? OpeningHours { get; init; }
    public required string? ZoneId { get; init; }
    public required string? ZoneName { get; init; }
    public required string ResourceType { get; init; }
}
