namespace Booking.Shared.Models;

/// <summary>
///     Computed availability view for a single resource on a selected date.
///     Assembled at query time from Booking DB data — never persisted.
///     <see cref="BookingWindows" /> may contain empty-detail entries for non-admin users
///     in Marketplace/Individual organizations (detail fields are nulled by the visibility filter).
/// </summary>
public sealed record ResourceDayView
{
    public required string ResourceId { get; set; }
    public required string ResourceName { get; set; }
    public required string ResourceType { get; set; } // tag constant string
    public required string LocationId { get; set; }
    public required string LocationName { get; set; }
    public required string? FloorId { get; set; }
    public required string? FloorName { get; set; }
    public required string? ZoneId { get; set; }
    public required string? ZoneName { get; set; }
    public required DateOnly Date { get; set; }
    public required ResourceAvailabilityClassification Status { get; set; }
    public required TimeOnly? OpeningFrom { get; set; } // null = closed all day
    public required TimeOnly? OpeningUntil { get; set; } // null = closed all day
    public required int TotalOpeningMinutes { get; set; }
    public required int BookedMinutes { get; set; }
    public required IReadOnlyList<BookingWindow> BookingWindows { get; set; }
}
