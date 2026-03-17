using Api.Shared.Services;
using Api.Shared.Services.Models;
using Location.Shared.Models;
using Location.Shared.Services.Cache;

namespace Location.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewAnalyticsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService,
    ICachedLocationService cachedLocationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanModifyAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanDeleteAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanViewAnalyticsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);
    }

    public async ValueTask<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await cachedLocationService.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanModify = await CanModifyAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanDelete = await CanDeleteAsync(location.OrganizationId, customer.Id, cancellationToken),
            CanViewAnalytics = await CanViewAnalyticsAsync(location.OrganizationId, customer.Id, cancellationToken)
        };
    }
}
