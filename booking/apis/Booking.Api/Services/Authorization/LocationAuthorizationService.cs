using Api.Shared.Models;
using Booking.Shared.Models;
using Booking.Shared.Repositories;
using Enterprise.Shared.Exceptions;
using Customer = Booking.Shared.Models.Customer;
using Location = Booking.Shared.Database.Entities.Location;

namespace Booking.Api.Services.Authorization;

public interface ILocationAuthorizationService
{
    bool CanViewLocationDetails(Location location, Customer customer);
    bool CanViewBookings(Location location, Customer customer);
    bool CanAddBooking(Location location, Customer customer);
    bool CanUpdateBooking(Location location, Customer customer);
    bool CanDeleteBooking(Location location, Customer customer);
    bool CanAddBookingOnBehalf(Location location, Customer customer);
    bool CanUpdateBookingOnBehalf(Location location, Customer customer);
    bool CanDeleteBookingOnBehalf(Location location, Customer customer);
    Task<LocationPermissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken);
}

public class LocationAuthorizationService(
    IOrganizationAuthorizationService organizationAuthorizationService,
    ICustomerService customerService,
    IRepositoryFactory repositoryFactory)
    : ILocationAuthorizationService
{
    public bool CanViewLocationDetails(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanViewBookings(location.Organization, customer);
    }

    public bool CanViewBookings(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanViewBookings(location.Organization, customer);
    }

    public bool CanAddBooking(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanAddBooking(location.Organization, customer);
    }

    public bool CanUpdateBooking(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanUpdateBooking(location.Organization, customer);
    }

    public bool CanDeleteBooking(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner or LocationMembershipType.Administrator or LocationMembershipType.Member;
        }

        return organizationAuthorizationService.CanDeleteBooking(location.Organization, customer);
    }

    public bool CanAddBookingOnBehalf(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanAddBookingOnBehalf(location.Organization, customer);
    }

    public bool CanUpdateBookingOnBehalf(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanUpdateBookingOnBehalf(location.Organization, customer);
    }

    public bool CanDeleteBookingOnBehalf(Location location, Customer customer)
    {
        if (location.Organization is null)
        {
            return location.LocationMembers.SingleOrDefault(item => item.Customer.Id == customer.Id)?.MembershipType is
                LocationMembershipType.Owner
                or LocationMembershipType.Administrator;
        }

        return organizationAuthorizationService.CanDeleteBookingOnBehalf(location.Organization, customer);
    }

    public async Task<LocationPermissions> GetPermissionsAsync(
        string locationId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(locationId))
        {
            return new LocationPermissions
            {
                CanViewBookings = false,
                CanAddBooking = false,
                CanUpdateBooking = false,
                CanDeleteBooking = false,
                CanAddBookingOnBehalf = false,
                CanUpdateBookingOnBehalf = false,
                CanDeleteBookingOnBehalf = false
            };
        }

        var (customer, _) = await customerService.GetCustomerAsync(cancellationToken);
        var location = await repositoryFactory.LocationRepository.GetByIdAsync(
            locationId,
            cancellationToken);
        if (location is null)
        {
            throw new OrganizationNotFound();
        }

        return new LocationPermissions
        {
            CanViewBookings = CanViewBookings(location, customer),
            CanAddBooking = CanAddBooking(location, customer),
            CanUpdateBooking = CanUpdateBooking(location, customer),
            CanDeleteBooking = CanDeleteBooking(location, customer),
            CanAddBookingOnBehalf = CanAddBookingOnBehalf(location, customer),
            CanUpdateBookingOnBehalf = CanUpdateBookingOnBehalf(location, customer),
            CanDeleteBookingOnBehalf = CanDeleteBookingOnBehalf(location, customer)
        };
    }
}
