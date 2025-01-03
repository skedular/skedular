using Api.Shared.Services.Models;
using Billing.Shared.Models;
using Billing.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using Organization = Billing.Shared.Database.Entities.Organization;

namespace Billing.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanViewBillingInfo(Organization organization, Customer customer);
    bool CanManageBillingInfo(Organization organization, Customer customer);
    Task<OrganizationLevelPermissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory)
    : IOrganizationAuthorizationService
{
    public bool CanViewBillingInfo(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
            or OrganizationMemberRoleConstants.Member
        };

    public bool CanManageBillingInfo(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
            or OrganizationMemberRoleConstants.Administrator
        };

    public async Task<OrganizationLevelPermissions> GetPermissionsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(
            organizationId,
            cancellationToken);

        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return new OrganizationLevelPermissions
        {
            CanViewBillingInfo = CanViewBillingInfo(organization, customer),
            CanManageBillingInfo = CanManageBillingInfo(organization, customer)
        };
    }
}
