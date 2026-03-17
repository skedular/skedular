using Api.Shared.Services;
using Api.Shared.Services.Models;
using Customer.Shared.Services.Cache;

namespace Customer.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanAddOrganizationAsDefaultAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddOrganizationTagAsDefaultAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> IsOrganizationMemberAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanAddOrganizationAsDefaultAsync(string organizationId, string customerId, CancellationToken cancellationToken) =>
        await IsOrganizationMemberAsync(organizationId, customerId, cancellationToken);

    public async ValueTask<bool> CanAddOrganizationTagAsDefaultAsync(string organizationId, string customerId, CancellationToken cancellationToken) =>
        await IsOrganizationMemberAsync(organizationId, customerId, cancellationToken);

    public async ValueTask<bool> IsOrganizationMemberAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }
}
