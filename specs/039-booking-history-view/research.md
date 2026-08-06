# Research: Unified Marketplace Booking History

## Separate lifecycles, composite read model

**Decision**: Keep subscriptions and standalone bookings as separate aggregates; compose a Booking-owned read model.

**Rationale**: `MarketplaceBookingSubscription` owns renewal, cancellation mode, allocation, and recurring bookings. `MarketplaceBooking` can instead represent a one-time booking. The request is historical discovery, not a lifecycle change.

**Alternatives considered**: Creating subscriptions for every purchase was rejected because it adds workflows. Separate pages were rejected because standalone history remains invisible. A persisted projection is selected because EF Core cannot translate the required cross-table set operation after the C# read-model projection. The projection stays rebuildable and transactional, with explicit application-layer refresh and upsert calls behind repository methods, so it does not become a second source of truth.

## Rename presentation, preserve URLs

**Decision**: Use the new `/purchases` routes and helpers in Spaces/Host; rename visible navigation/page copy to Marketplace purchases.

**Rationale**: The new purchase history is a distinct surface that combines standalone bookings and subscriptions. No legacy route compatibility is required for this feature.

**Alternatives considered**: Retaining `/subscriptions` would misrepresent standalone purchases and is unnecessary because backward compatibility is out of scope. Keeping the old label would misclassify hourly/one-time purchases.

## Server-side combined keyset pagination

**Decision**: Add one unified GraphQL connection with a shared Booking-domain model. It returns subscription roots and standalone marketplace bookings, ordered by newest meaningful activity. Cursor tie-breakers include source type and source ID.

**Rationale**: The existing subscription query excludes deleted subscriptions and has no standalone source. Browser-side merging cannot produce stable counts/cursors while records change. Existing repository patterns use keyset pagination.

**Alternatives considered**: Offset pagination and client merging were rejected. Generated child bookings are not top-level purchase entries.

## Subscription details own generated instances

**Decision**: A subscription is one main result; its details expose a paginated/filterable booking-instance connection. Booking cards/details link to their parent subscription.

**Rationale**: Prevents long-running subscriptions from flooding the main list and supports future instance-management work without building it now.

## Retained inactive history

**Decision**: Include eligible soft-deleted, canceled, expired, failed, and refunded records according to existing repository retention behavior. Do not introduce a new cutoff; retain indefinitely if no configured policy exists.

**Rationale**: `Booking` and `MarketplaceBookingSubscription` retain soft-delete metadata, but the current subscription repository excludes deleted rows. Payment, refund, and lifecycle state remain independent dimensions.

## Authorization, observability, and documentation

**Decision**: Reuse Booking's organization/team authorization; log query scope/counts and legacy-source warnings; update operator documentation.

**Rationale**: This exposes customer and financial operational information. It must remain at least as restricted as current subscription access and document the renamed combined view without claiming that one-time bookings are subscriptions.
