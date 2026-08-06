namespace Booking.Shared.Models;

public enum MarketplaceRefundEventType
{
    Requested, UnderReview, Approved, Rejected, ProviderPending, Processing, SentToXero, AccountingProjected, AccountingProjectionRequired, Completed,
    Failed, Cancelled, ReconciliationRequired,
}

public static class MarketplaceRefundEventTypeConstants
{
    public const string Requested = "REQUESTED";
    public const string UnderReview = "UNDER_REVIEW";
    public const string Approved = "APPROVED";
    public const string Rejected = "REJECTED";
    public const string ProviderPending = "PROVIDER_PENDING";
    public const string Processing = "PROCESSING";
    public const string SentToXero = "SENT_TO_XERO";
    public const string AccountingProjected = "ACCOUNTING_PROJECTED";
    public const string AccountingProjectionRequired = "ACCOUNTING_PROJECTION_REQUIRED";
    public const string Completed = "COMPLETED";
    public const string Failed = "FAILED";
    public const string Cancelled = "CANCELLED";
    public const string ReconciliationRequired = "RECONCILIATION_REQUIRED";
}

public static class MarketplaceRefundEventTypeConstantsExtensions
{
    public static MarketplaceRefundEventType ToMarketplaceRefundEventType(this string? eventType) =>
        eventType switch
        {
            MarketplaceRefundEventTypeConstants.Requested => MarketplaceRefundEventType.Requested,
            MarketplaceRefundEventTypeConstants.UnderReview => MarketplaceRefundEventType.UnderReview,
            MarketplaceRefundEventTypeConstants.Approved => MarketplaceRefundEventType.Approved,
            MarketplaceRefundEventTypeConstants.Rejected => MarketplaceRefundEventType.Rejected,
            MarketplaceRefundEventTypeConstants.ProviderPending => MarketplaceRefundEventType.ProviderPending,
            MarketplaceRefundEventTypeConstants.Processing => MarketplaceRefundEventType.Processing,
            MarketplaceRefundEventTypeConstants.SentToXero => MarketplaceRefundEventType.SentToXero,
            MarketplaceRefundEventTypeConstants.AccountingProjected => MarketplaceRefundEventType.AccountingProjected,
            MarketplaceRefundEventTypeConstants.AccountingProjectionRequired => MarketplaceRefundEventType.AccountingProjectionRequired,
            MarketplaceRefundEventTypeConstants.Completed => MarketplaceRefundEventType.Completed,
            MarketplaceRefundEventTypeConstants.Failed => MarketplaceRefundEventType.Failed,
            MarketplaceRefundEventTypeConstants.Cancelled => MarketplaceRefundEventType.Cancelled,
            MarketplaceRefundEventTypeConstants.ReconciliationRequired => MarketplaceRefundEventType.ReconciliationRequired,
            _ => MarketplaceRefundEventType.Requested,
        };

    public static string ToMarketplaceRefundEventTypeName(this MarketplaceRefundEventType eventType) =>
        eventType switch
        {
            MarketplaceRefundEventType.Requested => "Refund requested",
            MarketplaceRefundEventType.UnderReview => "Refund under review",
            MarketplaceRefundEventType.Approved => "Refund approved",
            MarketplaceRefundEventType.Rejected => "Refund rejected",
            MarketplaceRefundEventType.ProviderPending => "Provider pending",
            MarketplaceRefundEventType.Processing => "Processing",
            MarketplaceRefundEventType.SentToXero => "Sent to Xero",
            MarketplaceRefundEventType.AccountingProjected => "Accounting projection completed",
            MarketplaceRefundEventType.AccountingProjectionRequired => "Accounting projection requires attention",
            MarketplaceRefundEventType.Completed => "Completed",
            MarketplaceRefundEventType.Failed => "Failed",
            MarketplaceRefundEventType.Cancelled => "Refund cancelled",
            MarketplaceRefundEventType.ReconciliationRequired => "Reconciliation required",
            _ => throw new ArgumentOutOfRangeException(nameof(eventType), eventType, "Unknown refund event type."),
        };

    public static string ToMarketplaceRefundEventTypeName(this string eventType) =>
        eventType.ToMarketplaceRefundEventType().ToMarketplaceRefundEventTypeName();
}
