namespace Booking.Shared.Models;

public sealed record MarketplaceBookingFailureFinalization(
    string? FailureKey,
    string Category,
    string Scope,
    DateTimeOffset FinalizedAt,
    string? BookingId,
    string? RecurringBookingId,
    string? MarketplaceBookingSubscriptionId,
    DateTimeOffset? RequestedFrom,
    DateTimeOffset? RequestedUntil,
    IReadOnlyCollection<string> RequestedResourceIds,
    string CustomerAction,
    string? CorrelationId,
    string? Reason,
    string? ActorCustomerId,
    IReadOnlyCollection<MarketplaceBookingFailureRecipient> Recipients);

public sealed record MarketplaceBookingFailureRecipient(
    string RecipientKey,
    string Audience,
    string Channel,
    string? RecipientCustomerId,
    string? RecipientEmail);

public static class MarketplaceBookingFailureKey
{
    public static string Create(MarketplaceBookingFailureFinalization finalization)
    {
        if (!string.IsNullOrWhiteSpace(finalization.FailureKey))
        {
            return finalization.FailureKey;
        }

        var owner = finalization.Scope switch
        {
            MarketplaceBookingFailureScopeConstants.OneTimeBooking => finalization.BookingId,
            MarketplaceBookingFailureScopeConstants.InitialSeries => finalization.MarketplaceBookingSubscriptionId ?? finalization.RecurringBookingId,
            MarketplaceBookingFailureScopeConstants.RecurringOccurrence => string.Concat(finalization.RecurringBookingId, ":",
                ToStableWindow(finalization.RequestedFrom)),
            MarketplaceBookingFailureScopeConstants.RecurringCycle => string.Concat(
                finalization.MarketplaceBookingSubscriptionId ?? finalization.RecurringBookingId, ":", ToStableWindow(finalization.RequestedFrom)),
            _ => null,
        };
        ArgumentException.ThrowIfNullOrWhiteSpace(owner);
        return string.Concat("marketplace-booking-failure:", finalization.Scope, ":", owner, ":", finalization.Category);
    }

    private static string ToStableWindow(DateTimeOffset? requestedFrom) =>
        requestedFrom?.ToUniversalTime().ToUnixTimeMilliseconds().ToString() ?? "none";
}
