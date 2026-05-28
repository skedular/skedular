using MarketplaceBookingEntity = Booking.Shared.Database.Entities.MarketplaceBooking;
using MarketplaceBookingSubscriptionEntity = Booking.Shared.Database.Entities.MarketplaceBookingSubscription;
using OrganizationEntity = Booking.Shared.Database.Entities.Organization;
using ProductVersionEntity = Booking.Shared.Database.Entities.ProductVersion;

namespace Booking.Domain.IntegrationTests.Fixtures;

public record SubscriptionFilterScenario(
    OrganizationEntity Organization,
    ProductVersionEntity ProductVersion,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) ActivePending,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) ActiveConfirmed,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) CancelledPending,
    (MarketplaceBookingSubscriptionEntity Subscription, MarketplaceBookingEntity MarketplaceBooking) CancelledConfirmed);
