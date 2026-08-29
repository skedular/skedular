# Feature Specification: Reliable Marketplace Booking Cleanup

**Feature Branch**: `044-marketplace-booking-cleanup`  
**Created**: 2026-08-29  
**Status**: Draft  
**Input**: User description: "Make marketplace booking resource cleanup reliable when payment, invoice, Xero, Stripe, bank-transfer, scheduler, worker, or notification operations fail."

## Clarifications

### Session 2026-08-29

- Q: Should reconciliation automatically enqueue cleanup for every terminal booking with allocated resources, or require operator approval before cleanup? → A: Automatically enqueue cleanup for all eligible terminal bookings; record each attempt for operators.
- Q: Which bookings should reconciliation consider eligible for automatic cleanup? → A: Bookings with an explicit terminal payment/failure state and no confirmed service entitlement, resolving the effective state from linked payment owners such as a subscription when applicable.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Release resources after payment failure (Priority: P1)

As a customer whose marketplace booking payment fails or expires, I need the booking’s resources and slots released reliably so unavailable resources are not held indefinitely.

**Why this priority**: Orphaned allocations directly prevent other customers from booking and create incorrect availability.

**Independent Test**: Cause each one-time and recurring card or bank-transfer payment path to reach a terminal failure, then verify local allocations are released even when external providers are unavailable.

**Acceptance Scenarios**:

1. **Given** a one-time booking has allocated resources and payment reaches a terminal failure, **When** failure cleanup runs, **Then** the booking slots and allocations are released in one local transaction.
2. **Given** a recurring booking has generated instances and payment reaches a terminal failure, **When** cleanup runs, **Then** generated instances are removed or canceled and their resources are released locally.
3. **Given** cleanup is retried or replayed, **When** the booking is already released, **Then** no duplicate allocation changes or errors prevent completion.

### User Story 2 - Keep local cancellation independent of providers (Priority: P1)

As an operator, I need local release to complete even when Xero, Stripe, invoice processing, email, Kafka, or a worker is unavailable, while retaining enough state to recover provider work later.

**Why this priority**: External outages must not leave physical availability incorrect or make local cancellation ambiguous.

**Independent Test**: Fail each external cleanup operation after booking creation and verify local release commits, the provider operation is durably marked for retry, and the workflow can be replayed safely.

**Acceptance Scenarios**:

1. **Given** recurring resources are allocated and accounting cancellation fails, **When** cancellation is requested, **Then** local deletion/release commits before accounting cleanup is attempted and accounting is marked for transition or retry.
2. **Given** Stripe setup returns no product, pricing, customer, or checkout-session result, **When** the payment workflow handles the response, **Then** it records an explicit failure and starts durable cleanup rather than returning silently.
3. **Given** an email, event publication, or notification fails, **When** local release has committed, **Then** the release remains authoritative and notification delivery is retryable.

### User Story 3 - Show truthful recovery state (Priority: P2)

As a customer or operator, I need status to distinguish failure recording, release pending, resources released, and accounting cleanup pending so I do not see a false success state.

**Why this priority**: Accurate status reduces support confusion and prevents operators from assuming resources are available before the local commit.

**Independent Test**: Observe status before, during, and after cleanup under success and failure conditions and verify each displayed state matches the durable local state.

**Acceptance Scenarios**:

1. **Given** failure has been recorded but local release has not committed, **When** status is displayed, **Then** it says release is pending and does not claim resources were released.
2. **Given** local release commits but accounting cleanup fails, **When** status is displayed, **Then** it says resources are released and accounting cleanup is pending.
3. **Given** a mutation succeeds, **When** the UI updates, **Then** it reflects returned server state without a browser reload.

### Edge Cases

- A cleanup activity times out, loses its worker, or exhausts transient retries; the booking remains visibly release-pending and is eligible for reconciliation.
- Database concurrency or a partial prior attempt leaves some generated instances released and others present; a replay converges to the terminal local state.
- Xero emits an error after local release, or accounting identifiers are missing; local state stays authoritative and provider state is durable for follow-up.
- Initial arrears invoice generation fails permanently for one-time or recurring bookings; cleanup still runs.
- Scheduled or manual invoice generation, payment rejection, expiry, and provider webhook failures must use the same local release boundary.
- Notification or event publication occurs before a local transaction commits; no outward status may claim resources are released until commit succeeds.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST trace and cover resource allocation and terminal-failure paths for one-time card, one-time bank-transfer, recurring card, recurring bank-transfer, initial arrears, scheduled/manual invoices, Xero export, Stripe setup/checkout, payment expiry/rejection, and worker/activity retry exhaustion.
- **FR-002**: The system MUST make local resource and slot release the authoritative cancellation action and MUST NOT require an external provider to succeed.
- **FR-003**: For one-time bookings, the system MUST release allocations and booking slots in a local transaction before best-effort accounting cleanup.
- **FR-004**: For recurring bookings, the system MUST delete or cancel generated instances and release their resources locally before accounting cancellation.
- **FR-005**: External accounting and provider cleanup MUST be independently retryable and MUST preserve a durable pending/transition-required outcome after local release.
- **FR-006**: Existing payment and invoice workflows MUST detect terminal failures and delegate to one shared, idempotent Temporal cleanup contract. Local-release activities MUST use at most five delayed/exponential-backoff retry attempts; after exhaustion they MUST remain release-pending and create an immediate reconciliation candidate.
- **FR-007**: Payment workflows MUST convert missing Stripe product, pricing, customer, or checkout-session results into explicit durable failure and cleanup actions.
- **FR-008**: Initial arrears invoice workflows MUST initiate cleanup after permanent invoice, Xero, or local-invoice failure.
- **FR-009**: Failure records and terminal payment state MUST NOT be published as claiming resources are released until the local release transaction commits.
- **FR-010**: The system MUST provide durable reconciliation for failure records created by the handled terminal paths, automatically re-enqueue cleanup, and record each reconciliation attempt for operator visibility. Retry exhaustion MUST create an immediate reconciliation candidate in addition to recurring scans.
- **FR-011**: The system MUST determine reconciliation eligibility from the booking’s effective payment and entitlement state, including linked payment owners such as a subscription, rather than assuming the booking-local payment state is authoritative. Eligible cases are rejected or expired effective payments, or a durable terminal failure record (including invoice-generation, Xero, or Stripe setup failure when no payment record exists), provided no confirmed entitlement exists; pending, confirmed, and no-payment-required cases are excluded.
- **FR-012**: Reconciliation and cancellation MUST prevent canceled recurring subscriptions from recreating generated resources.
- **FR-013**: The UI MUST distinguish failure recorded, release pending, resources released, and accounting cleanup pending, and MUST only show resources released after local commit.
- **FR-014**: The UI MUST update server state through returned mutation/query data and MUST NOT use browser reloads for cache invalidation.
- **FR-015**: The implementation MUST preserve repository persistence boundaries, API parameter ordering, nullability conventions, American English copy, and generated-contract regeneration rules.
- **FR-016**: The feature MUST provide unit tests for every changed workflow/activity. Integration tests are only required for behavior that cannot be proven by unit tests and is essential to merge safety; any persistence assertions MUST use repository methods rather than direct database context access.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs for cleanup and reconciliation start, completion, skip, retry, and terminal failure.
- **LOG-002**: Logs MUST identify meaningful local and accounting state transitions and the decision ordering between local release and provider cleanup.
- **LOG-003**: Logs MUST include correlation context such as booking, workflow, and reconciliation identifiers without payment credentials or sensitive personal data.
- **LOG-004**: Recovery and reconciliation failures MUST produce actionable warning/error logs suitable for operator investigation.

### Key Entities

- **Marketplace booking**: The customer’s one-time or recurring reservation, including payment/failure and cancellation state.
- **Resource allocation and booking slot**: Local availability held by a booking and released by cleanup.
- **Generated recurring booking instance**: A child reservation created for a recurring booking and subject to cancellation or deletion.
- **Cleanup operation**: Durable local-release state, retry identity, and completion information for a booking.
- **Accounting cleanup operation**: Durable follow-up state linking local release to Xero, Stripe, invoice, or other provider cleanup.
- **Reconciliation run**: A durable attempt to find terminal bookings with remaining allocations and enqueue safe cleanup.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In controlled tests, 100% of terminal one-time and recurring payment/invoice failure paths release local allocations even when every external cleanup provider is unavailable.
- **SC-002**: Cleanup attempts that encounter transient failures use bounded retries and reconciliation; remaining cases are visibly release-pending and actionable.
- **SC-003**: No user-facing or operator-facing status claims resources are released before the corresponding local transaction commits.
- **SC-004**: Replaying the same cleanup request at least five times produces one stable local outcome with no duplicate booking instances or allocation changes.
- **SC-005**: A reconciliation run identifies 100% of newly recorded eligible terminal failures with remaining allocations and schedules them for cleanup within one run interval.
- **SC-006**: Operators can distinguish all four cleanup/accounting states from the UI in under 30 seconds during a failure investigation.

## Assumptions

- Existing booking repositories, transaction/outbox mechanisms, Temporal workflows, payment/invoice integrations, and UI status surfaces can be extended rather than replaced.
- Local release is the source of truth for resource availability; accounting and notification systems are projections or follow-up operations.
- Reconciliation runs on a bounded recurring schedule and may also be triggered by operational recovery workflows.
- Provider-specific accounting semantics remain owned by their existing integrations; this feature defines ordering, durable state, and recovery boundaries.
- No new customer-facing booking type is introduced; the feature covers existing one-time and recurring marketplace flows.
- The required repository inspection and implementation plan will identify any missing abstraction or path that cannot be safely reused; such gaps must be recorded before implementation.
