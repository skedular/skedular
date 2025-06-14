using Api.Shared.Services;
using Customer.Shared.Database.Entities;
using Customer.Shared.Repositories;

namespace Customer.Api.Services.Authorization;

public interface ILocationAuthorizationService
{
    Task<bool> CanAddLocationAsPreferredAsync(Location location, Shared.Database.Entities.Customer customer, CancellationToken cancellationToken);
    Task<bool> CanAddLocationTagAsPreferredAsync(Location location, Shared.Database.Entities.Customer customer, CancellationToken cancellationToken);
}

public class LocationAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    IRepositoryFactory repositoryFactory)
    : ILocationAuthorizationService
{
    public async Task<bool> CanAddLocationAsPreferredAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
            location.Organization.Id,
            false,
            false,
            cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organizationAuthorizationService.IsOrganizationMember(organization, customer);
    }

    public async Task<bool> CanAddLocationTagAsPreferredAsync(
        Location location,
        Shared.Database.Entities.Customer customer,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
            location.Organization.Id,
            false,
            false,
            cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return organizationAuthorizationService.IsOrganizationMember(organization, customer);
    }
}
