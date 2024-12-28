using Api.Shared.Services.Models;
using Team.Shared.Database.Entities;
using Customer = Team.Shared.Models.Customer;

namespace Team.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanView(Organization organization, Customer customer);
    bool CanModify(Organization organization, Customer customer);
    bool CanDelete(Organization organization, Customer customer);
    bool CanInvitePeople(Organization organization, Customer customer);
    bool CanCancelPeopleExistingInvitations(Organization organization, Customer customer);
}

public class OrganizationAuthorizationService : IOrganizationAuthorizationService
{
    public bool CanView(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
            or OrganizationMembershipTypeConstants.Member
        };

    public bool CanModify(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
        };

    public bool CanDelete(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
        };

    public bool CanInvitePeople(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
        };

    public bool CanCancelPeopleExistingInvitations(
        Organization organization,
        Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
        };
}
