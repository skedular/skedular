namespace Booking.Shared.Services;

public class MarketplaceBookingAvailabilityConflict(
    IReadOnlyCollection<string> unavailableResourceIds,
    string? failureId = null)
    : Exception("The requested booking capacity is no longer available.")
{
    public IReadOnlyCollection<string> UnavailableResourceIds { get; } = unavailableResourceIds;

    /// <summary>
    ///     The ID of the durable <see cref="Booking.Shared.Database.Entities.MarketplaceBookingFailure" />
    ///     that was persisted before this exception was raised, when available.
    /// </summary>
    public string? FailureId { get; } = failureId;
}
