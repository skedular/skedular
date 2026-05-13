namespace Booking.Shared.Models;

/// <summary>
///     Computed availability state for a single resource on a given date.
///     Evaluated using a fixed precedence rule:
///     <c>Blocked &gt; Occupied &gt; FullyBooked &gt; PartiallyBooked &gt; Unavailable &gt; Available</c>.
///     This enum is owned by <c>Booking.Shared</c> and is distinct from the 3-state
///     <c>ResourceAvailabilityClassification</c> in <c>Location.Shared</c> used for analytics snapshots.
/// </summary>
public enum ResourceAvailabilityClassification
{
    Available,
    Unavailable,
    PartiallyBooked, // at least one booking but free time remains in opening hours
    FullyBooked, // all opening-hour time covered by bookings
    Occupied, // checked-in (current date only, where check-in data available)
    Blocked // location ClosedDates or resource IsActive == false (v1)
}

/// <summary>
///     String constants representing each <see cref="ResourceAvailabilityClassification" /> value
///     as exposed in the GraphQL schema. Use these constants when mapping to/from the
///     GraphQL enum wire format.
/// </summary>
public static class ResourceAvailabilityClassificationConstants
{
    public const string Available = "AVAILABLE";
    public const string Unavailable = "UNAVAILABLE";
    public const string PartiallyBooked = "PARTIALLY_BOOKED";
    public const string FullyBooked = "FULLY_BOOKED";
    public const string Occupied = "OCCUPIED";
    public const string Blocked = "BLOCKED";
}

public static class ResourceAvailabilityClassificationExtensions
{
    extension(ResourceAvailabilityClassification src)
    {
        public string ToResourceAvailabilityClassificationName() =>
            src switch
            {
                ResourceAvailabilityClassification.Available => "Available",
                ResourceAvailabilityClassification.Unavailable => "Unavailable",
                ResourceAvailabilityClassification.PartiallyBooked => "Partially Booked",
                ResourceAvailabilityClassification.FullyBooked => "Fully Booked",
                ResourceAvailabilityClassification.Occupied => "Occupied",
                ResourceAvailabilityClassification.Blocked => "Blocked",
                _ => throw new ArgumentOutOfRangeException(nameof(src), src, null)
            };
    }
}
