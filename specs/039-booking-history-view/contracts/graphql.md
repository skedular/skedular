# GraphQL Contract: Marketplace Purchases

Add the following behavior to the source schema at `src/booking/apis/Booking.Api/schema.graphqls`; generated schemas and Relay artifacts remain outputs.

```graphql
marketplacePurchases(
  after: String
  first: Int
  before: String
  last: Int
  where: MarketplacePurchaseHistoryWhereInput!
  orderBy: [MarketplacePurchaseHistoryOrderInput!]
): ConnectionOfMarketplacePurchaseHistoryEdge!
```

The operator page uses `first: 50`, organization scope, and default `LAST_ACTIVITY_AT DESCENDING`. `lastActivityAt` is the latest purchase, modification, payment, cancellation/deletion, or refund event; equal timestamps are ordered by source type then source ID. Omitted filters mean all retained purchases.

```graphql
type MarketplacePurchaseHistoryEntry {
  id: ID!
  sourceType: MarketplacePurchaseSourceTypeDetails!
  marketplaceBooking: MarketplaceBookingDetails!
  booking: BookingDetails
  subscription: MarketplaceBookingSubscriptionDetails
  customer: CustomerDetails
  productVersion: ProductVersionDetails!
  purchaseAt: DateTime!
  bookingStart: DateTime
  bookingEnd: DateTime
  lastActivityAt: DateTime!
  lifecycleStatus: MarketplacePurchaseLifecycleStatusDetails!
  renewalState: MarketplacePurchaseRenewalStateDetails!
  deletedAt: DateTime
  cancelledAt: DateTime
  cancellationReason: String
  refund: MarketplaceRefundDetails
}
```

Exactly one root source (`booking` or `subscription`) is populated. `renewalState` is `NOT_APPLICABLE` for standalone purchases. The where input includes source/lifecycle/payment/refund/customer/product/renewal/cadence/date filters; order supports activity, purchase, booking start, and end state.

Add a `bookingInstances` cursor connection to `MarketplaceBookingSubscriptionDetails` for the detail page. It supports booking-date, lifecycle, and payment filters. Keep existing unbounded `recurringBookings` for compatibility but do not use it in the new detail UI.

Expose a nullable parent-subscription reference on bookings whose marketplace purchase belongs to a subscription. It is null for standalone bookings. Add queryable choice/details types for any new selectable state.

After source changes, run `scripts/generate-graphql.sh` and regenerate both operator apps' Relay artifacts.
