namespace Booking.Shared.Models;

public enum MarketplaceRefundEntityType
{
    MarketplaceBooking,
    MarketplaceBookingSubscription,
}

public static class MarketplaceRefundEntityTypeConstants
{
    public const string MarketplaceBooking = "MarketplaceBooking";
    public const string MarketplaceBookingSubscription = "MarketplaceBookingSubscription";
}

public static class MarketplaceRefundEntityTypeExtensions
{
    public static string ToMarketplaceRefundEntityTypeValue(this MarketplaceRefundEntityType value) =>
        value switch
        {
            MarketplaceRefundEntityType.MarketplaceBooking => MarketplaceRefundEntityTypeConstants.MarketplaceBooking,
            MarketplaceRefundEntityType.MarketplaceBookingSubscription => MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription,
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown marketplace refund entity type."),
        };

    public static MarketplaceRefundEntityType ToMarketplaceRefundEntityType(this string? value) =>
        value switch
        {
            MarketplaceRefundEntityTypeConstants.MarketplaceBooking => MarketplaceRefundEntityType.MarketplaceBooking,
            MarketplaceRefundEntityTypeConstants.MarketplaceBookingSubscription => MarketplaceRefundEntityType.MarketplaceBookingSubscription,
            _ => MarketplaceRefundEntityType.MarketplaceBooking,
        };
}
