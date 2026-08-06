namespace Booking.Shared.Models;

public enum MarketplaceRefundNotificationDeliveryStatus { Pending, Sent, Failed }

public static class MarketplaceRefundNotificationDeliveryStatusConstants
{
    public const string Pending = "Pending";
    public const string Sent = "Sent";
    public const string Failed = "Failed";
}

public static class MarketplaceRefundNotificationDeliveryStatusExtensions
{
    public static MarketplaceRefundNotificationDeliveryStatus ToMarketplaceRefundNotificationDeliveryStatus(this string value) =>
        value switch
        {
            MarketplaceRefundNotificationDeliveryStatusConstants.Pending => MarketplaceRefundNotificationDeliveryStatus.Pending,
            MarketplaceRefundNotificationDeliveryStatusConstants.Sent => MarketplaceRefundNotificationDeliveryStatus.Sent,
            MarketplaceRefundNotificationDeliveryStatusConstants.Failed => MarketplaceRefundNotificationDeliveryStatus.Failed,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown marketplace refund notification delivery status."),
        };
}
