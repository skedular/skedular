using Api.Shared.Services;
using Api.Shared.Services.Models;
using Location.Shared.Models;
using Location.Shared.Services.Cache;
using Customer = Location.Shared.Models.Customer;
using Organization = Location.Shared.Database.Entities.Organization;

namespace Location.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanView(Organization organization, Customer customer);
    ValueTask<bool> CanModifyAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanModify(Organization organization, Customer customer);
    ValueTask<bool> CanDeleteAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanDelete(Organization organization, Customer customer);
    ValueTask<bool> CanViewAnalyticsAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanViewAnalytics(Organization organization, Customer customer);
    ValueTask<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService,
    ICachedLocationService cachedLocationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
        return organization is not null && CanView(organization, customer);
    }

    public bool CanView(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
            or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanModifyAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
        return organization is not null && CanModify(organization, customer);
    }

    public bool CanModify(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanDeleteAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
        return organization is not null && CanDelete(organization, customer);
    }

    public bool CanDelete(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanViewAnalyticsAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);
        return organization is not null && CanViewAnalytics(organization, customer);
    }

    public bool CanViewAnalytics(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await cachedLocationService.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(location.OrganizationId, customer, cancellationToken),
            CanModify = await CanModifyAsync(location.OrganizationId, customer, cancellationToken),
            CanDelete = await CanDeleteAsync(location.OrganizationId, customer, cancellationToken),
            CanViewAnalytics = await CanViewAnalyticsAsync(location.OrganizationId, customer, cancellationToken)
        };
    }
}
