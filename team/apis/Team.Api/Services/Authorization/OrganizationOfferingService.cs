using Api.Shared.Services.Offering;
using Team.Shared.Database.Entities;
using Customer = Team.Shared.Models.Customer;

namespace Team.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    bool CanCreateTeam(Organization organization);
    bool IsMoreInteractionAllowed(Organization organization, Customer customer);
}

public class OrganizationOfferingService : IOrganizationOfferingService
{
    public bool CanCreateTeam(Organization organization)
    {
        var offering = organization.Offering;
        if (offering is null)
        {
            return false;
        }

        return offering.Code.GetOffering().MaxTeamCount == -1 ||
               organization.Teams.Count < offering.Code.GetOffering().MaxTeamCount;
    }

    public bool IsMoreInteractionAllowed(Organization organization, Customer customer)
    {
        var offering = organization.Offering;
        if (offering is null)
        {
            return false;
        }

        return offering.Code.GetOffering().MaxUserCount == -1 ||
               offering.ActiveCustomerIds.Length <= offering.Code.GetOffering().MaxUserCount ||
               offering.ActiveCustomerIds.Contains(customer.Id);
    }
}
