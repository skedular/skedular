namespace Booking.Shared.Models;

public enum MarketplaceExternalRefundReconciliationStatus
{
    Open,
    Resolved,
    Rejected,
    LookupFailed,
    NotFound,
    Matched,
    Unsettled,
    AccountingProjectionRequired,
    UnknownStripeRefundPath,
    RetryPending,
}

public static class MarketplaceExternalRefundReconciliationStatusConstants
{
    public const string Open = "Open";
    public const string Resolved = "Resolved";
    public const string Rejected = "Rejected";
    public const string LookupFailed = "LookupFailed";
    public const string NotFound = "NotFound";
    public const string Matched = "Matched";
    public const string Unsettled = "Unsettled";
    public const string AccountingProjectionRequired = "AccountingProjectionRequired";
    public const string UnknownStripeRefundPath = "UnknownStripeRefundPath";
    public const string RetryPending = "RetryPending";
}

public static class MarketplaceExternalRefundReconciliationStatusExtensions
{
    public static MarketplaceExternalRefundReconciliationStatus ToMarketplaceExternalRefundReconciliationStatus(this string value) =>
        value switch
        {
            MarketplaceExternalRefundReconciliationStatusConstants.Open => MarketplaceExternalRefundReconciliationStatus.Open,
            MarketplaceExternalRefundReconciliationStatusConstants.Resolved => MarketplaceExternalRefundReconciliationStatus.Resolved,
            MarketplaceExternalRefundReconciliationStatusConstants.Rejected => MarketplaceExternalRefundReconciliationStatus.Rejected,
            MarketplaceExternalRefundReconciliationStatusConstants.LookupFailed => MarketplaceExternalRefundReconciliationStatus.LookupFailed,
            MarketplaceExternalRefundReconciliationStatusConstants.NotFound => MarketplaceExternalRefundReconciliationStatus.NotFound,
            MarketplaceExternalRefundReconciliationStatusConstants.Matched => MarketplaceExternalRefundReconciliationStatus.Matched,
            MarketplaceExternalRefundReconciliationStatusConstants.Unsettled => MarketplaceExternalRefundReconciliationStatus.Unsettled,
            MarketplaceExternalRefundReconciliationStatusConstants.AccountingProjectionRequired => MarketplaceExternalRefundReconciliationStatus
                .AccountingProjectionRequired,
            MarketplaceExternalRefundReconciliationStatusConstants.UnknownStripeRefundPath => MarketplaceExternalRefundReconciliationStatus
                .UnknownStripeRefundPath,
            MarketplaceExternalRefundReconciliationStatusConstants.RetryPending => MarketplaceExternalRefundReconciliationStatus.RetryPending,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown external refund reconciliation status."),
        };
}

public enum MarketplaceExternalRefundReconciliationProvider { Stripe, StripePayout }

public static class MarketplaceExternalRefundReconciliationProviderExtensions
{
    public static string ToMarketplaceExternalRefundReconciliationProviderValue(this MarketplaceExternalRefundReconciliationProvider value) =>
        value switch
        {
            MarketplaceExternalRefundReconciliationProvider.Stripe => "STRIPE",
            MarketplaceExternalRefundReconciliationProvider.StripePayout => MarketplaceExternalRefundReconciliationProviderConstants.StripePayout,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown external refund reconciliation provider."),
        };

    public static MarketplaceExternalRefundReconciliationProvider ToMarketplaceExternalRefundReconciliationProvider(this string value) =>
        value switch
        {
            "STRIPE" => MarketplaceExternalRefundReconciliationProvider.Stripe,
            MarketplaceExternalRefundReconciliationProviderConstants.StripePayout => MarketplaceExternalRefundReconciliationProvider.StripePayout,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown external refund reconciliation provider."),
        };
}
