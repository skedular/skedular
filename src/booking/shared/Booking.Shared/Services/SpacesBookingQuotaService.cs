using Api.Shared.Services.Offering;
using Booking.Shared.Logging;
using Booking.Shared.Repositories;
using Microsoft.Extensions.Logging;

namespace Booking.Shared.Services;

public interface ISpacesBookingQuotaService
{
    Task<SpacesAccessDecision> EvaluateAccessAsync(
        string organizationId,
        SpacesAccessAction action,
        CancellationToken cancellationToken);

    Task<SpacesQuotaDecision> GetQuotaStatusAsync(
        string organizationId,
        CancellationToken cancellationToken);

    Task<SpacesQuotaDecision> CanCreateBookingInstanceAsync(
        string organizationId,
        DateTimeOffset bookingStartUtc,
        CancellationToken cancellationToken);

    Task<SpacesQuotaDecision> TryReserveBookingInstancesAsync(
        string organizationId,
        IReadOnlyList<DateTimeOffset> bookingStartUtcValues,
        CancellationToken cancellationToken);
}

public class SpacesBookingQuotaService(
    IRepositoryFactory repositoryFactory,
    ISpacesBookingInstanceCounter spacesBookingInstanceCounter,
    ISpacesAccessEvaluator spacesAccessEvaluator,
    TimeProvider timeProvider,
    ILogger<SpacesBookingQuotaService> logger) : ISpacesBookingQuotaService
{
    private const int FreePlanCode = 1;
    private const int LegacyEarlyBirdPlanCode = 4;
    private const int GrowthPlanCode = 5;
    private const int BusinessPlanCode = 6;
    private const int ContactUsPlanCode = 7;

    private static readonly HashSet<int> s_supportedPlanCodes =
    [
        FreePlanCode,
        LegacyEarlyBirdPlanCode,
        GrowthPlanCode,
        BusinessPlanCode,
        ContactUsPlanCode,
    ];

    public async Task<SpacesQuotaDecision> GetQuotaStatusAsync(
        string organizationId,
        CancellationToken cancellationToken) =>
        await EvaluateAsync(organizationId, [], cancellationToken);

    public async Task<SpacesQuotaDecision> CanCreateBookingInstanceAsync(
        string organizationId,
        DateTimeOffset bookingStartUtc,
        CancellationToken cancellationToken) =>
        await EvaluateAsync(organizationId, [bookingStartUtc], cancellationToken);

    public async Task<SpacesQuotaDecision> TryReserveBookingInstancesAsync(
        string organizationId,
        IReadOnlyList<DateTimeOffset> bookingStartUtcValues,
        CancellationToken cancellationToken) =>
        await EvaluateAsync(organizationId, bookingStartUtcValues, cancellationToken);

    public async Task<SpacesAccessDecision> EvaluateAccessAsync(
        string organizationId,
        SpacesAccessAction action,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(
            organizationId, cancellationToken);

        var decision = spacesAccessEvaluator.Evaluate(timeProvider.GetUtcNow(), organization?.Offering, action);
        logger.Log(
            decision.Allowed ? LogLevel.Information : LogLevel.Warning,
            decision.Allowed ? SpacesTrialLogEvents.AccessDecisionAllowed : SpacesTrialLogEvents.AccessDecisionDenied,
            "Spaces access decision for organization {OrganizationId}. Action: {Action}, Status: {Status}, ReasonCode: {ReasonCode}, Allowed: {Allowed}",
            organizationId,
            action,
            decision.Status,
            decision.ReasonCode,
            decision.Allowed);
        return decision;
    }

    private async Task<SpacesQuotaDecision> EvaluateAsync(
        string organizationId,
        IReadOnlyList<DateTimeOffset> bookingStartUtcValues,
        CancellationToken cancellationToken)
    {
        var organization = await repositoryFactory.SpacesBookingUsageRepository.GetOrganizationWithOfferingAsync(
            organizationId, cancellationToken);

        var offering = organization?.Offering;
        var planCode = offering?.SpacesPlanCode;

        if (planCode is null or 0 || !s_supportedPlanCodes.Contains(planCode.Value))
        {
            logger.LogWarning(
                "{EventId}: Spaces offering state missing or invalid for organization {OrganizationId}. PlanCode: {PlanCode}",
                SpacesPricingLogEvents.QuotaDecisionNoOfferingState,
                organizationId,
                planCode);

            return new SpacesQuotaDecision(
                false,
                SpacesQuotaReasonCode.MissingOfferingState,
                planCode,
                0,
                0,
                0,
                bookingStartUtcValues.Count,
                0,
                DateTimeOffset.MinValue,
                DateTimeOffset.MinValue);
        }

        var accessDecision = spacesAccessEvaluator.Evaluate(
            timeProvider.GetUtcNow(),
            offering,
            SpacesAccessAction.CreateBookingInstance);
        if (!accessDecision.Allowed)
        {
            logger.LogInformation(
                "{EventId}: Spaces access denied for organization {OrganizationId}. Status: {Status}, ReasonCode: {ReasonCode}",
                SpacesTrialLogEvents.AccessDecisionDenied,
                organizationId,
                accessDecision.Status,
                accessDecision.ReasonCode);

            return CreateDecision(
                    false,
                    accessDecision.Status == SpacesSubscriptionStatus.TrialExpired
                        ? SpacesQuotaReasonCode.TrialExpired
                        : SpacesQuotaReasonCode.MissingOfferingState,
                    planCode,
                    0,
                    0,
                    bookingStartUtcValues.Count,
                    0,
                    offering!.SpacesPeriodStart ?? DateTimeOffset.MinValue,
                    offering.SpacesPeriodEnd ?? DateTimeOffset.MinValue)
                with
                {
                    AccessDecision = accessDecision,
                };
        }

        var periodStart = offering!.SpacesPeriodStart ?? DateTimeOffset.MinValue;
        var periodEnd = offering.SpacesPeriodEnd ?? DateTimeOffset.MinValue;
        if (periodStart == DateTimeOffset.MinValue || periodEnd == DateTimeOffset.MinValue || periodEnd <= periodStart)
        {
            logger.LogWarning(
                "{EventId}: Spaces offering state has invalid period for organization {OrganizationId}. PeriodStart: {PeriodStart}, PeriodEnd: {PeriodEnd}",
                SpacesPricingLogEvents.QuotaDecisionNoOfferingState,
                organizationId,
                periodStart,
                periodEnd);

            return new SpacesQuotaDecision(
                false,
                SpacesQuotaReasonCode.MissingOfferingState,
                planCode,
                0,
                0,
                0,
                bookingStartUtcValues.Count,
                0,
                periodStart,
                periodEnd);
        }

        var quotaLimit = GetEffectiveQuotaLimit(planCode.Value, offering.SpacesQuotaLimit, offering.SpacesCustomCapacity);
        var currentUsage = await repositoryFactory.SpacesBookingUsageRepository.CountCurrentPeriodBookingInstancesAsync(
            organizationId,
            periodStart,
            periodEnd,
            cancellationToken);
        if (bookingStartUtcValues.Count == 0)
        {
            return quotaLimit >= 0 && currentUsage >= quotaLimit
                ? Block(organizationId, planCode.Value, currentUsage, quotaLimit, 0, 0, periodStart, periodEnd)
                : CreateDecision(
                    true,
                    SpacesQuotaReasonCode.WithinQuota,
                    planCode.Value,
                    currentUsage,
                    quotaLimit,
                    0,
                    0,
                    periodStart,
                    periodEnd);
        }

        var instanceCount = spacesBookingInstanceCounter.CountCurrentPeriodInstances(
            bookingStartUtcValues,
            periodStart,
            periodEnd);
        var excludedOutOfPeriodCount = instanceCount.ExcludedOutOfPeriodCount;
        var attemptedCurrentPeriodCount = instanceCount.CurrentPeriodCount;

        if (attemptedCurrentPeriodCount == 0)
        {
            logger.LogInformation(
                "{EventId}: Spaces booking instances excluded from quota because they are outside the current billing period. OrganizationId: {OrganizationId}, ExcludedCount: {ExcludedCount}",
                SpacesPricingLogEvents.QuotaCheckOutOfPeriodExcluded,
                organizationId,
                excludedOutOfPeriodCount);

            return CreateDecision(
                true,
                SpacesQuotaReasonCode.OutOfPeriodExcluded,
                planCode.Value,
                0,
                quotaLimit,
                0,
                excludedOutOfPeriodCount,
                periodStart,
                periodEnd);
        }

        var canCreate = quotaLimit < 0 || currentUsage + attemptedCurrentPeriodCount <= quotaLimit;
        if (!canCreate)
        {
            return Block(
                organizationId,
                planCode.Value,
                currentUsage,
                quotaLimit,
                attemptedCurrentPeriodCount,
                excludedOutOfPeriodCount,
                periodStart,
                periodEnd);
        }

        logger.LogInformation(
            "{EventId}: Spaces booking quota allowed. OrganizationId: {OrganizationId}, Usage: {CurrentUsage}, QuotaLimit: {QuotaLimit}, AttemptedCurrentPeriodCount: {AttemptedCurrentPeriodCount}, ExcludedOutOfPeriodCount: {ExcludedOutOfPeriodCount}",
            SpacesPricingLogEvents.QuotaDecisionWithinQuota,
            organizationId,
            currentUsage,
            quotaLimit,
            attemptedCurrentPeriodCount,
            excludedOutOfPeriodCount);

        return CreateDecision(
            true,
            SpacesQuotaReasonCode.WithinQuota,
            planCode.Value,
            currentUsage,
            quotaLimit,
            attemptedCurrentPeriodCount,
            excludedOutOfPeriodCount,
            periodStart,
            periodEnd);
    }

    private SpacesQuotaDecision Block(
        string organizationId,
        int planCode,
        int currentUsage,
        int quotaLimit,
        int attemptedCurrentPeriodCount,
        int excludedOutOfPeriodCount,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd)
    {
        var reasonCode = GetBlockedReasonCode(planCode, quotaLimit);
        logger.LogInformation(
            "{EventId}: Spaces booking quota blocked. OrganizationId: {OrganizationId}, ReasonCode: {ReasonCode}, Usage: {CurrentUsage}, QuotaLimit: {QuotaLimit}, AttemptedCurrentPeriodCount: {AttemptedCurrentPeriodCount}, ExcludedOutOfPeriodCount: {ExcludedOutOfPeriodCount}",
            reasonCode == SpacesQuotaReasonCode.FreeTierLimitExceeded
                ? SpacesPricingLogEvents.QuotaDecisionFreeTierExceeded
                : SpacesPricingLogEvents.QuotaDecisionPaidTierExceeded,
            organizationId,
            reasonCode,
            currentUsage,
            quotaLimit,
            attemptedCurrentPeriodCount,
            excludedOutOfPeriodCount);

        var upgradePlans = GetUpgradePlans(planCode);
        var decision = CreateDecision(
            false,
            reasonCode,
            planCode,
            currentUsage,
            quotaLimit,
            attemptedCurrentPeriodCount,
            excludedOutOfPeriodCount,
            periodStart,
            periodEnd);
        return upgradePlans.Count > 0 ? decision.WithUpgradePlans(upgradePlans) : decision;
    }

    private static SpacesQuotaDecision CreateDecision(
        bool canCreate,
        SpacesQuotaReasonCode reasonCode,
        int? planCode,
        int currentUsage,
        int quotaLimit,
        int attemptedCurrentPeriodCount,
        int excludedOutOfPeriodCount,
        DateTimeOffset periodStart,
        DateTimeOffset periodEnd) =>
        new(
            canCreate,
            reasonCode,
            planCode,
            currentUsage,
            quotaLimit,
            attemptedCurrentPeriodCount,
            excludedOutOfPeriodCount,
            quotaLimit < 0 ? int.MaxValue : Math.Max(0, quotaLimit - currentUsage),
            periodStart,
            periodEnd);

    private static int GetEffectiveQuotaLimit(int planCode, int? quotaLimit, int? customCapacity)
    {
        // Early Bird is permanently unlimited. Ignore stale replicated quota values
        // such as 100 or -1 from older organization projections.
        if (planCode == LegacyEarlyBirdPlanCode)
        {
            return -1;
        }

        if (customCapacity.HasValue)
        {
            return customCapacity.Value;
        }

        if (quotaLimit.HasValue)
        {
            return quotaLimit.Value;
        }

        return planCode switch
        {
            FreePlanCode => OfferingCode.SpacesFreeTierV1.GetOffering().MaxBookingInstanceCount ?? -1,
            GrowthPlanCode => OfferingCode.SpacesGrowthV1.GetOffering().MaxBookingInstanceCount ?? -1,
            BusinessPlanCode => OfferingCode.SpacesBusinessV1.GetOffering().MaxBookingInstanceCount ?? -1,
            ContactUsPlanCode => -1,
            _ => throw new ArgumentOutOfRangeException(nameof(planCode), planCode, "Unsupported Spaces plan code."),
        };
    }

    private static SpacesQuotaReasonCode GetBlockedReasonCode(int planCode, int quotaLimit) =>
        planCode switch
        {
            FreePlanCode => SpacesQuotaReasonCode.FreeTierLimitExceeded,
            ContactUsPlanCode => SpacesQuotaReasonCode.CustomCapacityExceeded,
            _ => SpacesQuotaReasonCode.PaidTierLimitExceeded,
        };

    private static IReadOnlyList<SpacesQuotaUpgradePlan> GetUpgradePlans(int currentPlanCode) =>
        currentPlanCode switch
        {
            1 => // Free -> Growth, Business
            [
                ToUpgradePlan(5, OfferingCode.SpacesGrowthV1, "SelfService"),
                ToUpgradePlan(6, OfferingCode.SpacesBusinessV1, "SelfService"),
            ],
            5 => // Growth -> Business
            [
                ToUpgradePlan(6, OfferingCode.SpacesBusinessV1, "SelfService"),
            ],
            6 => // Business -> Contact Us
            [
                ToUpgradePlan(7, OfferingCode.SpacesContactUsV1, "ContactUs"),
            ],
            _ => [], // Contact Us or unknown → no upgrade path
        };

    private static SpacesQuotaUpgradePlan ToUpgradePlan(int planCode, OfferingCode offeringCode, string availability)
    {
        var offering = offeringCode.GetOffering();
        return new SpacesQuotaUpgradePlan(
            planCode,
            offering.Name.Replace("Spaces ", string.Empty, StringComparison.Ordinal),
            availability,
            FormatPriceDescription(offering));
    }

    private static string? FormatPriceDescription(Offering offering) =>
        offering.FixedPrice.HasValue
            ? $"${offering.FixedPrice.Value / 100}/month"
            : null;
}
