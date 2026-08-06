using SharedOffering = Api.Shared.Services.Models.Offering;

namespace Api.Shared.Services.Offering;

public interface ISpacesAccessEvaluator
{
    SpacesAccessDecision Evaluate(DateTimeOffset nowUtc, SharedOffering? offering, SpacesAccessAction action);
}

public sealed class SpacesAccessEvaluator : ISpacesAccessEvaluator
{
    public const int ExpiringThresholdDays = 3;
    public static readonly TimeSpan TrialDuration = TimeSpan.FromDays(14);

    public SpacesAccessDecision Evaluate(DateTimeOffset nowUtc, SharedOffering? offering, SpacesAccessAction action)
    {
        if (offering is null)
        {
            return Missing(action, null, null, SpacesAccessReasonCode.MissingOfferingState);
        }

        if (!Offerings.SpacesOfferings.Contains(offering.Code))
        {
            return Allowed(
                SpacesSubscriptionStatus.LegacyActive,
                SpacesAccessReasonCode.AllowedPaid,
                action,
                offering,
                null,
                null,
                0,
                false);
        }

        if (offering.SpacesProductEnabled == false)
        {
            return Missing(action, offering.Code, offering, SpacesAccessReasonCode.MissingOfferingState);
        }

        if (offering.Code == OfferingCode.SpacesFreeTierV1)
        {
            return EvaluateTrial(nowUtc, offering, action);
        }

        return EvaluatePaid(nowUtc, offering, action);
    }

    private static SpacesAccessDecision EvaluateTrial(DateTimeOffset nowUtc, SharedOffering offering, SpacesAccessAction action)
    {
        // Legacy replicated Free offerings predate the explicit trial fields. Their
        // offering start is the durable creation-time anchor available to consumers.
        var trialStartedAt = offering.SpacesTrialStartedAt ?? offering.Start;
        var trialEndsAt = offering.SpacesTrialEndsAt ?? trialStartedAt.Add(TrialDuration);
        var remainingDays = nowUtc < trialEndsAt
            ? Math.Max(0, (int)Math.Ceiling((trialEndsAt - nowUtc).TotalDays))
            : 0;

        if (nowUtc < trialEndsAt)
        {
            var status = remainingDays <= ExpiringThresholdDays
                ? SpacesSubscriptionStatus.TrialExpiring
                : SpacesSubscriptionStatus.TrialActive;

            return Allowed(
                status,
                ReasonForAllowedAction(action, SpacesAccessReasonCode.AllowedTrial),
                action,
                offering,
                trialStartedAt,
                trialEndsAt,
                remainingDays,
                false);
        }

        return Restricted(
            SpacesSubscriptionStatus.TrialExpired,
            SpacesAccessReasonCode.TrialExpired,
            action,
            offering,
            trialStartedAt,
            trialEndsAt,
            true);
    }

    private static SpacesAccessDecision EvaluatePaid(DateTimeOffset nowUtc, SharedOffering offering, SpacesAccessAction action)
    {
        var isEffective = nowUtc >= offering.Start && nowUtc < offering.End;
        if (!isEffective)
        {
            return Restricted(
                SpacesSubscriptionStatus.PaidInactive,
                SpacesAccessReasonCode.PaidInactive,
                action,
                offering,
                offering.SpacesTrialStartedAt,
                offering.SpacesTrialEndsAt,
                true);
        }

        var isBridge = offering.SpacesNextBillingAt.HasValue &&
                       offering.SpacesNextBillingAt.Value > offering.Start &&
                       nowUtc < offering.SpacesNextBillingAt.Value;
        var status = isBridge ? SpacesSubscriptionStatus.ComplimentaryBridge : SpacesSubscriptionStatus.PaidActive;
        var defaultReason = isBridge
            ? SpacesAccessReasonCode.AllowedComplimentaryBridge
            : SpacesAccessReasonCode.AllowedPaid;

        return Allowed(
            status,
            ReasonForAllowedAction(action, defaultReason),
            action,
            offering,
            offering.SpacesTrialStartedAt,
            offering.SpacesTrialEndsAt,
            RemainingTrialDays(nowUtc, offering.SpacesTrialEndsAt),
            isBridge);
    }

    private static SpacesAccessDecision Missing(
        SpacesAccessAction action,
        OfferingCode? planCode,
        SharedOffering? offering,
        SpacesAccessReasonCode reasonCode)
    {
        var allowed = action is SpacesAccessAction.Read or
            SpacesAccessAction.ProtectExistingCommitment or
            SpacesAccessAction.AccountOrUpgrade;
        var resolvedReason = allowed
            ? ReasonForAllowedAction(action, SpacesAccessReasonCode.AllowedReadOrRecovery)
            : reasonCode;
        return new SpacesAccessDecision(
            allowed,
            SpacesSubscriptionStatus.MissingState,
            resolvedReason,
            action,
            planCode,
            offering?.SpacesTrialStartedAt,
            offering?.SpacesTrialEndsAt,
            0,
            false,
            false,
            true,
            true,
            offering?.SpacesNextBillingAt,
            false);
    }

    private static SpacesAccessDecision Restricted(
        SpacesSubscriptionStatus status,
        SpacesAccessReasonCode deniedReason,
        SpacesAccessAction action,
        SharedOffering offering,
        DateTimeOffset? trialStartedAt,
        DateTimeOffset? trialEndsAt,
        bool upgradeRequired)
    {
        var allowed = action is SpacesAccessAction.Read or
            SpacesAccessAction.ProtectExistingCommitment or
            SpacesAccessAction.AccountOrUpgrade;
        var reason = allowed
            ? ReasonForAllowedAction(action, SpacesAccessReasonCode.AllowedReadOrRecovery)
            : deniedReason;

        return new SpacesAccessDecision(
            allowed,
            status,
            reason,
            action,
            offering.Code,
            trialStartedAt,
            trialEndsAt,
            0,
            false,
            false,
            true,
            upgradeRequired,
            offering.SpacesNextBillingAt,
            false);
    }

    private static SpacesAccessDecision Allowed(
        SpacesSubscriptionStatus status,
        SpacesAccessReasonCode reasonCode,
        SpacesAccessAction action,
        SharedOffering offering,
        DateTimeOffset? trialStartedAt,
        DateTimeOffset? trialEndsAt,
        int remainingTrialDays,
        bool isBridge) =>
        new(
            true,
            status,
            reasonCode,
            action,
            offering.Code,
            trialStartedAt,
            trialEndsAt,
            remainingTrialDays,
            true,
            true,
            true,
            false,
            offering.SpacesNextBillingAt,
            isBridge);

    private static SpacesAccessReasonCode ReasonForAllowedAction(
        SpacesAccessAction action,
        SpacesAccessReasonCode defaultReason) =>
        action switch
        {
            SpacesAccessAction.Read or SpacesAccessAction.AccountOrUpgrade => SpacesAccessReasonCode.AllowedReadOrRecovery,
            SpacesAccessAction.ProtectExistingCommitment => SpacesAccessReasonCode.AllowedProtectiveAction,
            _ => defaultReason,
        };

    private static int RemainingTrialDays(DateTimeOffset nowUtc, DateTimeOffset? trialEndsAt) =>
        trialEndsAt.HasValue && nowUtc < trialEndsAt.Value
            ? Math.Max(0, (int)Math.Ceiling((trialEndsAt.Value - nowUtc).TotalDays))
            : 0;
}
