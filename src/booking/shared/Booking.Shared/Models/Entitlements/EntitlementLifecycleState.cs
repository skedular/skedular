namespace Booking.Shared.Models.Entitlements;

public enum EntitlementRefundStatus
{
    NotEligible = 0,
    Pending = 1,
    Completed = 2,
    ManualSettlementRequired = 3,
    Failed = 4,
}

public static class EntitlementLifecycleStateExtensions
{
    public static string ToPersistedValue(this EntitlementStatus value) => value switch
    {
        EntitlementStatus.Pending => "PENDING",
        EntitlementStatus.Active => "ACTIVE",
        EntitlementStatus.Expired => "EXPIRED",
        EntitlementStatus.Cancelled => "CANCELLED",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported entitlement status."),
    };

    public static EntitlementStatus EntitlementStatusFromPersistedValue(string value) => value switch
    {
        "PENDING" => EntitlementStatus.Pending,
        "ACTIVE" => EntitlementStatus.Active,
        "EXPIRED" => EntitlementStatus.Expired,
        "CANCELLED" => EntitlementStatus.Cancelled,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported entitlement status value."),
    };

    public static string ToPersistedValue(this EntitlementRefundStatus value) => value switch
    {
        EntitlementRefundStatus.NotEligible => "NOT_ELIGIBLE",
        EntitlementRefundStatus.Pending => "PENDING",
        EntitlementRefundStatus.Completed => "COMPLETED",
        EntitlementRefundStatus.ManualSettlementRequired => "MANUAL_SETTLEMENT_REQUIRED",
        EntitlementRefundStatus.Failed => "FAILED",
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported refund status."),
    };

    public static EntitlementRefundStatus RefundStatusFromPersistedValue(string value) => value switch
    {
        "NOT_ELIGIBLE" => EntitlementRefundStatus.NotEligible,
        "PENDING" => EntitlementRefundStatus.Pending,
        "COMPLETED" => EntitlementRefundStatus.Completed,
        "MANUAL_SETTLEMENT_REQUIRED" => EntitlementRefundStatus.ManualSettlementRequired,
        "FAILED" => EntitlementRefundStatus.Failed,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unsupported refund status value."),
    };
}
