using Api.Shared.Services;
using Api.Shared.Services.Models;
using Booking.Shared.Models;
using Booking.Shared.Services.Cache;
using Customer = Booking.Shared.Models.Customer;
using Organization = Booking.Shared.Database.Entities.Organization;

namespace Booking.Api.Services.Authorization;

public interface IOrganizationAuthorizationService
{
    bool CanViewOrganizationDetails(Organization organization, Customer customer);
    bool CanViewBookings(Organization organization, Customer customer);
    bool CanAddBooking(Organization organization, Customer customer);
    bool CanUpdateBooking(Organization organization, Customer customer);
    bool CanDeleteBooking(Organization organization, Customer customer);
    bool CanViewMemberPersonalDetails(Organization organization, Customer customer);
    bool CanModifyPaymentMethod(Organization organization, Customer customer);

    Task<OrganizationPermissions> GetPermissionsAsync(
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
    public bool CanViewOrganizationDetails(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewBookings(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanAddBooking(Organization organization, Customer customer) =>
        organization.Type == OrganizationTypeConstants.Marketplace ||
        (organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer));

    public bool CanUpdateBooking(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanDeleteBooking(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator or OrganizationMemberRoleConstants.Member
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanViewMemberPersonalDetails(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public bool CanModifyPaymentMethod(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatusConstants.Active,
            Role: OrganizationMemberRoleConstants.Owner or OrganizationMemberRoleConstants.Administrator
        } && organizationSsoAuthorizationService.IsSsoValid(organization, customer);

    public async Task<OrganizationPermissions> GetPermissionsAsync(
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
            CanViewBookings = CanViewBookings(organization, customer),
            CanAddBooking = CanAddBooking(organization, customer),
            CanUpdateBooking = CanUpdateBooking(organization, customer),
            CanDeleteBooking = CanDeleteBooking(organization, customer),
            CanModifyPaymentMethod = CanModifyPaymentMethod(organization, customer)
        };
    }
}
