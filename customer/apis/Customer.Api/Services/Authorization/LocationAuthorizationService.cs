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

    Task<bool> CanAddLocationTagAsDefaultAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken);

    public bool IsLocationMember(Location location, Shared.Database.Entities.Customer customer);
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
            return IsLocationMember(location, customer);
        }

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization.Id, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organizationAuthorizationService.IsOrganizationMember(organization, customer);
    }

    public async Task<bool> CanAddLocationTagAsDefaultAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken)
    {
        if (location.Organization is null)
        {
            return IsLocationMember(location, customer);
        }

        var organization =
            await repositoryFactory.OrganizationRepository.GetByIdAsync(location.Organization.Id, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organizationAuthorizationService.IsOrganizationMember(organization, customer);
    }

    public bool IsLocationMember(Location location, Shared.Database.Entities.Customer customer)
    {
        var locationMember =
            location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id);

        return locationMember?.MembershipType is OldLocationMembershipType.Owner or OldLocationMembershipType.Administrator
            or OldLocationMembershipType.Member;
    }
}
