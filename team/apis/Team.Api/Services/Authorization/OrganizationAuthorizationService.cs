using Api.Shared.Services.Models;
using Team.Shared.Database.Entities;
using Team.Shared.Services.Cache;
using Customer = Team.Shared.Models.Customer;

namespace Team.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanView(Organization organization, Customer customer);
    ValueTask<bool> CanModifyAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanModify(Organization organization, Customer customer);
    ValueTask<bool> CanDeleteAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanDelete(Organization organization, Customer customer);
    ValueTask<bool> CanInvitePeopleAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanInvitePeople(Organization organization, Customer customer);
    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanCancelPeopleExistingInvitations(Organization organization, Customer customer);
    ValueTask<bool> CanViewMemberPersonalDetailsAsync(string organizationId, Customer customer, CancellationToken cancellationToken);
    bool CanViewMemberPersonalDetails(Organization organization, Customer customer);
}

public class OrganizationAuthorizationService(
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanView(organization, customer);
    }

    public bool CanView(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanModifyAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanModify(organization, customer);
    }

    public bool CanModify(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanDeleteAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanDelete(organization, customer);
    }

    public bool CanDelete(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanInvitePeopleAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanInvitePeople(organization, customer);
    }

    public bool CanInvitePeople(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(string organizationId, Customer customer,
        CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanCancelPeopleExistingInvitations(organization, customer);
    }

    public bool CanCancelPeopleExistingInvitations(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async ValueTask<bool> CanViewMemberPersonalDetailsAsync(string organizationId, Customer customer, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken);
        return organization is not null && CanViewMemberPersonalDetails(organization, customer);
    }

    public bool CanViewMemberPersonalDetails(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);
}
