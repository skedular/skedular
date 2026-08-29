# Feature Specification: Backend-Owned Marketplace Purchase Lifecycle History

**Feature Branch**: `045-marketplace-purchase-history`
**Created**: 2026-08-29
**Status**: Implementation complete
**Input**: Redesign `MarketplacePurchaseHistory` from one mutable snapshot per purchase into append-only lifecycle history for subscriptions and credit entitlements.

## User Scenarios & Testing

### User Story 1 - Review subscription lifecycle (Priority: P1)

As a customer or authorized operator, I can open a subscription purchase and see the backend-recorded lifecycle in reverse chronological order, including creation, start, renewal, cancellation scheduling/completion, payment changes, and refund changes.

**Why this priority**: Lifecycle history is the primary audit and support value of the feature.

**Independent Test**: Seed a subscription through multiple lifecycle transitions, open its detail page, refresh it, and verify the same ordered events are returned and rendered.

**Acceptance Scenarios**:

1. **Given** a subscription with creation, start, renewal, cancellation scheduled, and cancellation completed events, **When** its history is requested, **Then** all events appear newest first with their event dates and relevant cancellation dates.
2. **Given** a payment or refund state transition, **When** the transition is persisted, **Then** a corresponding event appears without rewriting prior events.
3. **Given** a cancellation scheduled for a future date, **When** history is viewed before and after that date, **Then** the scheduled event preserves the requested effective date and a completed event appears only when cancellation completes.

### User Story 2 - Review credit entitlement lifecycle (Priority: P1)

As a customer or authorized operator, I can see credit entitlement creation, credit consumption, expiration, payment changes, and refund changes without the UI reconstructing history from aggregate timestamps or quantities.

**Independent Test**: Seed an entitlement with grants, consumption, expiration, and payment/refund transitions and verify the returned event list and displayed details.

**Acceptance Scenarios**:

1. **Given** a credit entitlement purchase, **When** its history is requested, **Then** creation and entitlement-creation events are returned with credit quantities where applicable.
2. **Given** credits are consumed more than once, **When** history is requested, **Then** each accepted consumption event is retained in newest-first order.
3. **Given** an entitlement expires, **When** history is requested, **Then** expiration is shown as an event and the current purchase snapshot reflects expiration.

### User Story 3 - Preserve purchase-list and booking behavior (Priority: P1)

As a user, I can continue using the purchases list, and one-time booking details remain unchanged and do not gain a history tab.

**Independent Test**: Query a mixed purchase list and open one subscription, one entitlement, and one standalone booking; verify list values and detail navigation.

**Acceptance Scenarios**:

1. **Given** a mixed list, **When** it is queried, **Then** each purchase is represented by the current snapshot derived from its latest relevant events, with existing filters, authorization, pagination, and ordering preserved.
2. **Given** a standalone one-time booking, **When** its detail page is opened, **Then** no purchase-history tab or lifecycle-history query is shown, and existing booking content is unchanged.
3. **Given** a user deep-links directly to a subscription or entitlement detail page and refreshes, **When** history is loaded, **Then** the backend returns the same history without client-side event reconstruction.

### Edge Cases

- No history exists for an eligible subscription or entitlement: return an empty history state, not fabricated events; the purchase remains viewable from the list.
- Duplicate delivery of the same lifecycle transition: persist one event using its stable idempotency key.
- Events arrive out of order: order by event occurrence time, then deterministic event ID, and never let a late event erase an earlier event.
- Two events share a timestamp: use a deterministic tie-breaker so refreshes do not reorder them.
- A cancellation has both requested and effective dates: display both when present; completion time is distinct from effective cancellation date.
- A missing optional source detail (such as refund metadata): return the event with available fields and a stable generic label.
- Historical one-time bookings may retain existing purchase-list rows, but no lifecycle event history is created or displayed for them.

## Requirements

### Functional Requirements

- **FR-001**: The system MUST store marketplace lifecycle history as append-only events keyed to one subscription purchase or credit entitlement purchase; no event may be updated or deleted as part of normal lifecycle processing.
- **FR-002**: Supported event types MUST include purchase creation, subscription start, renewal, cancellation scheduled, cancellation completed, entitlement creation, entitlement expiration, credit consumption, payment-state change, and refund-state change.
- **FR-003**: Each event MUST include a stable event ID, purchase source type and ID, event type, occurrence timestamp, recorded timestamp, and a deterministic idempotency key; event-specific data MUST be nullable only when the event type does not use it. Database discriminator values MUST be stored as strings and mapped through explicit switch-based conversions to domain and API enums.
- **FR-004**: Event ordering MUST be newest first by occurrence timestamp, then recorded timestamp, then stable event ID; pagination cursors MUST preserve this ordering.
- **FR-005**: Replaying a lifecycle write with the same purchase, event type, and idempotency key MUST be a no-op and MUST return the existing event identity.
- **FR-006**: The purchase list MUST derive its current snapshot from the latest relevant event state, while retaining the existing purchase-list fields, filters, authorization, pagination, and source classification.
- **FR-007**: Lifecycle write points MUST record events when purchase creation, subscription start/renewal, cancellation scheduling/completion, entitlement creation/expiration/consumption, payment-state changes, and refund-state changes are authoritatively committed.
- **FR-008**: Repository code MUST own event persistence, uniqueness, ordered reads, snapshot derivation, and transaction boundaries; services MUST coordinate lifecycle decisions and call repository operations; transport resolvers MUST not access persistence directly.
- **FR-009**: The GraphQL contract MUST expose eligible purchase history as a connection of backend-provided event details, with stable IDs, an eligible subscription/entitlement source enum, event type/name, event time, cancellation requested/effective dates when applicable, and event-specific values.
- **FR-010**: History MUST be available only for subscriptions and credit entitlements. One-time marketplace bookings MUST not receive lifecycle events, a history tab, or a history query from their detail page.
- **FR-011**: History reads MUST enforce existing organization/customer authorization, including customer self-scope and authorized operator access; unauthorized users MUST receive the existing authorization failure behavior.
- **FR-012**: The frontend MUST render only the event records returned by the backend. It MUST NOT construct lifecycle events from `startedAt`, `cancelledAt`, `nextRenewalAt`, payment fields, refund fields, or other aggregate fields.
- **FR-013**: Missing history MUST render a clear empty state without blocking the purchase detail page; loading, error, refresh, and deep-link states MUST be handled without browser reloads or invented fallback events.
- **FR-014**: Existing one-time booking list/detail behavior MUST remain unchanged, including current booking content and payment/refund presentation.
- **FR-015**: The migration MUST assume no legacy production data: replace the mutable history persistence shape with event and derived-snapshot storage without a backfill requirement; test fixtures may be recreated in the new shape.

### Observability and Logging Requirements

- **LOG-001**: Log event append attempts, successful appends, idempotent replays, and rejected writes with purchase scope, event type, and correlation context.
- **LOG-002**: Log history reads with authorized scope, source type, page size, and result count; do not log payment credentials, tokens, or sensitive customer data.
- **LOG-003**: Log snapshot derivation warnings for missing, unknown, or out-of-order source state and log downstream recovery paths.

## Key Entities

- **Purchase lifecycle event**: Immutable fact about a subscription or credit entitlement purchase, with typed event data and idempotency identity.
- **Purchase current snapshot**: Derived read representation used by the purchases list; it is recomputable from events and never the source of lifecycle truth.
- **Subscription purchase**: Recurring marketplace purchase eligible for lifecycle history.
- **Credit entitlement purchase**: Purchase that grants credits and is eligible for entitlement/consumption history.

## Success Criteria

### Measurable Outcomes

- **SC-001**: 100% of supported lifecycle transitions in acceptance tests produce exactly one corresponding event, including replayed transitions.
- **SC-002**: 100% of subscription and entitlement history responses are deterministically newest-first across repeated reads and refreshes.
- **SC-003**: At least 95% of eligible history pages load within 2 seconds at normal organization scale, excluding network latency outside the service.
- **SC-004**: 100% of tested one-time booking details remain free of a history tab and continue to expose their existing content.
- **SC-005**: No frontend test relies on aggregate timestamps or statuses to create a history event; every rendered event is traceable to a backend response item.

## Assumptions

- Existing authentication and organization authorization rules remain authoritative.
- Event retention is indefinite unless a later policy explicitly changes it.
- Event-specific payloads use existing purchase, payment, refund, subscription, and entitlement identifiers; secrets and credentials are excluded.
- The backend owns lifecycle truth and the frontend is a read-only renderer for this feature.
- No legacy production rows need conversion, so migration focuses on the new schema and clean test data.
