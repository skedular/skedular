# Data Model: Unified Marketplace Booking History

## Authoritative existing entities

| Entity | History role |
|---|---|
| `MarketplaceBooking` | Financial purchase record; standalone when it has no subscription parent. |
| `Booking` | One-time scheduled occurrence and its soft-delete evidence. |
| `MarketplaceBookingSubscription` | Recurring commercial purchase root, renewal/cancellation state, and soft-delete evidence. |
| `RecurringBooking` | Subscription schedule/instances; detail-only, never a second main entry. |
| `MarketplaceRefund` / events | Independent refund progression and timeline. |

## Durable history projection

`MarketplacePurchaseHistory` is a Booking-owned, rebuildable read projection. Each row references exactly one root source: either a standalone `MarketplaceBooking` or a `MarketplaceBookingSubscription`.

It stores only the fields needed to authorize, filter, order, and explain a retained purchase: source type/reference, organization/product/customer references, purchase/activity and booking-window timestamps, payment/subscription state, renewal flags, deletion/cancellation evidence, and the latest refund reference/status.

The projection is maintained transactionally by explicit application-layer refresh and upsert calls behind Booking repository methods. API services, Temporal activities, processors, and repository updates use those methods; the source aggregates remain authoritative and the projection may be rebuilt from them.

## GraphQL read model

`MarketplacePurchaseHistoryEntry` is a shared Booking-domain read model projected from `MarketplacePurchaseHistory`. The application service returns it; GraphQL maps it at the boundary.

| Field | Rule |
|---|---|
| `Id` | Stable composite source type + source ID; unique across pages. |
| `SourceType` | `StandaloneBooking` or `Subscription`; explicit mapping only. |
| `MarketplaceBookingId` | Required financial source. |
| `BookingId` / `SubscriptionId` | Exactly one root reference is populated. |
| Customer/product/quantity/amount/currency | Sourced from authoritative related records; missing legacy data is unavailable, never fabricated. |
| `PurchaseAt`, `BookingStart`, `BookingEnd`, `LastActivityAt` | Used for filters/order. `LastActivityAt` is the latest purchase, modification, payment, cancellation/deletion, or refund event; equal timestamps sort by source type then source ID. |
| Payment/lifecycle/refund status | Independent dimensions; none is inferred from another. |
| `RenewalState` | Subscription-only: not applicable, renews, stops at period end, ended, or unavailable fallback. |
| Deletion/cancellation data | Timestamp, actor, reason when retained and available. |

## Criteria and invariants

- Search supports organization, source type, lifecycle/payment/refund status, customer, product, renewal, cadence, and purchase/booking/end date ranges.
- Default result is all retained entries, `LastActivityAt` descending; cursor includes primary value, source type, and source ID.
- A subscription root is never emitted again as its root marketplace booking or a generated child booking.
- Generated booking instances are queried in the subscription detail connection with booking-date/payment/lifecycle filters and their own cursor.
- No migration or new retention cutoff is planned; preserve existing repository behavior unless a legal deletion policy applies.
