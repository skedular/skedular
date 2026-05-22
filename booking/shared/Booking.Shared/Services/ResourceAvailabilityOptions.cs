namespace Booking.Shared.Services;

public class ResourceAvailabilityOptions
{
    public int SlowQueryThresholdMs { get; init; } = 2000;
}
