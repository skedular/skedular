namespace Booking.Shared.Models;

public enum MarketplaceAutomaticPaymentStatusType
{
    Unknown,
    Pending,
    Active,
    Paused,
    PastDue,
    ActionRequired,
    FinalizationFailed,
    Incomplete,
    Canceled,
    Disconnected,
    Failed,
}

public sealed record MarketplaceAutomaticPaymentStatus(
    bool Configured,
    MarketplaceAutomaticPaymentStatusType Status,
    DateTimeOffset? CurrentPeriodEndsAt,
    bool CancelAtPeriodEnd);
