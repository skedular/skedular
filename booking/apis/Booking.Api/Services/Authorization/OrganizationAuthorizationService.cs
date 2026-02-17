using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services.Cache;

namespace Booking.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    ValueTask<bool> CanViewOrganizationDetailsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanViewBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken);
    ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken);

    ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    ICachedOrganizationService cachedOrganizationService,
    IOrganizationSsoAuthorizationService organizationSsoAuthorizationService)
    : IOrganizationAuthorizationService
{
    public async ValueTask<bool> CanViewOrganizationDetailsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanViewBookingsAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanAddBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanUpdateBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanDeleteBookingAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<bool> CanModifyPaymentMethodAsync(string organizationId, string customerId, CancellationToken cancellationToken)
    {
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(organizationId, null, cancellationToken) ??
                           throw new OrganizationNotFound();

        return organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customerId) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && await organizationSsoAuthorizationService.IsSsoValidAsync(organizationId, customerId, cancellationToken);
    }

    public async ValueTask<OrganizationPermissions> GetPermissionsAsync(
        string? organizationId,
        string? organizationUniqueAlphanumericName,
        CancellationToken cancellationToken)
    {
        var customer = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await cachedOrganizationService.GetByIdOrUniqueAlphanumericNameAsync(
            organizationId,
            organizationUniqueAlphanumericName,
            cancellationToken) ?? throw new OrganizationNotFound();

        return new OrganizationPermissions
        {
            CanViewBookings = await CanViewBookingsAsync(organization.Id, customer.Id, cancellationToken),
            CanAddBooking = await CanAddBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanUpdateBooking = await CanUpdateBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanDeleteBooking = await CanDeleteBookingAsync(organization.Id, customer.Id, cancellationToken),
            CanModifyPaymentMethod = await CanModifyPaymentMethodAsync(organization.Id, customer.Id, cancellationToken)
        };
    }
}
