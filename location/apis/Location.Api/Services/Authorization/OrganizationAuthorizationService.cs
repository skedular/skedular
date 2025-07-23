using Api.Shared.Services;
using Api.Shared.Services.Models;
using Location.Shared.Models;
using Location.Shared.Repositories;
using Customer = Location.Shared.Models.Customer;
using Organization = Location.Shared.Database.Entities.Organization;

namespace Location.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanView(Organization organization, Customer customer);
    bool CanModify(Organization organization, Customer customer);
    bool CanDelete(Organization organization, Customer customer);
    bool CanViewAnalytics(Organization organization, Customer customer);
    Task<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService)
    : IOrganizationAuthorizationService
{
    public bool CanView(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
            or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanModify(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanDelete(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewAnalytics(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async Task<Permissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(locationId, cancellationToken) ?? throw new LocationNotFound();

        return new Permissions
        {
            CanView = CanView(location.Organization, customer),
            CanModify = CanModify(location.Organization, customer),
            CanDelete = CanDelete(location.Organization, customer),
            CanViewAnalytics = CanViewAnalytics(location.Organization, customer)
        };
    }
}
