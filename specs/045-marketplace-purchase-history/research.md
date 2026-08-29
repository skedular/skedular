# Research: Backend-Owned Marketplace Purchase Lifecycle History

## Decision: Keep the current purchase snapshot as a derived read model

**Rationale**: The existing `MarketplacePurchaseHistory` repository and service already support the purchases list, scalar filtering, organization authorization, and pagination. Retaining that shape as a recomputable snapshot limits list regressions while moving lifecycle truth into immutable events.

**Alternatives considered**: Replacing the list query with direct event reduction on every request would make existing filters and performance harder to preserve. Keeping the current mutable row as the only source would lose audit history and cannot represent repeated renewals, consumption, or state transitions.

## Decision: Use append-only event rows in the existing MarketplacePurchaseHistory table

**Rationale**: The requirement explicitly excludes one-time booking history. A source-type/source-ID identity plus idempotency key supports repeated renewals and consumption while preventing duplicate processor/workflow deliveries.

**Alternatives considered**: A separate event table would duplicate storage and migration ownership and was explicitly rejected. Adding events to one-time bookings would change their existing UX and scope.

## Decision: Idempotency is source + deterministic transition key

**Rationale**: Lifecycle transitions can be retried or delivered out of order by Temporal, processors, payment reconciliation, and refund workflows. A unique `(SourceType, SourceId, IdempotencyKey)` constraint makes replay safe without treating equal statuses as new events.

**Alternatives considered**: Timestamp-based deduplication is unsafe for rapid legitimate transitions; status-based deduplication loses repeated renewals and credit consumption.

## Decision: Order by occurrence time, recorded time, and event ID

**Rationale**: Business occurrence time gives users meaningful newest-first history, recorded time provides a deterministic second key for late arrivals, and event ID gives a stable final tie-breaker for cursor pagination and refreshes.

**Alternatives considered**: Database insertion order is not a business contract and can change under retries or concurrent writes.

## Decision: Append and snapshot update share the authoritative local transaction

**Rationale**: The event must describe a committed local lifecycle transition, and the purchases list must converge immediately to the same state. Provider calls and speculative UI state remain outside this authority boundary.

**Alternatives considered**: A later best-effort refresh can leave a transition invisible or produce a snapshot that disagrees with the event stream.

## Decision: Expose a connection of typed backend events through GraphQL

**Rationale**: The existing API is GraphQL/Relay-based and the frontend must render backend-provided history only. A connection supports deep links, refreshes, stable cursors, empty states, and event-specific fields without aggregate-field reconstruction.

**Alternatives considered**: Returning a preformatted string list would prevent consistent localization and typed event behavior; returning aggregate timestamps would recreate the current ambiguity.

## Decision: Preserve existing authorization and omit one-time booking history entirely

**Rationale**: Purchase history currently uses organization scope plus customer self-scope/operator authorization. Reusing it avoids a privacy regression. An absent history field/tab for one-time bookings is a stronger boundary than returning an empty event stream that callers might later populate.

**Alternatives considered**: A new authorization policy would duplicate existing rules and increase security review scope; a shared tab would violate the explicit one-time booking requirement.

## Decision: No legacy backfill

**Rationale**: There is no legacy production data. The migration can create the new event structure and preserve clean test fixtures without inventing historical events from aggregate fields.

**Alternatives considered**: A synthetic backfill would violate the backend-only event rule and create events that never occurred at an authoritative write point.

## Decision: Store event-specific values in dedicated typed columns

**Rationale**: Dedicated columns preserve queryability, schema-level type safety, and straightforward GraphQL mapping for cancellation dates, quantities, payment/refund states, and amounts. They also keep the event contract explicit and avoid hiding lifecycle data in an opaque serialized payload.

**Alternatives considered**: A JSON/blob payload would reduce initial column count but weaken relational validation, make event-specific queries harder, and permit inconsistent shapes across event types.
