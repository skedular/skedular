namespace Booking.Shared.Models;

public static class MarketplaceRefundEventTypeConstants
{
    public const string Requested = "REQUESTED";
    public const string PendingAccounting = "PENDING_ACCOUNTING";
    public const string SentToXero = "SENT_TO_XERO";
    public const string ManualRequired = "MANUAL_REQUIRED";
    public const string ManualCompleted = "MANUAL_COMPLETED";
    public const string Completed = "COMPLETED";
    public const string Failed = "FAILED";
}

public static class MarketplaceRefundEventTypeConstantsExtensions
{
    public static string ToMarketplaceRefundEventTypeName(this string eventType) =>
        eventType switch
        {
            MarketplaceRefundEventTypeConstants.Requested => "Refund requested",
            MarketplaceRefundEventTypeConstants.PendingAccounting => "Pending accounting",
            MarketplaceRefundEventTypeConstants.SentToXero => "Sent to Xero",
            MarketplaceRefundEventTypeConstants.ManualRequired => "Manual follow-up required",
            MarketplaceRefundEventTypeConstants.ManualCompleted => "Completed manually",
            MarketplaceRefundEventTypeConstants.Completed => "Completed",
            MarketplaceRefundEventTypeConstants.Failed => "Failed",
            _ => eventType
        };
}
