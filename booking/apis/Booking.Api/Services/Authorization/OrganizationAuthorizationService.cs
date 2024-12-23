using Api.Shared.Models;
using Booking.Shared.Models;
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
    bool CanAddBookingOnBehalf(Organization organization, Customer customer);
    bool CanUpdateBookingOnBehalf(Organization organization, Customer customer);
    bool CanDeleteBookingOnBehalf(Organization organization, Customer customer);
    Task<OrganizationPermissions> GetPermissionsAsync(string organizationId, CancellationToken cancellationToken);
}

public class OrganizationAuthorizationService(
    ICachedCustomerService cachedCustomerService,
    ICachedOrganizationService cachedOrganizationService)
    : IOrganizationAuthorizationService
{
    public bool CanViewOrganizationDetails(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
            or OrganizationMembershipType.Member
        };

    public bool CanViewBookings(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
            or OrganizationMembershipType.Member
        };

    public bool CanAddBooking(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
            or OrganizationMembershipType.Member
        };

    public bool CanUpdateBooking(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
            or OrganizationMembershipType.Member
        };

    public bool CanDeleteBooking(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
            or OrganizationMembershipType.Member
        };

    public bool CanAddBookingOnBehalf(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
        };

    public bool CanUpdateBookingOnBehalf(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
        };

    public bool CanDeleteBookingOnBehalf(Organization organization, Customer customer) =>
        organization.OrganizationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id) is
        {
            Status: OrganizationMemberStatus.Active,
            MembershipType: OrganizationMembershipType.Owner or OrganizationMembershipType.Administrator
        };

    public async Task<OrganizationPermissions> GetPermissionsAsync(
        string organizationId,
        CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(organizationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var organization = await cachedOrganizationService.GetByIdAsync(organizationId, cancellationToken);

        return new OrganizationPermissions
        {
            CanViewBookings = CanViewBookings(organization, customer),
            CanAddBooking = CanAddBooking(organization, customer),
            CanUpdateBooking = CanUpdateBooking(organization, customer),
            CanDeleteBooking = CanDeleteBooking(organization, customer),
            CanAddBookingOnBehalf = CanAddBookingOnBehalf(organization, customer),
            CanUpdateBookingOnBehalf = CanUpdateBookingOnBehalf(organization, customer),
            CanDeleteBookingOnBehalf = CanDeleteBookingOnBehalf(organization, customer)
        };
    }
}
