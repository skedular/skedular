using Enterprise.Shared.Context;
using Location.Shared.Database.Entities;
using Location.Shared.Services.Cache;
using Customer = Location.Shared.Models.Customer;

namespace Location.Api.Services.Authorization;

public interface IOrganizationSsoAuthorizationService
{
    ValueTask<bool> IsSsoValidAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool IsSsoValid(Organization organization, Customer customer);
}

public class OrganizationSsoAuthorizationService(IContext context, ICachedOrganizationService cachedOrganizationService)
    : IOrganizationSsoAuthorizationService
{
    public async ValueTask<bool> IsSsoValidAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && IsSsoValid(organization, customer);
    }

    public bool IsSsoValid(Organization organization, Customer customer)
    {
        if (organization.OrganizationSsoSettings is null || !organization.OrganizationSsoSettings.IsActive)
        {
            return true;
        }

        var userSso = context.GetUserSsoContext(organization.Id);
        return userSso is not null && customer.Identities.Select(item => item.Email).Contains(userSso.Email);
    }
}
