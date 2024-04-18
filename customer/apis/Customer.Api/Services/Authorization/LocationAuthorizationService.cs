using Api.Shared.Models;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;
using Enterprise.Shared.Exceptions;

namespace Customer.Api.Services.Authorization;

public interface ILocationAuthorizationService
{
    Task<bool> CanAddLocationAsDefaultAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken);
}

public class LocationAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory)
    : ILocationAuthorizationService
{
    public async Task<bool> CanAddLocationAsDefaultAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken)
    {
        if (location.Organization is null)
        {
            var teamMember =
                location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id);

            return teamMember?.MembershipType is LocationMembershipType.Owner or LocationMembershipType.Administrator
                or LocationMembershipType.Member;
        }

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization.Id,
                cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organizationAuthorizationService.IsOrganizationMember(organization, customer);
    }
}
