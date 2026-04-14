using Api.Shared.Services;
using Enterprise.Shared.Context;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationSsoAuthorizationService
{
    ValueTask<bool> IsSsoValidAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationSsoAuthorizationService(
    IContext context,
    ICachedOrganizationService cachedOrganizationService,
    ICachedCustomerService cachedCustomerService,
    ILogger<OrganizationSsoAuthorizationService> logger)
    : IOrganizationSsoAuthorizationService
{
    public async ValueTask<bool> IsSsoValidAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken);
        if (organization?.OrganizationSsoSettings is null || !organization.OrganizationSsoSettings.IsActive)
        {
            logger.LogInformation("SSO validation bypassed for organization {OrganizationId} because SSO is inactive", organizationId);
            return true;
        }

        var userSso = context.GetUserSsoContext(organization.Id);
        var customer = await cachedCustomerService.GetByIdAsync(customerId, cancellationToken) ?? throw new CustomerNotFound();
        var allowed = userSso is not null && customer.Identities.Select(item => item.Email).Contains(userSso.Email);

        if (allowed)
        {
            logger.LogInformation("SSO validation granted for customer {CustomerId} in organization {OrganizationId}", customerId, organizationId);
        }
        else
        {
            logger.LogWarning("SSO validation denied for customer {CustomerId} in organization {OrganizationId}", customerId, organizationId);
        }

        return allowed;
    }
}
