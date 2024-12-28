using Api.Shared.Services.Models;
using Customer.Shared.Database.Entities;

namespace Customer.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanAddOrganizationAsDefault(Organization organization, Shared.Database.Entities.Customer customer);
    bool CanAddOrganizationTagAsDefault(Organization organization, Shared.Database.Entities.Customer customer);
    bool IsOrganizationMember(Organization organization, Shared.Database.Entities.Customer customer);
}

public class OrganizationAuthorizationService : IOrganizationAuthorizationService
{
    public bool CanAddOrganizationAsDefault(Organization organization, Shared.Database.Entities.Customer customer) =>
        IsOrganizationMember(organization, customer);

    public bool CanAddOrganizationTagAsDefault(Organization organization, Shared.Database.Entities.Customer customer) =>
        IsOrganizationMember(organization, customer);

    public bool IsOrganizationMember(Organization organization, Shared.Database.Entities.Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            MembershipType: OrganizationMembershipTypeConstants.Owner
            or OrganizationMembershipTypeConstants.Administrator
            or OrganizationMembershipTypeConstants.Member
        };
}
