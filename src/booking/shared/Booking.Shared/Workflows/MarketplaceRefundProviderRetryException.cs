namespace Booking.Shared.Workflows;

/// <summary>
///     Signals Temporal that a provider result was persisted but should be retried.
///     Provider exceptions are persisted by the integration service so the failure is
///     visible immediately, while this exception keeps the activity retry policy active.
/// </summary>
public sealed class MarketplaceRefundProviderRetryException(string message) : Exception(message);
