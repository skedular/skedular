using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken);
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(ICachedOrganizationService cachedOrganizationService) : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxTeamCount == -1 ||
                                        organization.Teams.Count < offering.Code.GetOffering().MaxTeamCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                        offering.ActiveCustomerIds.Count <= offering.Code.GetOffering().MaxUserCount ||
                                        offering.ActiveCustomerIds.Contains(customerId));
    }
}
