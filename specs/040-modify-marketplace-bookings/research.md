# Phase 0 Research: Modify Marketplace Bookings

## Dedicated modification command

**Decision**: Add a dedicated `modifyMarketplaceBooking` command rather than expanding `updateMarketplaceBooking`.

**Rationale**: The existing patch enum and input only support participants, notes, and category. Its client is autosave-oriented, which is unsafe for a command that reserves slots, validates an entire replacement resource set, and requires an operator reason. A dedicated command has a clear confirmation boundary and typed results.

**Alternatives considered**: Extending the patch with schedule/resources was rejected because it would conflate autosave and transactional fulfillment changes.

## Atomic schedule and resource replacement

**Decision**: Validate the proposed time window, product/day/cadence entitlement, resource type/tag eligibility, resource count, and payment/start-time conditions before claiming the replacement set in one serializable transaction.

**Rationale**: The current shared marketplace update clears slots and checks availability using the persisted window, while marketplace entity merging does not currently move marketplace `From`/`Until`. It cannot safely implement rescheduling. Existing add flows already demonstrate complete-slot-set claiming and conflict handling.

**Alternatives considered**: Reusing `AdjustRequiredResourcesAsync` was rejected because it is best-effort scheduler repair, not a customer command; releasing old slots first was rejected because a conflict could destroy a valid booking.

## Authorization and eligibility

**Decision**: Derive authority from the persisted booking and product owner. Allow only involved/purchasing customer self-service or product-owner owner/admin action; require confirmed or no-payment-required status, future start, and a required operator reason.

**Rationale**: Request organization/team fields are mutable and current generic update checks are insufficient for acting on behalf of a customer. The clarified product policy excludes pending/terminal payment states and allows change until start regardless of cancellation cutoff.

**Alternatives considered**: Trusting request participants or applying cancellation policy windows was rejected as insecure or contrary to the clarified rules.

## Subscription reconciliation

**Decision**: Successful modification of one subscription occurrence sets `HasRecurringInstanceOverrides`; it must remain in its current cycle and be excluded from daily resource repair and cross-cycle preference propagation.

**Rationale**: Current reconciliation already excludes overrides and counts them as present schedule dates, but its preference resolution can otherwise carry a latest assigned resource into the next cycle. An override is an occurrence exception, not a parent preference change.

**Alternatives considered**: Updating `WeeklySelectedDays` or subscription `RequestedResources`, or signaling a new daily workflow action, was rejected because it changes later occurrences and is not required for immediate persisted UI consistency.

## Audit and customer notification

**Decision**: Persist a modification record and per-recipient notification delivery records, then dispatch through Temporal outbox/activity infrastructure. Expose the persisted history/delivery status in customer UI.

**Rationale**: Browser toast and GraphQL topic refresh are not durable customer notifications. Marketplace refund and failure paths already model durable delivery, retries, templates, and recovery.

**Alternatives considered**: Synchronous email or logs-only notification was rejected because neither provides recovery after a delivery failure.

## UI and documentation coverage

**Decision**: Implement one shared customer detail/hub flow and independent Spaces and Host operator flows. Update Spaces booking/subscription, shared booking/resource/availability/subscription, and Host booking/renter documentation.

**Rationale**: `webapp` owns customer booking details; Spaces and Host intentionally diverge. Host books a whole place, so it gets date/time but no resource selector. Existing documentation states subscriptions are Spaces-only.

**Alternatives considered**: A single shared operator component or Host resource selector was rejected because it would obscure product behavior and create incorrect UI promises.

## Generation and verification

**Decision**: Regenerate GraphQL schema with `scripts/generate-graphql.sh` and Relay artifacts in each consuming application.

**Rationale**: The repository treats GraphQL source, composed schema, and Relay output as generated-contract surfaces that must stay synchronized.

**Alternatives considered**: Hand-editing schema or Relay output was rejected by repository policy.
