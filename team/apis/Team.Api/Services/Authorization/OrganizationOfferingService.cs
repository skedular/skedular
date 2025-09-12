using Api.Shared.Services.Offering;
using Team.Shared.Database.Entities;
using Team.Shared.Services.Cache;
using Customer = Team.Shared.Models.Customer;

namespace Team.Api.Services.Authorization;

public interface IOrganizationOfferingService
{
    ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken);
    bool CanCreateTeam(Organization organization);
    ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool IsMoreInteractionAllowed(Organization organization, Customer customer);
}

public class OrganizationOfferingService(ICachedOrganizationService cachedOrganizationService) : IOrganizationOfferingService
{
    public async ValueTask<bool> CanCreateTeamAsync(string organizationId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
        return organization is not null && CanCreateTeam(organization);
    }

    public bool CanCreateTeam(Organization organization)
    {
        var offering = organization.Offering;
        return offering is not null && (offering.Code.GetOffering().MaxTeamCount == -1 ||
                                        organization.Teams.Count < offering.Code.GetOffering().MaxTeamCount);
    }

    public async ValueTask<bool> IsMoreInteractionAllowedAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
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
