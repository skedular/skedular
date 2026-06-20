using Api.Shared.Services.Offering;
using HotChocolate;

namespace Booking.Api.GraphQL.Booking;

[GraphQLName("BookingSpacesQuotaStatusDetails")]
public class BookingSpacesQuotaStatusDetails
{
    [GraphQLName("organizationId")] public string OrganizationId { get; set; } = string.Empty;
    [GraphQLName("currentPeriodStartUtc")] public DateTimeOffset CurrentPeriodStartUtc { get; set; }
    [GraphQLName("currentPeriodEndUtc")] public DateTimeOffset CurrentPeriodEndUtc { get; set; }
    [GraphQLName("planCode")] public int? PlanCode { get; set; }
    [GraphQLName("quotaLimit")] public int? QuotaLimit { get; set; }
    [GraphQLName("currentUsage")] public int CurrentUsage { get; set; }

    [GraphQLName("attemptedCurrentPeriodCount")]
    public int AttemptedCurrentPeriodCount { get; set; }

    [GraphQLName("excludedOutOfPeriodCount")]
    public int ExcludedOutOfPeriodCount { get; set; }

    [GraphQLName("totalAttemptedInstanceCount")]
    public int TotalAttemptedInstanceCount { get; set; }

    [GraphQLName("remainingQuota")] public int? RemainingQuota { get; set; }
    [GraphQLName("quotaExceeded")] public bool QuotaExceeded { get; set; }
    [GraphQLName("reasonCode")] public SpacesQuotaReasonCodeDetails? ReasonCode { get; set; }
    [GraphQLName("upgradePlans")] public IReadOnlyCollection<UpgradePlanDetails> UpgradePlans { get; set; } = [];
}

[GraphQLName("SpacesQuotaReasonCodeDetails")]
public class SpacesQuotaReasonCodeDetails
{
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("type")] public SpacesQuotaReasonCode Type { get; set; }
}

[GraphQLName("UpgradePlanDetails")]
public class UpgradePlanDetails
{
    [GraphQLName("planCode")] public int PlanCode { get; set; }
    [GraphQLName("name")] public string Name { get; set; } = string.Empty;
    [GraphQLName("availability")] public string Availability { get; set; } = string.Empty;
    [GraphQLName("priceDescription")] public string? PriceDescription { get; set; }
}
