using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken);

    ValueTask<EntitlementDecision> GetTeamCreationEntitlementAsync(
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
    public async ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        if (organization.Type != OrganizationTypeConstants.Private)
        {
            logger.LogWarning(
                "Create-team offering check denied for organization {OrganizationId} because organization type {OrganizationType} is not private",
                organizationId,
                organization.Type);
            throw new TeamNotAllowedForOrganizationType();
        }

        var decision = pricingEntitlementEvaluator.EvaluateTeamCreation(organization.Offering, organization.Teams.Count);
        var allowed = decision.IsAllowed;

        if (allowed)
        {
            logger.LogInformation("Create-team offering check granted for organization {OrganizationId}", organizationId);
        }
        else
        {
            logger.LogWarning("Create-team offering check denied for organization {OrganizationId}", organizationId);
        }

        return allowed;
    }

    public async ValueTask<EntitlementDecision> GetTeamCreationEntitlementAsync(
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
