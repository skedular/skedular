using Api.Shared.Services.Models;
using Enterprise.Shared.Pagination;

namespace Booking.Shared.Models;

public static class MarketplacePurchaseHistorySourceTypeConstants
{
    public const string MarketplaceBooking = "MarketplaceBooking";
    public const string MarketplaceBookingSubscription = "MarketplaceBookingSubscription";
    public const string EntitlementPurchase = "EntitlementPurchase";
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
    string PaymentStatus,
    string? ProductVersionId,
    string? ProductTitle,
    decimal? TotalAmount,
    string? Currency,
    string? CustomerId,
    string OrganizationId,
    string? DeletedByCustomerId,
    string? CancellationReason,
    string? SubscriptionStatus,
    bool AutoRenew,
    bool CancelAtPeriodEnd,
    bool IsDeleted,
    string? RefundId = null,
    string? EntitlementStatus = null,
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
    string? EntitlementStatus = null,
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
