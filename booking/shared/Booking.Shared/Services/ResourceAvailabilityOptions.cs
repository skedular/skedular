namespace Booking.Shared.Services;

public sealed class ResourceAvailabilityOptions
{
    public int SlowQueryThresholdMs { get; init; } = 2000;
}
