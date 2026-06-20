namespace Api.Shared.Services.Offering;

public sealed record SpacesAccessDecision(
    bool Allowed,
    SpacesSubscriptionStatus Status,
    SpacesAccessReasonCode ReasonCode,
    SpacesAccessAction Action,
    OfferingCode? PlanCode,
    DateTimeOffset? TrialStartedAt,
    DateTimeOffset? TrialEndsAt,
    int RemainingTrialDays,
    bool CanUseProduct,
    bool CanAcceptBookings,
    bool CanProtectExistingCommitments,
    bool UpgradeRequired,
    DateTimeOffset? NextBillingAt,
    bool IsComplimentaryBridge);
