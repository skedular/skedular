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

public static class MarketplaceBookingFailureResourceReleaseStatusConstants
{
    public const string Pending = "Pending";
    public const string Released = "Released";
}

public enum MarketplaceBookingFailureResourceReleaseStatus
{
    Unknown,
    Pending,
    Released,
}

public static class MarketplaceBookingFailureAccountingCleanupStatusConstants
{
    public const string NotRequired = "NotRequired";
    public const string Pending = "Pending";
    public const string TransitionRequired = "TransitionRequired";
}

public enum MarketplaceBookingFailureAccountingCleanupStatus
{
    Unknown,
    NotRequired,
    Pending,
    TransitionRequired,
}

public static class MarketplaceBookingFailureCleanupStatusExtensions
{
    public static MarketplaceBookingFailureResourceReleaseStatus ToResourceReleaseStatus(this string value) => value switch
    {
        MarketplaceBookingFailureResourceReleaseStatusConstants.Pending => MarketplaceBookingFailureResourceReleaseStatus.Pending,
        MarketplaceBookingFailureResourceReleaseStatusConstants.Released => MarketplaceBookingFailureResourceReleaseStatus.Released,
        _ => MarketplaceBookingFailureResourceReleaseStatus.Unknown,
    };

    public static string ToPersistedValue(this MarketplaceBookingFailureResourceReleaseStatus value) => value switch
    {
        MarketplaceBookingFailureResourceReleaseStatus.Pending => MarketplaceBookingFailureResourceReleaseStatusConstants.Pending,
        MarketplaceBookingFailureResourceReleaseStatus.Released => MarketplaceBookingFailureResourceReleaseStatusConstants.Released,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown resource release status cannot be persisted."),
    };

    public static MarketplaceBookingFailureAccountingCleanupStatus ToAccountingCleanupStatus(this string value) => value switch
    {
        MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired => MarketplaceBookingFailureAccountingCleanupStatus.NotRequired,
        MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending => MarketplaceBookingFailureAccountingCleanupStatus.Pending,
        MarketplaceBookingFailureAccountingCleanupStatusConstants.TransitionRequired => MarketplaceBookingFailureAccountingCleanupStatus
            .TransitionRequired,
        _ => MarketplaceBookingFailureAccountingCleanupStatus.Unknown,
    };

    public static string ToPersistedValue(this MarketplaceBookingFailureAccountingCleanupStatus value) => value switch
    {
        MarketplaceBookingFailureAccountingCleanupStatus.NotRequired => MarketplaceBookingFailureAccountingCleanupStatusConstants.NotRequired,
        MarketplaceBookingFailureAccountingCleanupStatus.Pending => MarketplaceBookingFailureAccountingCleanupStatusConstants.Pending,
        MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired => MarketplaceBookingFailureAccountingCleanupStatusConstants
            .TransitionRequired,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown accounting cleanup status cannot be persisted."),
    };

    public static string ToDisplayName(this MarketplaceBookingFailureResourceReleaseStatus value) => value switch
    {
        MarketplaceBookingFailureResourceReleaseStatus.Pending => "Pending",
        MarketplaceBookingFailureResourceReleaseStatus.Released => "Released",
        _ => "Unknown",
    };

    public static string ToDisplayName(this MarketplaceBookingFailureAccountingCleanupStatus value) => value switch
    {
        MarketplaceBookingFailureAccountingCleanupStatus.NotRequired => "Not required",
        MarketplaceBookingFailureAccountingCleanupStatus.Pending => "Pending",
        MarketplaceBookingFailureAccountingCleanupStatus.TransitionRequired => "Transition required",
        _ => "Unknown",
    };
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
