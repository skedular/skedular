namespace Booking.Shared.Models;

public static class MarketplaceRefundStatusConstants
{
    public const string Requested = "Requested";
    public const string PendingAccounting = "PendingAccounting";
    public const string ManualRequired = "ManualRequired";
    public const string ManualCompleted = "ManualCompleted";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
}

public static class MarketplaceRefundStatusExtensions
{
    public static string ToMarketplaceRefundStatusName(this string status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.Requested => "Requested",
            MarketplaceRefundStatusConstants.PendingAccounting => "Pending accounting",
            MarketplaceRefundStatusConstants.ManualRequired => "Manual follow-up required",
            MarketplaceRefundStatusConstants.ManualCompleted => "Completed manually",
            MarketplaceRefundStatusConstants.Completed => "Completed",
            MarketplaceRefundStatusConstants.Failed => "Failed",
            _ => status
        };
}
