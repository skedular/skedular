using Api.Shared.Services.Models;
using Booking.Shared.Models.Entitlements;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

public static class MarketplacePurchaseHistorySourceTypeConstants
{
    public const string MarketplaceBooking = "MarketplaceBooking";
    public const string MarketplaceBookingSubscription = "MarketplaceBookingSubscription";
    public const string EntitlementPurchase = "EntitlementPurchase";
}

public static class MarketplacePurchaseHistoryEventTypeConstants
{
    public const string PurchaseCreated = "PURCHASE_CREATED";
    public const string SubscriptionStarted = "SUBSCRIPTION_STARTED";
    public const string SubscriptionRenewed = "SUBSCRIPTION_RENEWED";
    public const string CancellationScheduled = "CANCELLATION_SCHEDULED";
    public const string CancellationCompleted = "CANCELLATION_COMPLETED";
    public const string EntitlementCreated = "ENTITLEMENT_CREATED";
    public const string EntitlementExpired = "ENTITLEMENT_EXPIRED";
    public const string CreditsConsumed = "CREDITS_CONSUMED";
    public const string PaymentStateChanged = "PAYMENT_STATE_CHANGED";
    public const string RefundStateChanged = "REFUND_STATE_CHANGED";
}

public enum MarketplacePurchaseHistoryEventType
{
    PurchaseCreated,
    SubscriptionStarted,
    SubscriptionRenewed,
    CancellationScheduled,
    CancellationCompleted,
    EntitlementCreated,
    EntitlementExpired,
    CreditsConsumed,
    PaymentStateChanged,
    RefundStateChanged,
}

public enum MarketplacePurchaseHistoryEligibleSourceType
{
    Subscription,
    Entitlement,
}

public static class MarketplacePurchaseHistoryEventMappings
{
    public static MarketplacePurchaseHistoryEventType ToEventType(this string value) => value switch
    {
        MarketplacePurchaseHistoryEventTypeConstants.PurchaseCreated => MarketplacePurchaseHistoryEventType.PurchaseCreated,
        MarketplacePurchaseHistoryEventTypeConstants.SubscriptionStarted => MarketplacePurchaseHistoryEventType.SubscriptionStarted,
        MarketplacePurchaseHistoryEventTypeConstants.SubscriptionRenewed => MarketplacePurchaseHistoryEventType.SubscriptionRenewed,
        MarketplacePurchaseHistoryEventTypeConstants.CancellationScheduled => MarketplacePurchaseHistoryEventType.CancellationScheduled,
        MarketplacePurchaseHistoryEventTypeConstants.CancellationCompleted => MarketplacePurchaseHistoryEventType.CancellationCompleted,
        MarketplacePurchaseHistoryEventTypeConstants.EntitlementCreated => MarketplacePurchaseHistoryEventType.EntitlementCreated,
        MarketplacePurchaseHistoryEventTypeConstants.EntitlementExpired => MarketplacePurchaseHistoryEventType.EntitlementExpired,
        MarketplacePurchaseHistoryEventTypeConstants.CreditsConsumed => MarketplacePurchaseHistoryEventType.CreditsConsumed,
        MarketplacePurchaseHistoryEventTypeConstants.PaymentStateChanged => MarketplacePurchaseHistoryEventType.PaymentStateChanged,
        MarketplacePurchaseHistoryEventTypeConstants.RefundStateChanged => MarketplacePurchaseHistoryEventType.RefundStateChanged,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown marketplace purchase history event type."),
    };

    public static MarketplacePurchaseHistoryEligibleSourceType ToEligibleSourceType(this string value) => value switch
    {
        MarketplacePurchaseHistorySourceTypeConstants.MarketplaceBookingSubscription => MarketplacePurchaseHistoryEligibleSourceType.Subscription,
        MarketplacePurchaseHistorySourceTypeConstants.EntitlementPurchase => MarketplacePurchaseHistoryEligibleSourceType.Entitlement,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Marketplace purchase history source is not eligible for event history."),
    };
}

public sealed record MarketplacePurchaseHistoryEventModel(
    string Id,
    string SourceId,
    MarketplacePurchaseHistoryEligibleSourceType SourceType,
    MarketplacePurchaseHistoryEventType Type,
    DateTimeOffset OccurredAt,
    DateTimeOffset RecordedAt,
    PaymentStatus? PreviousPaymentStatus,
    PaymentStatus? PaymentStatus,
    MarketplaceRefundStatus? PreviousRefundStatus,
    MarketplaceRefundStatus? RefundStatus,
    string? RefundId,
    int? CreditQuantity,
    int? RemainingCreditQuantity,
    decimal? Amount,
    Currency? Currency,
    DateTimeOffset? CancellationRequestedAt,
    DateTimeOffset? CancellationEffectiveAt,
    DateTimeOffset? RenewalAt,
    string? Reason,
    MarketplaceBookingSubscriptionStatus? SubscriptionStatus = null,
    EntitlementStatus? EntitlementStatus = null,
    bool? AutoRenew = null,
    bool? CancelAtPeriodEnd = null,
    bool? IsDeleted = null,
    string? CorrelationId = null);

public sealed record MarketplacePurchaseHistoryCurrentState(
    DateTimeOffset? PurchasedAt,
    DateTimeOffset? ActivityAt,
    PaymentStatus? PaymentStatus,
    MarketplaceRefundStatus? RefundStatus,
    string? RefundId,
    int? CreditQuantity,
    int? RemainingCreditQuantity,
    DateTimeOffset? CancellationRequestedAt,
    DateTimeOffset? CancellationEffectiveAt,
    DateTimeOffset? RenewalAt,
    MarketplaceBookingSubscriptionStatus? SubscriptionStatus,
    EntitlementStatus? EntitlementStatus,
    bool? AutoRenew,
    bool? CancelAtPeriodEnd,
    bool? IsDeleted);

public static class MarketplacePurchaseHistoryReducer
{
    public static MarketplacePurchaseHistoryCurrentState Reduce(
        IEnumerable<MarketplacePurchaseHistoryEventModel> events)
    {
        ArgumentNullException.ThrowIfNull(events);
        var ordered = events.OrderBy(item => item.OccurredAt).ThenBy(item => item.RecordedAt).ThenBy(item => item.Id).ToList();
        DateTimeOffset? purchasedAt = null;
        DateTimeOffset? activityAt = null;
        PaymentStatus? paymentStatus = null;
        MarketplaceRefundStatus? refundStatus = null;
        string? refundId = null;
        int? creditQuantity = null;
        int? remainingCreditQuantity = null;
        DateTimeOffset? cancellationRequestedAt = null;
        DateTimeOffset? cancellationEffectiveAt = null;
        DateTimeOffset? renewalAt = null;
        MarketplaceBookingSubscriptionStatus? subscriptionStatus = null;
        EntitlementStatus? entitlementStatus = null;
        bool? autoRenew = null;
        bool? cancelAtPeriodEnd = null;
        bool? isDeleted = null;

        foreach (var item in ordered)
        {
            activityAt = item.OccurredAt;
            if (item.Type == MarketplacePurchaseHistoryEventType.PurchaseCreated)
            {
                purchasedAt ??= item.OccurredAt;
            }

            if (item.PaymentStatus is not null)
            {
                paymentStatus = item.PaymentStatus;
            }

            if (item.RefundStatus is not null)
            {
                refundStatus = item.RefundStatus;
                refundId = item.RefundId;
            }

            if (item.CreditQuantity is not null)
            {
                creditQuantity = item.CreditQuantity;
            }

            if (item.RemainingCreditQuantity is not null)
            {
                remainingCreditQuantity = item.RemainingCreditQuantity;
            }

            cancellationRequestedAt = item.CancellationRequestedAt ?? cancellationRequestedAt;
            cancellationEffectiveAt = item.CancellationEffectiveAt ?? cancellationEffectiveAt;
            renewalAt = item.RenewalAt ?? renewalAt;
            subscriptionStatus = item.SubscriptionStatus ?? subscriptionStatus;
            entitlementStatus = item.EntitlementStatus ?? entitlementStatus;
            autoRenew = item.AutoRenew ?? autoRenew;
            cancelAtPeriodEnd = item.CancelAtPeriodEnd ?? cancelAtPeriodEnd;
            isDeleted = item.IsDeleted ?? isDeleted;
        }

        return new MarketplacePurchaseHistoryCurrentState(purchasedAt, activityAt, paymentStatus, refundStatus, refundId, creditQuantity,
            remainingCreditQuantity,
            cancellationRequestedAt, cancellationEffectiveAt, renewalAt, subscriptionStatus, entitlementStatus, autoRenew,
            cancelAtPeriodEnd, isDeleted);
    }
}

public enum MarketplacePurchaseSourceType
{
    Booking,
    Subscription,
    Entitlement,
}

public enum MarketplacePurchaseLifecycleState
{
    Active,
    Cancelled,
    Deleted,
    Expired,
    PaymentFailed,
    Pending,
}

public enum MarketplacePurchaseRenewalState
{
    NotApplicable,
    Renews,
    DoesNotRenew,
}

public enum MarketplacePurchaseHistoryOrderField
{
    ActivityAt,
    PurchasedAt,
    BookingFrom,
    BookingUntil,
}

public record MarketplacePurchaseHistorySearchCriteria(
    string? OrganizationCustomDomain,
    string? CustomerId,
    string? ProductVersionId,
    IReadOnlyList<MarketplacePurchaseSourceType>? SourceTypes = null,
    IReadOnlyList<MarketplacePurchaseLifecycleState>? LifecycleStates = null,
    IReadOnlyList<PaymentStatus>? PaymentStatuses = null,
    DateTimeOffset? ActivityFrom = null,
    DateTimeOffset? ActivityUntil = null,
    DateTimeOffset? BookingFrom = null,
    DateTimeOffset? BookingUntil = null,
    bool IncludeMineOnly = false);

public record MarketplacePurchaseHistoryOrder(OrderDirection Direction, MarketplacePurchaseHistoryOrderField Field);

/// <summary>
///     Database projection used to page the unified purchase feed. It intentionally contains
///     only scalar data: displaying purchase history must not hydrate recurring bookings or
///     start any subscription/resource reconciliation work.
/// </summary>
public sealed record MarketplacePurchaseHistoryRow(
    string Id,
    MarketplacePurchaseSourceType SourceType,
    DateTimeOffset PurchasedAt,
    DateTimeOffset ActivityAt,
    DateTimeOffset? BookingFrom,
    DateTimeOffset? BookingUntil,
    PaymentStatus PaymentStatus,
    string? ProductVersionId,
    string? ProductTitle,
    decimal? TotalAmount,
    Currency? Currency,
    string? CustomerId,
    string OrganizationId,
    string? DeletedByCustomerId,
    string? CancellationReason,
    MarketplaceBookingSubscriptionStatus? SubscriptionStatus,
    bool AutoRenew,
    bool CancelAtPeriodEnd,
    bool IsDeleted,
    string? RefundId = null,
    EntitlementStatus? EntitlementStatus = null,
    int CreditQuantity = 0,
    int GrantedQuantity = 0,
    int AvailableQuantity = 0);

public sealed record MarketplacePurchaseHistoryEntry(
    string Id,
    MarketplacePurchaseSourceType SourceType,
    MarketplacePurchaseLifecycleState LifecycleState,
    MarketplacePurchaseRenewalState RenewalState,
    DateTimeOffset PurchasedAt,
    DateTimeOffset ActivityAt,
    DateTimeOffset? BookingFrom,
    DateTimeOffset? BookingUntil,
    PaymentStatus PaymentStatus,
    string? ProductVersionId,
    string? ProductTitle,
    decimal? TotalAmount,
    Currency? Currency,
    string? CustomerId,
    bool IsDeleted,
    string? DeletedByCustomerId = null,
    string? CancellationReason = null,
    string? RefundId = null,
    string? BookingId = null,
    MarketplaceBooking? MarketplaceBooking = null,
    MarketplaceBookingSubscription? MarketplaceBookingSubscription = null,
    EntitlementStatus? EntitlementStatus = null,
    int CreditQuantity = 0,
    int GrantedQuantity = 0,
    int AvailableQuantity = 0,
    string? PaymentMethod = null)
{
    public string SourceTypeName => SourceType switch
    {
        MarketplacePurchaseSourceType.Booking => "One-time booking",
        MarketplacePurchaseSourceType.Subscription => "Subscription",
        MarketplacePurchaseSourceType.Entitlement => "Credit entitlement",
        _ => "Marketplace purchase",
    };

    public string LifecycleStateName => LifecycleState switch
    {
        MarketplacePurchaseLifecycleState.Active => "Active",
        MarketplacePurchaseLifecycleState.Cancelled => "Canceled",
        MarketplacePurchaseLifecycleState.Deleted => "Deleted",
        MarketplacePurchaseLifecycleState.Expired => "Expired",
        MarketplacePurchaseLifecycleState.PaymentFailed => "Payment failed",
        MarketplacePurchaseLifecycleState.Pending => "Pending",
        _ => "Unknown",
    };

    public string RenewalStateName => RenewalState switch
    {
        MarketplacePurchaseRenewalState.Renews => "Renews",
        MarketplacePurchaseRenewalState.DoesNotRenew => "Does not renew",
        _ => "Not applicable",
    };
}
