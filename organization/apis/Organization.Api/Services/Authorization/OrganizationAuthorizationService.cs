using Api.Shared.Services.Models;
using Enterprise.Shared.Exceptions;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanView(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanModify(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanDelete(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanInvitePeople(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanViewAnalytics(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanViewMemberPersonalDetails(Shared.Database.Entities.Organization organization, Customer customer);
    Task<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(ICachedCustomerService cachedCustomerService, IRepositoryFactory repositoryFactory)
    : IOrganizationAuthorizationService
{
    public bool CanView(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        };

    public bool CanModify(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        };

    public bool CanDelete(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        };

    public bool CanInvitePeople(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        };

    public bool CanCancelPeopleExistingInvitations(
        Shared.Database.Entities.Organization organization,
        Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        };

    public bool CanViewAnalytics(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        };

    public bool CanViewMemberPersonalDetails(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        };

    public async Task<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken);
        if (organization is null)
        {
            throw new OrganizationNotFound();
        }

        return new Permissions
        {
            CanView = CanView(organization, customer),
            CanModify = CanModify(organization, customer),
            CanDelete = CanDelete(organization, customer),
            CanInvitePeople = CanInvitePeople(organization, customer),
            CanCancelPeopleExistingInvitations = CanCancelPeopleExistingInvitations(organization, customer),
            CanViewAnalytics = CanViewAnalytics(organization, customer)
        };
    }
}
