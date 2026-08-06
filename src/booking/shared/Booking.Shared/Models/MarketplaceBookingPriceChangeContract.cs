namespace Booking.Shared.Models;

/// <summary>
///     Server-authoritative inputs for a marketplace booking edit that may change price.
///     Clients submit these values; the booking pricing service calculates all amounts.
/// </summary>
public sealed record MarketplaceBookingPriceChangePatch(
    DateTimeOffset? From,
    DateTimeOffset? Until,
    int? Quantity,
    string? ProductPricingId,
    IReadOnlyCollection<string>? ResourceIds)
{
    public bool HasPriceAffectingChange =>
        From is not null || Until is not null || Quantity is not null || ProductPricingId is not null || ResourceIds is not null;
}

public enum MarketplaceRefundOwnershipScope
{
    OneTimeBooking,
    RecurringBillingWindow,
    SubscriptionBillingWindow,
}

/// <summary>Identifies the single billed entity responsible for a cancellation or modification refund.</summary>
public sealed record MarketplaceRefundOwnership(
    MarketplaceRefundOwnershipScope Scope,
    string LocalEntityType,
    string LocalEntityId,
    string? BookingId,
    string? RecurringBookingId,
    string? MarketplaceBookingSubscriptionId);
