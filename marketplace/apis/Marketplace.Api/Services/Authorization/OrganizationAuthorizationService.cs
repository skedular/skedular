using Api.Shared.Services;
using Api.Shared.Services.Models;
using Marketplace.Shared.Services.Cache;

namespace Marketplace.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanModifyProductAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanModifyProductAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);
    }
}
