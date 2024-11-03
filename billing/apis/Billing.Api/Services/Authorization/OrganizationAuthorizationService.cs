using Api.Shared.Models;
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
    public bool CanViewBillingInfo(Organization organization, Customer customer)
    {
        var organizationMember =
            organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id);

        return organizationMember?.MembershipType is OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator or OrganizationMembershipType.Member;
    }

    public bool CanManageBillingInfo(Organization organization, Customer customer)
    {
        var organizationMember =
            organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id);

        return organizationMember?.MembershipType is OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator;
    }

    public async Task<OrganizationLevelPermissions> GetPermissionsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetCustomerAsync(cancellationToken);
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
