using Api.Shared.Services.Models;
using Marketplace.Shared.Database.Entities;
using Customer = Marketplace.Shared.Models.Customer;

namespace Marketplace.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanModifyProduct(Organization organization, Customer customer);
}

public class OrganizationAuthorizationService(IOrganizationSsoAuthorizationService organizationSsoAuthorizationService)
    : IOrganizationAuthorizationService
{
    public bool CanModifyProduct(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);
}
