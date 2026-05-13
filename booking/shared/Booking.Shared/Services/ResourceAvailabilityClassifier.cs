using Booking.Shared.Models;

namespace Booking.Shared.Services;

/// <summary>
///     Classifies the availability status of a single resource for a given day.
///     Extracted as a separate, testable unit from <see cref="ResourceAvailabilityDayViewService" />.
/// </summary>
public interface IResourceAvailabilityClassifier
{
    /// <summary>
    ///     Classifies the availability status for a resource on a specific day.
    ///     Precedence (highest to lowest): Blocked → Unavailable → FullyBooked → PartiallyBooked → Available.
    /// </summary>
    /// <param name="inactive">Whether the resource is inactive (blocked).</param>
    /// <param name="isLocationClosed">Whether the location is closed on the given date.</param>
    /// <param name="isDayClosed">Whether the opening hours for the day are marked as closed.</param>
    /// <param name="totalOpeningMinutes">Total effective opening minutes for the day (0 = no hours configured).</param>
    /// <param name="bookedMinutes">Total booked minutes (merged, non-overlapping) during the day.</param>
    /// <returns>The <see cref="ResourceAvailabilityClassification" /> for the resource on that day.</returns>
    ResourceAvailabilityClassification Classify(
        bool inactive,
        bool isLocationClosed,
        bool isDayClosed,
        int totalOpeningMinutes,
        int bookedMinutes);
}

/// <inheritdoc cref="IResourceAvailabilityClassifier" />
public sealed class ResourceAvailabilityClassifier : IResourceAvailabilityClassifier
{
    /// <inheritdoc />
    public ResourceAvailabilityClassification Classify(
        bool inactive,
        bool isLocationClosed,
        bool isDayClosed,
        int totalOpeningMinutes,
        int bookedMinutes) =>
        inactive
            ? ResourceAvailabilityClassification.Blocked
            : isLocationClosed || isDayClosed
                ? ResourceAvailabilityClassification.Unavailable
                : totalOpeningMinutes == 0
                    ? ResourceAvailabilityClassification.Unavailable
                    : bookedMinutes <= 0
                        ? ResourceAvailabilityClassification.Available
                        : bookedMinutes >= totalOpeningMinutes
                            ? ResourceAvailabilityClassification.FullyBooked
                            : ResourceAvailabilityClassification.PartiallyBooked;
}
