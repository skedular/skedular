using Booking.Shared.Models;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingPayload")]
public class BookingPayload
{
    [GraphQLName("clientMutationId")] public string? ClientMutationId { get; set; }
    [GraphQLName("booking")] public BookingDetails? Booking { get; set; }
    [GraphQLName("quotaError")] public BookingSpacesQuotaErrorDetails? QuotaError { get; set; }
    [GraphQLName("accessError")] public SpacesAccessErrorDetails? AccessError { get; set; }
    [GraphQLName("availabilityError")] public BookingAvailabilityErrorDetails? AvailabilityError { get; set; }
    [GraphQLName("failure")] public MarketplaceBookingFailureDetails? Failure { get; set; }
    [GraphQLName("cancellationError")] public CancellationErrorDetails? CancellationError { get; set; }
}

[GraphQLName("CancellationErrorDetails")]
public class CancellationErrorDetails
{
    [GraphQLName("code")] public CancellationErrorCode Code { get; set; }
    [GraphQLName("message")] public string Message { get; set; } = string.Empty;
}

[GraphQLName("BookingAvailabilityErrorDetails")]
public class BookingAvailabilityErrorDetails
{
    [GraphQLName("message")] public string Message { get; set; } = string.Empty;

    [GraphQLName("unavailableResourceIds")]
    public IReadOnlyCollection<string> UnavailableResourceIds { get; set; } = [];
}

[GraphQLName("BookingSpacesQuotaErrorDetails")]
public class BookingSpacesQuotaErrorDetails
{
    [GraphQLName("errorCode")] public string ErrorCode { get; set; } = string.Empty;
    [GraphQLName("reasonCode")] public SpacesQuotaReasonCodeDetails? ReasonCode { get; set; }
    [GraphQLName("currentUsage")] public int CurrentUsage { get; set; }
    [GraphQLName("quotaLimit")] public int QuotaLimit { get; set; }

    [GraphQLName("attemptedCurrentPeriodCount")]
    public int AttemptedCurrentPeriodCount { get; set; }

    [GraphQLName("excludedOutOfPeriodCount")]
    public int ExcludedOutOfPeriodCount { get; set; }

    [GraphQLName("totalAttemptedInstanceCount")]
    public int TotalAttemptedInstanceCount { get; set; }

    [GraphQLName("remainingQuota")] public int RemainingQuota { get; set; }
    [GraphQLName("upgradePlans")] public IReadOnlyCollection<UpgradePlanDetails> UpgradePlans { get; set; } = [];
}
