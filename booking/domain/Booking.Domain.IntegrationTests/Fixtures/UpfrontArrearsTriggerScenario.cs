using BookingEntity = Booking.Shared.Database.Entities.Booking;
using CustomerEntity = Booking.Shared.Database.Entities.Customer;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductEntity = Booking.Shared.Database.Entities.Product;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Domain.IntegrationTests.Fixtures;

public record UpfrontArrearsTriggerScenario(
    OrganizationEntity Organization,
    CustomerEntity Customer,
    ProductEntity Product,
    ProductVersionEntity ProductVersion,
    BookingEntity Booking,
    MarketplaceBookingEntity MarketplaceBooking);
