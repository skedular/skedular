namespace Booking.Shared.Models.Entitlements;

public enum EntitlementPurchaseLifecycleState
{
    Pending,
    Confirmed,
    Completed,
    Expired,
    Rejected,
    RenewalFailed,
}
