namespace Booking.Shared.Models;

public static class MarketplaceBookingFailureCategoryConstants
{
    public const string AvailabilityConflict = "AvailabilityConflict";
    public const string PaymentFailed = "PaymentFailed";
    public const string PaymentExpired = "PaymentExpired";
}

public static class MarketplaceBookingFailureScopeConstants
{
    public const string OneTimeBooking = "OneTimeBooking";
    public const string InitialSeries = "InitialSeries";
    public const string RecurringOccurrence = "RecurringOccurrence";
    public const string RecurringCycle = "RecurringCycle";
}

public static class MarketplaceBookingFailureCustomerActionConstants
{
    public const string Rebook = "Rebook";
    public const string ReviewSubscription = "ReviewSubscription";
    public const string None = "None";
}

public static class MarketplaceBookingFailureEventTypeConstants
{
    public const string Finalized = "Finalized";
    public const string CapacityReleased = "CapacityReleased";
    public const string DispatchQueued = "DispatchQueued";
    public const string DeliverySucceeded = "DeliverySucceeded";
    public const string DeliveryFailed = "DeliveryFailed";
    public const string ResolutionAccepted = "ResolutionAccepted";
    public const string ResolutionDeclined = "ResolutionDeclined";
    public const string ResolutionExpired = "ResolutionExpired";
}

public static class MarketplaceBookingFailureResolutionDecisionConstants
{
    public const string Accepted = "Accepted";
    public const string Declined = "Declined";
    public const string Expired = "Expired";
}

public static class MarketplaceBookingFailureDeliveryAudienceConstants
{
    public const string Customer = "Customer";
    public const string SpacesStakeholder = "SpacesStakeholder";
    public const string HostStakeholder = "HostStakeholder";
}

public static class MarketplaceBookingFailureDeliveryChannelConstants
{
    public const string InApplication = "InApplication";
    public const string Email = "Email";
}

public static class MarketplaceBookingFailureDeliveryStatusConstants
{
    public const string Pending = "Pending";
    public const string Sent = "Sent";
    public const string Skipped = "Skipped";
    public const string Failed = "Failed";
}

public static class MarketplaceBookingFailureExtensions
{
    public static string ToMarketplaceBookingFailureCategoryName(this string value) => value switch
    {
        MarketplaceBookingFailureCategoryConstants.AvailabilityConflict => "Availability conflict",
        MarketplaceBookingFailureCategoryConstants.PaymentFailed => "Payment failed",
        MarketplaceBookingFailureCategoryConstants.PaymentExpired => "Payment expired",
        _ => value,
    };

    public static string ToMarketplaceBookingFailureScopeName(this string value) => value switch
    {
        MarketplaceBookingFailureScopeConstants.OneTimeBooking => "One-time booking",
        MarketplaceBookingFailureScopeConstants.InitialSeries => "Initial series",
        MarketplaceBookingFailureScopeConstants.RecurringOccurrence => "Recurring occurrence",
        MarketplaceBookingFailureScopeConstants.RecurringCycle => "Recurring cycle",
        _ => value,
    };

    public static string ToMarketplaceBookingFailureCustomerActionName(this string value) => value switch
    {
        MarketplaceBookingFailureCustomerActionConstants.Rebook => "Book again",
        MarketplaceBookingFailureCustomerActionConstants.ReviewSubscription => "Review subscription",
        MarketplaceBookingFailureCustomerActionConstants.None => "No action needed",
        _ => value,
    };

    public static string ToMarketplaceBookingFailureDeliveryAudienceName(this string value) => value switch
    {
        MarketplaceBookingFailureDeliveryAudienceConstants.Customer => "Customer",
        MarketplaceBookingFailureDeliveryAudienceConstants.SpacesStakeholder => "Spaces stakeholder",
        MarketplaceBookingFailureDeliveryAudienceConstants.HostStakeholder => "Host stakeholder",
        _ => value,
    };

    public static string ToMarketplaceBookingFailureDeliveryChannelName(this string value) => value switch
    {
        MarketplaceBookingFailureDeliveryChannelConstants.InApplication => "In application",
        MarketplaceBookingFailureDeliveryChannelConstants.Email => "Email",
        _ => value,
    };

    public static string ToMarketplaceBookingFailureDeliveryStatusName(this string value) => value switch
    {
        MarketplaceBookingFailureDeliveryStatusConstants.Pending => "Pending",
        MarketplaceBookingFailureDeliveryStatusConstants.Sent => "Sent",
        MarketplaceBookingFailureDeliveryStatusConstants.Skipped => "Skipped",
        MarketplaceBookingFailureDeliveryStatusConstants.Failed => "Failed",
        _ => value,
    };
}
