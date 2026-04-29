namespace Api.Shared.Services.Models;

public enum MarketplaceBookingSubscriptionStatus
{
    Active,
    Cancelled,
    Expired,
    RenewalFailed,
    Paused
}

public static class MarketplaceBookingSubscriptionStatusConstants
{
    public const string Active = "ACTIVE";
    public const string Cancelled = "CANCELLED";
    public const string Expired = "EXPIRED";
    public const string RenewalFailed = "RENEWAL_FAILED";
    public const string Paused = "PAUSED";
}

public static class MarketplaceBookingSubscriptionStatusExtensions
{
    extension(string src)
    {
        public MarketplaceBookingSubscriptionStatus ToMarketplaceBookingSubscriptionStatus() =>
            src switch
            {
                MarketplaceBookingSubscriptionStatusConstants.Active => MarketplaceBookingSubscriptionStatus.Active,
                MarketplaceBookingSubscriptionStatusConstants.Cancelled => MarketplaceBookingSubscriptionStatus.Cancelled,
                MarketplaceBookingSubscriptionStatusConstants.Expired => MarketplaceBookingSubscriptionStatus.Expired,
                MarketplaceBookingSubscriptionStatusConstants.RenewalFailed => MarketplaceBookingSubscriptionStatus.RenewalFailed,
                MarketplaceBookingSubscriptionStatusConstants.Paused => MarketplaceBookingSubscriptionStatus.Paused,
                _ => throw new ArgumentOutOfRangeException()
            };
    }

    extension(MarketplaceBookingSubscriptionStatus src)
    {
        public string ToMarketplaceBookingSubscriptionStatus() =>
            src switch
            {
                MarketplaceBookingSubscriptionStatus.Active => MarketplaceBookingSubscriptionStatusConstants.Active,
                MarketplaceBookingSubscriptionStatus.Cancelled => MarketplaceBookingSubscriptionStatusConstants.Cancelled,
                MarketplaceBookingSubscriptionStatus.Expired => MarketplaceBookingSubscriptionStatusConstants.Expired,
                MarketplaceBookingSubscriptionStatus.RenewalFailed => MarketplaceBookingSubscriptionStatusConstants.RenewalFailed,
                MarketplaceBookingSubscriptionStatus.Paused => MarketplaceBookingSubscriptionStatusConstants.Paused,
                _ => throw new ArgumentOutOfRangeException()
            };

        public string ToMarketplaceBookingSubscriptionStatusName() =>
            src switch
            {
                MarketplaceBookingSubscriptionStatus.Active => "Active",
                MarketplaceBookingSubscriptionStatus.Cancelled => "Cancelled",
                MarketplaceBookingSubscriptionStatus.Expired => "Expired",
                MarketplaceBookingSubscriptionStatus.RenewalFailed => "Renewal failed",
                MarketplaceBookingSubscriptionStatus.Paused => "Paused",
                _ => throw new ArgumentOutOfRangeException()
            };
    }
}
