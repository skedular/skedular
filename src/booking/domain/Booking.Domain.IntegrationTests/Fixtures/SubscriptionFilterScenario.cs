using LocationEntity = Booking.Shared.Database.Entities.Location;
using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;
using ResourceEntity = Booking.Shared.Database.Entities.Resource;

namespace Booking.Domain.IntegrationTests.Fixtures;

public record SubscriptionFilterScenario(
    OrganizationEntity Organization,
    ProductVersionEntity ProductVersion,
    LocationEntity Location,
    ResourceEntity Resource,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) ActivePending,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) ActiveConfirmed,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) CancelledPending,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) CancelledConfirmed);
