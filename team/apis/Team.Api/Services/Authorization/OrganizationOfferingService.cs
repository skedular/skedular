using Api.Shared.Services;
using Api.Shared.Services.Models;
using Api.Shared.Services.Offering;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken);
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(ICachedOrganizationService cachedOrganizationService, ILogger<OrganizationOfferingService> logger)
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

        var offering = organization.Offering;
        var allowed = offering is not null && (offering.Code.GetOffering().MaxTeamCount == -1 ||
                                               organization.Teams.Count < offering.Code.GetOffering().MaxTeamCount);

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

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();
        var offering = organization.Offering;
        var allowed = offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                               offering.ActiveCustomerIds.Count <= offering.Code.GetOffering().MaxUserCount ||
                                               offering.ActiveCustomerIds.Contains(customerId));

        if (allowed)
        {
            logger.LogInformation(
                "Interaction allowance granted for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }
        else
        {
            logger.LogWarning(
                "Interaction allowance denied for customer {CustomerId} in organization {OrganizationId}",
                customerId,
                organizationId);
        }

        return allowed;
    }
}
