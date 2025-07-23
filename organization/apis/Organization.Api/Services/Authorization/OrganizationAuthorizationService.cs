using Api.Shared.Services;
using Api.Shared.Services.Models;
using Organization.Shared.Models;
using Organization.Shared.Repositories;

namespace Organization.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanViewMinimum(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanView(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanModify(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanDelete(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanInvitePeople(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanCancelPeopleExistingInvitations(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanViewAnalytics(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanViewMemberPersonalDetails(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanManagePaymentMethod(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanViewStripeConnectAccount(Shared.Database.Entities.Organization organization, Customer customer);
    bool CanManageStripeConnectAccount(Shared.Database.Entities.Organization organization, Customer customer);
    Task<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IRepositoryFactory repositoryFactory,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService)
    : IOrganizationAuthorizationService
{
    public bool CanViewMinimum(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        };

    public bool CanView(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanModify(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanDelete(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanInvitePeople(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanCancelPeopleExistingInvitations(
        Shared.Database.Entities.Organization organization,
        Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewAnalytics(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewMemberPersonalDetails(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanManagePaymentMethod(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewStripeConnectAccount(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanManageStripeConnectAccount(Shared.Database.Entities.Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async Task<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken)
    {
        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await repositoryFactory.OrganizationRepository.GetByIdAsync(organizationId, cancellationToken) ??
                           throw new OrganizationNotFound();

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
