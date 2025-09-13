using Api.Shared.Services;
using Api.Shared.Services.Models;
using Team.Shared.Services.Cache;

namespace Team.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanInvitePeopleAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanInvitePeopleAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        string organizationId,
        string customerId,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }
}
