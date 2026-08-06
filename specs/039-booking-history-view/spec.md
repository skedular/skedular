# Feature Specification: Unified Marketplace Booking History

**Feature Branch**: `039-booking-history-view`  
**Created**: 2026-08-02  
**Status**: Draft  
**Input**: User description: "Research and specify a unified scheduler view for marketplace bookings and subscriptions, including canceled and deleted booking history"

## Research Summary

Marketplace subscriptions and marketplace bookings currently represent different business lifecycles. A subscription represents recurring entitlement, renewal, cancellation mode, recurring booking instances, and resource-allocation work. A standalone marketplace booking represents one purchased booking, including hourly or short-duration purchases, payment state, invoice information, and refund linkage. Creating a subscription for every standalone booking would add lifecycle and resource-allocation behavior that the purchase does not need.

The current scheduler subscriptions view queries only marketplace subscriptions. Standalone marketplace bookings are therefore absent from that view, and deletion/cancellation can make historical discovery inconsistent. The preferred product direction is a unified, history-oriented marketplace purchase view that presents both source types without changing their domain lifecycles. It must retain the distinction between a recurring subscription and a standalone booking and link recurring child bookings to their parent subscription where applicable.

This specification is intentionally decision-oriented: planning must validate the exact retention and deletion semantics in the existing booking and subscription repositories before implementation.

## Clarifications

### Session 2026-08-02

- Q: How long should inactive marketplace purchase history be retained? → A: Preserve existing repository behavior; retain indefinitely when no configured policy exists.
- Q: Where should the unified history live? → A: Rename and enhance the existing subscriptions page as Marketplace purchases.
- Q: What should the default Marketplace purchases view show? → A: All retained purchases, newest activity first, with pagination; rename and enhance the current subscriptions page while retaining list and grid views.
- Q: How should subscription-generated booking instances appear? → A: Show the parent subscription only; show its generated bookings in a paginated, filterable subscription detail view.
- Q: How should retention be applied when the existing policy is unknown? → A: Preserve the existing repository behavior; do not add a new cutoff, and retain indefinitely when no configured policy exists.
- Q: What defines newest activity? → A: The latest purchase, modification, payment, cancellation/deletion, or refund event; break equal timestamps by source type and source ID.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Review all marketplace purchases (Priority: P1)

As a coworking-space owner or administrator, I want one chronological list containing standalone marketplace bookings and marketplace subscriptions so that I can understand every customer purchase, regardless of booking cadence.

**Why this priority**: The current split view hides hourly and other non-recurring purchases, preventing operators from reconciling activity and answering customer questions.

**Independent Test**: Seed one standalone booking and one subscription for the same organization, open the marketplace purchase history, and verify both appear once with their source type, customer, product, booking window, amount, payment state, and lifecycle state.

**Acceptance Scenarios**:

1. **Given** an organization has standalone, recurring, and subscription-backed marketplace activity, **When** an authorized operator opens the renamed Marketplace purchases page, **Then** its default paginated list contains all retained purchase types in newest-activity-first order.
2. **Given** a subscription has generated recurring booking instances, **When** the operator opens its details, **Then** the parent subscription provides a paginated, filterable list of its relevant booking instances without presenting each instance as an unrelated marketplace purchase.
3. **Given** the same purchase is eligible for more than one existing scheduler surface, **When** it is loaded in the unified view, **Then** it appears once with a stable identity and source classification.
4. **Given** an operator changes between list and grid view, **When** the current filters, sort order, or page are retained, **Then** both presentations show the same purchase set and classification.
5. **Given** a booking card or booking detail belongs to a subscription, **When** an operator follows its subscription link, **Then** the corresponding subscription detail view opens.

### User Story 2 - Investigate canceled or deleted activity (Priority: P1)

As an owner or administrator, I want canceled and deleted purchases to remain discoverable with their historical details so that I can see what happened after a booking stopped being active.

**Why this priority**: Payment, refund, cancellation, and dispute investigation requires historical evidence even when the underlying booking is no longer active.

**Independent Test**: Create a paid standalone booking and a paid subscription, cancel or delete each through supported flows, then filter the history for inactive records and verify each record remains visible with its final state and available history.

**Acceptance Scenarios**:

1. **Given** a standalone booking has been canceled or deleted, **When** an operator selects inactive or all history, **Then** the booking remains listed and is clearly labeled as canceled/deleted with the relevant timestamp and actor when available.
2. **Given** a subscription has been canceled, **When** an operator opens its history entry, **Then** the subscription status, cancellation mode, cancellation time, affected recurring bookings, payment state, and refund state are visible.
3. **Given** an inactive purchase has a refund record or refund events, **When** the operator opens its details, **Then** the refund status and timeline are reachable from the same purchase record.
4. **Given** a purchase is removed by an operational cleanup path such as expiry or payment failure, **When** the operator searches history, **Then** the record is retained with the resulting lifecycle/payment outcome rather than disappearing silently.

### User Story 3 - Search and reconcile lifecycle and money (Priority: P2)

As an operator, I want to filter and sort the unified history by purchase type, customer, product, date, payment state, cancellation state, and refund state so that I can reconcile operational and financial activity efficiently.

**Why this priority**: A combined list is only useful at operational scale if it supports the questions operators ask about payment, cancellation, and refunds.

**Independent Test**: Seed records across each filter dimension, apply filters individually and in combination, and verify that results are complete, correctly classified, stable across pagination, and do not duplicate parent/child records.

**Acceptance Scenarios**:

1. **Given** history contains active, canceled, deleted, refunded, and payment-failed purchases, **When** an operator filters by any supported state, **Then** only matching records are returned and the result count is accurate.
2. **Given** a purchase has multiple lifecycle events, **When** results are sorted by purchase date, cancellation date, or last activity, **Then** ordering is deterministic and records with equal sort values remain stable across pages.
3. **Given** a standalone booking has no subscription, **When** an operator filters for standalone purchases, **Then** it is returned without a fabricated subscription or recurring-resource status.

### User Story 4 - Preserve correct operational behavior (Priority: P2)

As a product owner, I want the history view to observe booking and subscription lifecycles without changing their operational behavior so that hourly bookings do not start unnecessary renewal or resource-allocation workflows.

**Why this priority**: The reporting problem must not introduce recurring workflow cost, duplicate resource allocation, or incorrect cancellation semantics.

**Independent Test**: Compare workflow and resource-allocation activity for equivalent standalone purchases before and after the history feature; verify that adding a record to history does not create recurring allocation work.

**Acceptance Scenarios**:

1. **Given** a standalone hourly booking is created, **When** it is shown in history, **Then** no subscription renewal, recurring allocation, or subscription-only cancellation workflow is created solely because it is listed.
2. **Given** a subscription is canceled, **When** its history is loaded, **Then** the view reflects the authoritative cancellation and refund outcomes without initiating a second cancellation.
3. **Given** a history query encounters an incomplete or legacy relationship, **When** the result is displayed, **Then** the record remains visible with an explicit unknown/unavailable relationship label and the query does not fail for the rest of the page.

### Edge Cases

- A standalone booking and its underlying booking record may have different deletion metadata; history must use the authoritative marketplace purchase outcome and show unavailable details explicitly.
- A subscription may be deleted while its generated recurring bookings remain retained; the parent-child relationship must remain navigable without double-counting.
- A payment can be pending, rejected, expired, confirmed, not required, or absent; payment state must not be inferred from cancellation or refund state.
- A confirmed payment can have no refund, a pending refund, a partial refund, a full refund, or a failed/reconciliation-required refund; these are separate display dimensions.
- A purchase may be canceled immediately or at period end; the list must distinguish scheduled cancellation from completed cancellation.
- A record may change state while the operator is paging; pagination and refresh must not produce duplicate entries or silently lose a newly inactive record.
- Records from different currencies or organizations must not be combined outside the operator's authorized organization scope.
- Repeated retries, webhook updates, or workflow signals must not create duplicate history entries or duplicate lifecycle events.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST rename and enhance the existing organization-level subscriptions page as the authorized Marketplace purchases page, containing both standalone marketplace bookings and marketplace subscriptions.
- **FR-002**: Each result MUST identify its source type as standalone booking or subscription and MUST preserve a stable identifier that can open the authoritative detail view.
- **FR-003**: Each result MUST display, when available, customer, organization, product, purchase/booking window, quantity, amount, currency, payment state, lifecycle state, and last meaningful activity time.
- **FR-004**: The view MUST represent recurring subscription parents and their generated booking instances without double-counting the parent purchase as independent purchases.
- **FR-004a**: A subscription detail view MUST provide deterministic pagination and filtering for its generated booking instances.
- **FR-005**: The view MUST include eligible canceled, deleted, expired, payment-failed, and otherwise inactive records according to an explicit history filter; inactive records MUST NOT disappear solely because they are no longer active.
- **FR-006**: The system MUST preserve deletion and cancellation evidence, including state, timestamp, actor or initiating party when available, reason when available, and links to relevant refund or payment history.
- **FR-007**: The view MUST distinguish immediate cancellation, scheduled period-end cancellation, completed cancellation, deletion, expiry, payment failure, and refund outcomes rather than collapsing them into one inactive label.
- **FR-008**: Operators MUST be able to filter by source type, lifecycle state, payment state, refund state, customer, product, renewal state, booking cadence, and relevant date range, subject to existing authorization.
- **FR-009**: The Marketplace purchases page MUST default to all retained purchases ordered by newest activity first and MUST provide deterministic pagination.
- **FR-009a**: Operators MUST be able to sort results by purchase date, booking start, cancellation/deletion date, and last activity without duplicate or missing entries across pages.
- **FR-010**: A history query MUST return a single unified result per purchase identity and MUST avoid duplicate entries when a subscription has child bookings or multiple related records.
- **FR-011**: The history capability MUST be read-oriented and MUST NOT cause a standalone booking to acquire subscription renewal, recurring allocation, or subscription-only workflow obligations.
- **FR-012**: Existing subscription workflows, standalone booking workflows, cancellation behavior, payment processing, and refund ownership MUST remain authoritative; the history view MUST not reimplement or silently alter those decisions.
- **FR-013**: The system MUST preserve the existing repository retention behavior for inactive marketplace purchases and their available lifecycle events, without introducing a new cutoff; when no configured retention policy exists, records remain retained indefinitely unless a separate legal deletion obligation applies.
- **FR-014**: The experience MUST provide an understandable empty state, partial-data state, loading state, and authorization/error state without implying that no purchase ever existed when history data is unavailable.
- **FR-015**: The feature MUST support both the scheduler spaces operator experience and the scheduler host operator experience where each currently exposes marketplace bookings or subscriptions, using consistent terminology and state meaning.
- **FR-016**: The Marketplace purchases page MUST retain list and grid presentations; both presentations MUST show the same paginated, filtered, and sorted purchase results.
- **FR-017**: Each entry MUST make its marketplace purchase type understandable, including whether it is a subscription, whether it will renew, and whether it is a standalone hourly or other non-subscription booking.
- **FR-018**: Booking cards and booking detail views MUST link to the parent subscription detail when the booking belongs to a subscription.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs for unified history queries, including organization scope, requested filters, source types, and result count without sensitive payment data.
- **LOG-002**: The feature MUST emit structured logs for source reconciliation and deduplication decisions, including stable correlation context and the reason a related record was included, excluded, or marked unavailable.
- **LOG-003**: The feature MUST emit actionable warning/error logs when historical records cannot be resolved, retained lifecycle metadata is inconsistent, or an authorized query cannot complete.
- **LOG-004**: Feature logs MUST include correlation context and MUST avoid customer PII, payment credentials, and unnecessary financial details.

### Key Entities

- **Marketplace purchase history entry**: A read-oriented representation of one operator-visible marketplace purchase, classified as a standalone booking or subscription and linked to its authoritative source.
- **Standalone marketplace booking**: A one-time marketplace purchase that may have payment, invoice, cancellation, deletion, and refund outcomes without a subscription lifecycle.
- **Marketplace booking subscription**: A recurring marketplace purchase aggregate with renewal, cancellation mode, recurring booking instances, and resource-allocation responsibilities.
- **Recurring booking instance**: A booking generated under a subscription; it contributes operational detail and payment/refund context but must not become an unrelated history purchase.
- **Lifecycle history event**: A durable record of creation, payment, cancellation, deletion, expiry, failure, refund, or other meaningful state transition shown to an operator.
- **Refund record**: The durable refund decision and progression associated with a standalone booking or subscription, including its event timeline and downstream status.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In usability testing, operators can locate any known standalone booking or subscription, including an inactive one, in under 60 seconds using the unified history view.
- **SC-002**: For a test dataset containing 100% of standalone and subscription purchases in an organization, the history view returns 100% of eligible purchases exactly once, with zero parent/child duplicates.
- **SC-003**: At least 95% of test scenarios involving cancellation, deletion, payment failure, and refund states show the correct final state and a navigable history timeline without requiring database or support-team access.
- **SC-004**: Adding a standalone purchase to the history view creates zero additional subscription renewal or recurring resource-allocation workflows.
- **SC-005**: At least 90% of operators in acceptance testing correctly distinguish standalone booking, active subscription, scheduled cancellation, completed cancellation, deletion, and refund outcome from the displayed labels and details.
- **SC-006**: Repeated refreshes and pagination over a changing test dataset produce no duplicate history entries and no unexplained disappearance of records that meet the selected history filter.
- **SC-007**: The feature reduces operator reports that a canceled or deleted marketplace purchase cannot be found by at least 50% during the first full reporting period after release, compared with the preceding equivalent period.

## Assumptions

- Subscription creation remains reserved for recurring or otherwise subscription-appropriate purchases; this feature does not convert standalone bookings into subscriptions.
- Existing authorization rules for organization owners, administrators, hosts, and spaces operators remain the source of access decisions.
- Existing booking, subscription, payment, refund, and lifecycle event records are the source of truth; the feature may introduce a read model or query composition but does not duplicate business decisions.
- “Deleted” means operationally removed from the active view but retained according to existing repository behavior; permanent legal/data deletion remains outside this feature unless required by law.
- Customer-facing purchase history is not part of the first operator-history slice unless the planning phase finds that the same authoritative history contract is required to prevent conflicting state meanings.
- Scheduler spaces and scheduler host may use different navigation shells, but they must expose the renamed and enhanced Marketplace purchases page with the same purchase classification and lifecycle semantics.
- The current refund reliability work remains a dependency for displaying complete refund progression; this feature must show unavailable or in-progress refund information rather than inventing a final outcome.

## Scope and Options Considered

### In scope

- Research and document current booking/subscription lifecycle differences and deletion retention behavior.
- Rename and enhance the current subscriptions page as the Marketplace purchases page, retaining its list/grid interaction model while adding all retained purchase types, pagination, and richer classification/filtering.
- Define source classification, parent/child presentation, filters, sorting, inactive-state visibility, and links to payment/refund timelines.
- Validate that the read experience does not add recurring workflow or resource-allocation obligations.

### Out of scope

- Making every marketplace booking a subscription.
- Replacing subscription renewal, booking allocation, payment, cancellation, or refund ownership rules.
- Redesigning the customer checkout or pricing model.
- Rewriting historical financial records or retracting already-generated invoices/refunds.
- Permanently deleting records or changing organization-wide retention/compliance policy without a separate decision.
- Editing the date, time, or other details of generated subscription booking instances; this may be a future feature using the detail view's instance list.

### Decision guidance for planning

Planning MUST compare at least these options against the requirements: (A) create subscriptions for all bookings, (B) keep separate pages and improve each independently, and (C) provide a unified purchase-history read surface while retaining separate domain lifecycles. The default recommendation is C because it addresses discoverability and auditability without imposing recurring workflow semantics on standalone bookings. The plan must confirm whether existing soft-delete metadata and lifecycle/refund events are sufficient for the required retention guarantees and identify any missing source-of-truth records before proposing new persistence.
