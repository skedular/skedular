using Booking.Shared.Models;
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
    ICachedCustomerService cachedCustomerService,
    ICachedLocationService cachedLocationService)
    : ILocationAuthorizationService
{
    public bool CanViewLocationDetails(Location location, Customer customer) =>
        organizationAuthorizationService.CanViewBookings(location.Organization, customer);

    public bool CanViewBookings(Location location, Customer customer) =>
        organizationAuthorizationService.CanViewBookings(location.Organization, customer);

    public bool CanAddBooking(Location location, Customer customer) =>
        organizationAuthorizationService.CanAddBooking(location.Organization, customer);

    public bool CanUpdateBooking(Location location, Customer customer) =>
        organizationAuthorizationService.CanUpdateBooking(location.Organization, customer);

    public bool CanDeleteBooking(Location location, Customer customer) =>
        organizationAuthorizationService.CanDeleteBooking(location.Organization, customer);

    public bool CanAddBookingOnBehalf(Location location, Customer customer) =>
        organizationAuthorizationService.CanAddBookingOnBehalf(location.Organization, customer);

    public bool CanUpdateBookingOnBehalf(Location location, Customer customer) =>
        organizationAuthorizationService.CanUpdateBookingOnBehalf(location.Organization, customer);

    public bool CanDeleteBookingOnBehalf(Location location, Customer customer) =>
        organizationAuthorizationService.CanDeleteBookingOnBehalf(location.Organization, customer);

    public async Task<LocationPermissions> GetPermissionsAsync(string locationId, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(locationId);

        var (customer, _) = await cachedCustomerService.GetAsync(cancellationToken);
        var location = await cachedLocationService.GetByIdAsync(locationId, cancellationToken);

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
