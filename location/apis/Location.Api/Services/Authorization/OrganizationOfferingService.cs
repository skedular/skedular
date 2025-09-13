using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Location.Shared.Services.Cache;

namespace Location.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken);
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(ICachedOrganizationService cachedOrganizationService) : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxLocationCount == -1 ||
                                        organization.Locations.Count < offering.Code.GetOffering().MaxLocationCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                        offering.ActiveCustomerIds.Count <= offering.Code.GetOffering().MaxUserCount ||
                                        offering.ActiveCustomerIds.Contains(customerId));
    }
}
