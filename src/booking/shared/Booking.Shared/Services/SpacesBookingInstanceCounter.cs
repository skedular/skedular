namespace Booking.Shared.Services;

public interface ISpacesBookingInstanceCounter
{
    SpacesBookingInstanceCount CountCurrentPeriodInstances(
        IReadOnlyList<DateTimeOffset> bookingStartUtcValues,
        DateTimeOffset periodStartUtc,
        DateTimeOffset periodEndUtc);
}

public record SpacesBookingInstanceCount(int CurrentPeriodCount, int ExcludedOutOfPeriodCount);

public class SpacesBookingInstanceCounter : ISpacesBookingInstanceCounter
{
    public SpacesBookingInstanceCount CountCurrentPeriodInstances(
        IReadOnlyList<DateTimeOffset> bookingStartUtcValues,
        DateTimeOffset periodStartUtc,
        DateTimeOffset periodEndUtc)
    {
        var currentPeriodCount = bookingStartUtcValues.Count(item => item >= periodStartUtc && item < periodEndUtc);
        return new SpacesBookingInstanceCount(currentPeriodCount, bookingStartUtcValues.Count - currentPeriodCount);
    }
}
