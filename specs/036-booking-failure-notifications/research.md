# Research: Booking Failure Communications

## Decision: Keep the existing payment-release workflows

**Rationale**: One-time card and bank-transfer workflows already release slots, retain the booking, set a terminal payment state, publish the booking, and cancel invoice work. Recurring payment workflows already release bookings for the unpaid recurring cycle while retaining the subscription configuration. Add classification/history/communication at those finalization points rather than replacing the lifecycle.

**Evidence**: `PayBookingViaCard`, `PayBookingViaBankTransfer`, and `BookingIntegrations.ReleaseBookingResourcesAsync`; `PayRecurringBookingViaCard`, `PayRecurringBookingViaBankTransfer`, and `MarketplaceBookingSubscriptionIntegrations.ReleaseRecurringBookingResourcesAsync`.

**Alternatives considered**: Cancel all subscriptions on failed cycle payment (rejected; conflicts with clarified policy) and hold capacity for manual review (rejected; holds unpaid capacity).

## Decision: Add atomic repository-owned availability claims

**Rationale**: Current availability validation is a no-tracking read before booking transaction/association writes. Competing requests can both observe empty slots. A repository operation must claim the complete set in an EF-managed serializable transaction, retry a serialization conflict a bounded number of times, and return a typed availability conflict if it cannot complete. No raw SQL or explicit lock statements are required.

**Evidence**: `MarketplaceBookingService.AddAsync`, `ResourceService`, and `ResourceRepository.GetAvailableResourceIdsAsync`.

**Alternatives considered**: Rechecking availability in the service (rejected: still races), explicit raw-SQL locking (rejected: unnecessary complexity), and redesigning all slot persistence around a new normalized allocation model (rejected: disproportionate to the existing resource-slot model).

## Decision: Model failures separately from payment and booking export state

**Rationale**: Existing `PaymentStatus` identifies payment state but does not preserve a final reason, conflict context, history, notification state, or idempotency key. The marketplace refund aggregate and append-only event history provide the closest booking-owned pattern.

**Evidence**: `MarketplaceRefund`, `MarketplaceRefundEvent`, their repositories/services, and `MarketplaceBooking`.

**Alternatives considered**: Add a single reason field to the booking (rejected: cannot retain occurrence/series context or delivery audit) and reuse invoice-export records (rejected: they are accounting projections, not booking outcome truth).

## Decision: Reuse booking email recipient/template patterns; add durable dispatch

**Rationale**: `MarketplaceRefundNotificationService` already resolves verified customer identities, active organization Owner/Administrator identities, organization contact/routing addresses, and deduplicates recipients. `IEmailService` only delivers email; it has no durable retry/idempotency. Persist recipient/channel delivery rows and dispatch after finalization through a Booking-owned retry-safe workflow/activity initiated by the existing Temporal outbox.

**Evidence**: `MarketplaceRefundNotificationService`, `Enterprise.Shared.Email.EmailService`, `TemporalOutboxService`, and Enterprise outbox workers.

**Alternatives considered**: Synchronous direct email in the transaction/request (rejected: delivery failure delays outcome and workflow retries duplicate sends) and a new cross-product notification service (rejected: none exists and it expands scope).

## Decision: Treat in-app communication as a retained booking failure surface

**Rationale**: The existing notifications page shows invitations and UI toasts are transient; no durable general notification entity exists. Expose retained failure outcomes in customer booking history/details and authorized organization/host booking views, backed by the same delivery/outcome record rather than by toast-only UI.

**Alternatives considered**: Toast-only errors (rejected: not durable) and building a generalized notification center (deferred; unrelated platform scope).

## Decision: Initial series is all-or-nothing; later occurrences are independent

**Rationale**: The feature requires no partially confirmed initial series. Build/claim every initial requested occurrence in one all-or-nothing allocation boundary; on conflict, roll back capacity and record one series-level failure. Later recurring reconciliation retains its daily model but must convert a skipped/resource-less occurrence into a durable occurrence failure and a single immediate communication.

**Alternatives considered**: Preserve current best-effort shells/skips (rejected: ambiguous customer experience) and cancel the entire subscription on one later occurrence conflict (rejected by clarification).
