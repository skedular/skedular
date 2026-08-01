namespace Booking.Shared.Models;

public enum MarketplaceRefundStatus
{
    Requested, UnderReview, Approved, Rejected, Processing, ProviderPending, Completed, Failed, Cancelled, ReconciliationRequired
}

public static class MarketplaceRefundStatusConstants
{
    public const string Requested = "Requested";
    public const string UnderReview = "UnderReview";
    public const string Approved = "Approved";
    public const string Rejected = "Rejected";
    public const string ProviderPending = "ProviderPending";
    public const string Processing = "Processing";
    public const string Completed = "Completed";
    public const string Failed = "Failed";
    public const string Cancelled = "Cancelled";
    public const string ReconciliationRequired = "ReconciliationRequired";
}

public static class MarketplaceRefundStatusExtensions
{
    public static MarketplaceRefundStatus ToMarketplaceRefundStatus(this string? status) =>
        status switch
        {
            MarketplaceRefundStatusConstants.Requested => MarketplaceRefundStatus.Requested,
            MarketplaceRefundStatusConstants.UnderReview => MarketplaceRefundStatus.UnderReview,
            MarketplaceRefundStatusConstants.Approved => MarketplaceRefundStatus.Approved,
            MarketplaceRefundStatusConstants.Rejected => MarketplaceRefundStatus.Rejected,
            MarketplaceRefundStatusConstants.Processing => MarketplaceRefundStatus.Processing,
            MarketplaceRefundStatusConstants.ProviderPending => MarketplaceRefundStatus.ProviderPending,
            MarketplaceRefundStatusConstants.Completed => MarketplaceRefundStatus.Completed,
            MarketplaceRefundStatusConstants.Failed => MarketplaceRefundStatus.Failed,
            MarketplaceRefundStatusConstants.Cancelled => MarketplaceRefundStatus.Cancelled,
            MarketplaceRefundStatusConstants.ReconciliationRequired => MarketplaceRefundStatus.ReconciliationRequired,
            _ => MarketplaceRefundStatus.Requested
        };

    public static string ToMarketplaceRefundStatusName(this MarketplaceRefundStatus status) =>
        status switch
        {
            MarketplaceRefundStatus.Requested => "Requested",
            MarketplaceRefundStatus.UnderReview => "Under review",
            MarketplaceRefundStatus.Approved => "Approved",
            MarketplaceRefundStatus.Rejected => "Rejected",
            MarketplaceRefundStatus.ProviderPending => "Provider pending",
            MarketplaceRefundStatus.Processing => "Processing",
            MarketplaceRefundStatus.Completed => "Completed",
            MarketplaceRefundStatus.Failed => "Failed",
            MarketplaceRefundStatus.Cancelled => "Cancelled",
            MarketplaceRefundStatus.ReconciliationRequired => "Reconciliation required",
            _ => throw new ArgumentOutOfRangeException(
                nameof(status),
                status,
                $"Unexpected value for {nameof(status)}: {status}. Update enum mapping or caller input.")
        };
}
