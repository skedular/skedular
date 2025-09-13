using Api.Shared.Services;
using Customer.Shared.Database.Entities;
using Customer.Shared.Services.Cache;

namespace Customer.Api.Services.Authorization;

public interface ILocationAuthorizationService
{
    ValueTask<bool> CanAddLocationAsPreferredAsync(Location location, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddLocationTagAsPreferredAsync(Location location, string customerId, CancellationToken cancellationToken);
}

public class LocationAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : ILocationAuthorizationService
{
    public async ValueTask<bool> CanAddLocationAsPreferredAsync(Location location, string customerId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(location.Organization.Id, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await organizationAuthorizationService.IsOrganizationMemberAsync(organization.Id, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanAddLocationTagAsPreferredAsync(Location location, string customerId, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(location.Organization);

        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(location.Organization.Id, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return await organizationAuthorizationService.IsOrganizationMemberAsync(organization.Id, customerId, cancellationToken);
    }
}
