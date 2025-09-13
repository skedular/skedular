using Api.Shared.Services;
using Enterprise.Shared.Context;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services.Authorization;

public interface IOrganizationSsoAuthorizationService
{
    ValueTask<bool> IsSsoValidAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationSsoAuthorizationService(
    IContext context,
    ICachedOrganizationService cachedOrganizationService,
    ICachedCustomerService cachedCustomerService)
    : IOrganizationSsoAuthorizationService
{
    public async ValueTask<bool> IsSsoValidAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        if (organization?.OrganizationSsoSettings is null || !organization.OrganizationSsoSettings.IsActive)
        {
            return true;
        }

        var userSso = context.GetUserSsoContext(organization.Id);
        var customer = await cachedCustomerService.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();
        return userSso is not null && customer.Identities.Select(item => item.Email).Contains(userSso.Email);
    }
}
