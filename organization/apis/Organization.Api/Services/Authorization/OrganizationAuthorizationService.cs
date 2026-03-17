using Api.Shared.Services;
using Api.Shared.Services.Models;
using Organization.Shared.Models;
using Organization.Shared.Services.Cache;

namespace Organization.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewMinimumAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);

    ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanViewAnalyticsAsync(Shared.Database.Entities.Organization organization, string customerId, CancellationToken cancellationToken);

    ValueTask<bool> CanManagePaymentMethodAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanViewStripeConnectAccountAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<bool> CanManageStripeConnectAccountAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken);

    ValueTask<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public ValueTask<bool> CanViewMinimumAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        ValueTask.FromResult(organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        });

    public async ValueTask<bool> CanViewAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanModifyAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanDeleteAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanInvitePeopleAsync(Shared.Database.Entities.Organization organization, string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanCancelPeopleExistingInvitationsAsync(
        Shared.Database.Entities.Organization organization,
        string customerId, CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanViewAnalyticsAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanManagePaymentMethodAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanViewStripeConnectAccountAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<bool> CanManageStripeConnectAccountAsync(
        Shared.Database.Entities.Organization organization,
        string customerId,
        CancellationToken cancellationToken) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organization.Id, customerId, cancellationToken);

    public async ValueTask<Permissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await cachedOrganizationService.GetByIdOrCustomDomainAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return new Permissions
        {
            CanView = await CanViewAsync(organization, customer.Id, cancellationToken),
            CanModify = await CanModifyAsync(organization, customer.Id, cancellationToken),
            CanDelete = await CanDeleteAsync(organization, customer.Id, cancellationToken),
            CanInvitePeople = await CanInvitePeopleAsync(organization, customer.Id, cancellationToken),
            CanCancelPeopleExistingInvitations = await CanCancelPeopleExistingInvitationsAsync(organization, customer.Id, cancellationToken),
            CanViewAnalytics = await CanViewAnalyticsAsync(organization, customer.Id, cancellationToken)
        };
    }
}
