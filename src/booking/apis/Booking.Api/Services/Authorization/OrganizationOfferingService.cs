using Api.Shared.Services;
using Api.Shared.Services.Offering;
using Booking.Shared.Services.Cache;

namespace Booking.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationOfferingService(
    ICachedOrganizationService cachedOrganizationService,
    IPricingEntitlementEvaluator pricingEntitlementEvaluator)
    : IOrganizationOfferingService
{
    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return pricingEntitlementEvaluator.EvaluateActiveUser(organization.Offering, customerId).IsAllowed;
    }
}
