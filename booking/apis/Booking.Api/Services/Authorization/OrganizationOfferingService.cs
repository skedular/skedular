using Api.Shared.Services.Offering;
using Booking.Shared.Database.Entities;
using Customer = Booking.Shared.Models.Customer;

namespace Booking.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    bool IsMoreInteractionAllowed(Organization organization, Customer customer);
}

public class OrganizationOfferingService : IOrganizationOfferingService
{
    public bool IsMoreInteractionAllowed(Organization organization, Customer customer)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxUserCount == -1 ||
                                        offering.ActiveCustomerIds.Length <= offering.Code.GetOffering().MaxUserCount ||
                                        offering.ActiveCustomerIds.Contains(customer.Id));
    }
}
