namespace Booking.Shared.Models;

public sealed record MarketplaceAutomaticPaymentStatus(
    bool Configured,
    string Status,
    DateTimeOffset? CurrentPeriodEndsAt,
    bool CancelAtPeriodEnd);
