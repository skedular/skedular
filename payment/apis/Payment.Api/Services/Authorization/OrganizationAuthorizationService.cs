using Api.Shared.Services.Models;
using Payment.Shared.Models;
using Organization = Payment.Shared.Database.Entities.Organization;

namespace Payment.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanViewPaymentMethod(Organization organization, Customer customer);
    bool CanManagePaymentMethod(Organization organization, Customer customer);
}

public class OrganizationAuthorizationService : IOrganizationAuthorizationService
{
    public bool CanViewPaymentMethod(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
        };

    public bool CanManagePaymentMethod(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
        };
}
