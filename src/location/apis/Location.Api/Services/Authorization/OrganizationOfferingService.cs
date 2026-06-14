using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Location.Shared.Services.Cache;

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
    IPricingEntitlementEvaluator pricingEntitlementEvaluator)
    : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

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

        return pricingEntitlementEvaluator.EvaluateActiveUserCount(organization.Offering, currentActiveUserCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId).IsAllowed;
    }
}
