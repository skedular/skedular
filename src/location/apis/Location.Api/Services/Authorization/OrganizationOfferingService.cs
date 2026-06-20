using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Location.Shared.Logging;
using Location.Shared.Services.Cache;
using Offering = Api.Shared.Services.Models.Offering;

namespace Location.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken);

    ValueTask<EntitlementDecision> GetLocationCreationEntitlementAsync(
        string organizationId,
        int currentActiveUserCount,
        CancellationToken cancellationToken);

    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(
    ICachedOrganizationService cachedOrganizationService,
    ILogger<OrganizationOfferingService> logger,
    IPricingEntitlementEvaluator pricingEntitlementEvaluator,
    ISpacesAccessEvaluator spacesAccessEvaluator,
    TimeProvider timeProvider)
    : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var spacesDecision = EvaluateSpacesAccess(organizationId, organization.Offering);
        if (!spacesDecision.Allowed)
        {
            logger.LogWarning(
                "Spaces create-location access denied for organization {OrganizationId}: {Status} {ReasonCode}",
                organizationId,
                spacesDecision.Status,
                spacesDecision.ReasonCode);
            return false;
        }

        var decision = pricingEntitlementEvaluator.EvaluateLocationCreation(organization.Offering, organization.Locations.Count);
        if (decision.IsAllowed)
        {
            logger.LogInformation("Create-location offering check granted for organization {OrganizationId}", organizationId);
        }
        else
        {
            logger.LogWarning(
                "Create-location offering check denied for organization {OrganizationId}: {ReasonCode}",
                organizationId,
                decision.ReasonCode);
        }

        return decision.IsAllowed;
    }

    public async ValueTask<EntitlementDecision> GetLocationCreationEntitlementAsync(
        string organizationId,
        int currentActiveUserCount,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var spacesDecision = EvaluateSpacesAccess(organizationId, organization.Offering);
        if (!spacesDecision.Allowed)
        {
            return new EntitlementDecision(false, EntitlementReasonCode.OfferingNotEffective);
        }

        return pricingEntitlementEvaluator.EvaluateActiveUserCount(organization.Offering, currentActiveUserCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (!EvaluateSpacesAccess(organizationId, organization.Offering).Allowed)
        {
            return false;
        }

        return pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId).IsAllowed;
    }

    private SpacesAccessDecision EvaluateSpacesAccess(string organizationId, Offering? offering)
    {
        var decision = spacesAccessEvaluator.Evaluate(
            timeProvider.GetUtcNow(),
            offering,
            SpacesAccessAction.CreateOrModify);
        logger.Log(
            decision.Allowed ? LogLevel.Information : LogLevel.Warning,
            decision.Allowed ? SpacesTrialLogEvents.AccessDecisionAllowed : SpacesTrialLogEvents.AccessDecisionDenied,
            "Spaces location access decision for organization {OrganizationId}. Status: {Status}, ReasonCode: {ReasonCode}, Allowed: {Allowed}",
            organizationId,
            decision.Status,
            decision.ReasonCode,
            decision.Allowed);
        return decision;
    }
}
