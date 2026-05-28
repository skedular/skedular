namespace Booking.Shared.Models;

/// <summary>
///     Result wrapper returned by <c>IResourceAvailabilityDayViewService.GetAsync</c>.
///     Contains the full filtered, sorted list of resource day views and an opaque subscription key.
/// </summary>
/// <param name="Items">The computed resource day views matching the query filter.</param>
/// <param name="SubscriptionKey">
///     Opaque, backend-generated key derived from the canonicalised filter.
///     The client must treat this as an opaque string — do not construct or interpret it.
///     Pass this value directly to <c>onResourceAvailabilityChanged</c> to subscribe to
///     real-time updates scoped to the same filter that produced this result.
/// </param>
public sealed record ResourceDayViewResult(
    IReadOnlyList<ResourceDayView> Items,
    string SubscriptionKey);
