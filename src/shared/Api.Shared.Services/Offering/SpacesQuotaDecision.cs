namespace Api.Shared.Services.Offering;

public record SpacesQuotaUpgradePlan(
    int PlanCode,
    string Name,
    string Availability,
    string? PriceDescription);

public record SpacesQuotaDecision(
    bool CanCreate,
    SpacesQuotaReasonCode ReasonCode,
    int? PlanCode,
    int CurrentUsage,
    int QuotaLimit,
    int AttemptedCurrentPeriodCount,
    int ExcludedOutOfPeriodCount,
    int RemainingQuota,
    DateTimeOffset CurrentPeriodStartUtc,
    DateTimeOffset CurrentPeriodEndUtc)
{
    public IReadOnlyList<SpacesQuotaUpgradePlan> UpgradePlans { get; init; } = [];
    public SpacesAccessDecision? AccessDecision { get; init; }

    public int TotalAttemptedInstanceCount => AttemptedCurrentPeriodCount + ExcludedOutOfPeriodCount;
}

public static class SpacesQuotaDecisionExtensions
{
    public static SpacesQuotaDecision WithUpgradePlans(this SpacesQuotaDecision decision, IReadOnlyList<SpacesQuotaUpgradePlan> upgradePlans) =>
        decision with
        {
            UpgradePlans = upgradePlans,
        };
}
