using Api.Shared.Services.Offering;
using Location.Shared.Database.Entities;
using Customer = Location.Shared.Models.Customer;

namespace Location.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    bool CanCreateLocation(Organization organization);
    bool IsMoreInteractionAllowed(Organization organization, Customer customer);
}

public class OrganizationOfferingService : IOrganizationOfferingService
{
    public bool CanCreateLocation(Organization organization)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxLocationCount == -1 ||
                                        organization.Locations.Count < offering.Code.GetOffering().MaxLocationCount);
    }

    public bool IsMoreInteractionAllowed(Organization organization, Customer customer)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                        offering.ActiveCustomerIds.Length <= offering.Code.GetOffering().MaxUserCount ||
                                        offering.ActiveCustomerIds.Contains(customer.Id));
    }
}
