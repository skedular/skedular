namespace Booking.Shared.Models;

/// <summary>
///     A confirmed request to change the fulfillment window or resource assignment of an existing marketplace booking.
///     Commercial terms are intentionally absent: a modification can never reprice or repurchase the booking.
/// </summary>
public sealed record MarketplaceBookingModificationRequest(
    string BookingId,
    uint ExpectedVersion,
    DateTimeOffset From,
    DateTimeOffset Until,
    IReadOnlyCollection<string>? ResourceIds,
    string? Reason,
    string ActorCustomerId,
    MarketplaceBookingModificationActorKind ActorKind);

public enum MarketplaceBookingModificationActorKind
{
    Customer,
    OrganizationOperator,
}

public static class MarketplaceBookingModificationActorKindExtensions
{
    public static string ToMarketplaceBookingModificationActorKindValue(this MarketplaceBookingModificationActorKind value) => value switch
    {
        MarketplaceBookingModificationActorKind.Customer => MarketplaceBookingModificationActorKindConstants.Customer,
        MarketplaceBookingModificationActorKind.OrganizationOperator => MarketplaceBookingModificationActorKindConstants.OrganizationOperator,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static MarketplaceBookingModificationActorKind ToMarketplaceBookingModificationActorKind(this string? value) => value switch
    {
        MarketplaceBookingModificationActorKindConstants.Customer => MarketplaceBookingModificationActorKind.Customer,
        MarketplaceBookingModificationActorKindConstants.OrganizationOperator => MarketplaceBookingModificationActorKind.OrganizationOperator,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown marketplace booking modification actor kind."),
    };
}

public static class MarketplaceBookingModificationActorKindConstants
{
    public const string Customer = "Customer";
    public const string OrganizationOperator = "OrganizationOperator";
}

public enum MarketplaceBookingModificationErrorCode
{
    InvalidInput,
    NotEligible,
    Unauthorized,
    OperatorReasonRequired,
    StaleVersion,
    Unavailable,
    InvalidResourceSelection,
    OutsideSubscriptionCycle,
}

public enum MarketplaceBookingModificationNotificationDeliveryStatus
{
    Pending,
    Sent,
    RecoveryRequired,
}

public static class MarketplaceBookingModificationNotificationDeliveryStatusConstants
{
    public const string Pending = "Pending";
    public const string Sent = "Sent";
    public const string RecoveryRequired = "RecoveryRequired";
}

public static class MarketplaceBookingModificationNotificationDeliveryStatusExtensions
{
    public static string ToMarketplaceBookingModificationNotificationDeliveryStatusValue(
        this MarketplaceBookingModificationNotificationDeliveryStatus value) => value switch
    {
        MarketplaceBookingModificationNotificationDeliveryStatus.Pending => MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending,
        MarketplaceBookingModificationNotificationDeliveryStatus.Sent => MarketplaceBookingModificationNotificationDeliveryStatusConstants.Sent,
        MarketplaceBookingModificationNotificationDeliveryStatus.RecoveryRequired => MarketplaceBookingModificationNotificationDeliveryStatusConstants
            .RecoveryRequired,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
    };

    public static MarketplaceBookingModificationNotificationDeliveryStatus
        ToMarketplaceBookingModificationNotificationDeliveryStatus(this string? value) => value switch
    {
        MarketplaceBookingModificationNotificationDeliveryStatusConstants.Pending => MarketplaceBookingModificationNotificationDeliveryStatus.Pending,
        MarketplaceBookingModificationNotificationDeliveryStatusConstants.Sent => MarketplaceBookingModificationNotificationDeliveryStatus.Sent,
        MarketplaceBookingModificationNotificationDeliveryStatusConstants.RecoveryRequired => MarketplaceBookingModificationNotificationDeliveryStatus
            .RecoveryRequired,
        _ => throw new ArgumentOutOfRangeException(nameof(value), value, "Unknown marketplace booking modification notification status."),
    };
}

public sealed record MarketplaceBookingModificationResult(
    Booking? Booking,
    MarketplaceBookingModificationSummary? Modification,
    MarketplaceBookingModificationError? Error)
{
    public bool Succeeded => Booking is not null && Error is null;
}

public sealed record MarketplaceBookingModificationError(
    MarketplaceBookingModificationErrorCode Code,
    string Message,
    IReadOnlyCollection<string>? UnavailableResourceIds = null,
    Booking? CurrentBooking = null);

public sealed record MarketplaceBookingModificationSummary(
    string Id,
    string BookingId,
    DateTimeOffset OccurredAt,
    MarketplaceBookingModificationActorKind ActorKind,
    string? Reason,
    DateTimeOffset OriginalFrom,
    DateTimeOffset OriginalUntil,
    DateTimeOffset ResultFrom,
    DateTimeOffset ResultUntil,
    IReadOnlyCollection<string> OriginalResourceIds,
    IReadOnlyCollection<string> ResultResourceIds,
    IReadOnlyCollection<string> OriginalResourceNames,
    IReadOnlyCollection<string> ResultResourceNames,
    bool SubscriptionOccurrenceOverride);
