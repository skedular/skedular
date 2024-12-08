using Api.Shared.Models;
using Location.Shared.Database.Entities;
using Customer = Location.Shared.Models.Customer;

namespace Location.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanView(Organization organization, Customer customer);
    bool CanModify(Organization organization, Customer customer);
    bool CanDelete(Organization organization, Customer customer);
    bool CanInvitePeople(Organization organization, Customer customer);
    bool CanCancelPeopleExistingInvitations(Organization organization, Customer customer);
    bool CanViewAnalytics(Organization organization, Customer customer);
}

public class OrganizationAuthorizationService : IOrganizationAuthorizationService
{
    public bool CanView(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator or OrganizationMembershipType.Member;

    public bool CanModify(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator;

    public bool CanDelete(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner;

    public bool CanInvitePeople(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator;

    public bool CanCancelPeopleExistingInvitations(
        Organization organization,
        Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator;

    public bool CanViewAnalytics(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.NewMembershipType is
            OrganizationMembershipType.Owner
            or OrganizationMembershipType.Administrator;
}
