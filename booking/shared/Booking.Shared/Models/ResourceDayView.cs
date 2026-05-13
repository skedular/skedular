namespace Booking.Shared.Models;

/// <summary>
///     Computed availability view for a single resource on a selected date.
///     Assembled at query time from Booking DB data — never persisted.
///     <see cref="BookingWindows" /> may contain empty-detail entries for non-admin users
///     in Marketplace/Individual organizations (detail fields are nulled by the visibility filter).
/// </summary>
public sealed record ResourceDayView
{
    public required string ResourceId { get; init; }
    public required string ResourceName { get; init; }
    public required string ResourceType { get; init; } // tag constant string
    public required string LocationId { get; init; }
    public required string LocationName { get; init; }
    public required string? FloorId { get; init; }
    public required string? FloorName { get; init; }
    public required string? ZoneId { get; init; }
    public required string? ZoneName { get; init; }
    public required DateOnly Date { get; init; }
    public required ResourceAvailabilityClassification Status { get; init; }
    public required TimeOnly? OpeningFrom { get; init; } // null = closed all day
    public required TimeOnly? OpeningUntil { get; init; } // null = closed all day
    public required int TotalOpeningMinutes { get; init; }
    public required int BookedMinutes { get; init; }
    public required IReadOnlyList<BookingWindow> BookingWindows { get; init; }
}
