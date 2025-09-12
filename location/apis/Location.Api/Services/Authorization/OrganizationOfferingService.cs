using Api.Shared.Services.Offering;
using Location.Shared.Database.Entities;
using Location.Shared.Services.Cache;
using Customer = Location.Shared.Models.Customer;

namespace Location.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken);
    bool CanCreateLocation(Organization organization);
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool IsMoreInteractionAllowed(Organization organization, Customer customer);
}

public class OrganizationOfferingService(ICachedOrganizationService cachedOrganizationService) : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateLocationAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanCreateLocation(organization);
    }

    public bool CanCreateLocation(Organization organization)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxLocationCount == -1 ||
                                        organization.Locations.Count < offering.Code.GetOffering().MaxLocationCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && IsMoreInteractionAllowed(organization, customer);
    }

    public bool IsMoreInteractionAllowed(Organization organization, Customer customer)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                        offering.ActiveCustomerIds.Count <= offering.Code.GetOffering().MaxUserCount ||
                                        offering.ActiveCustomerIds.Contains(customer.Id));
    }
}
