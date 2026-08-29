# Data Model: Backend-Owned Marketplace Purchase Lifecycle History

## Event aggregate

`MarketplacePurchaseHistoryEvent` is the immutable append-only record. It belongs to exactly one eligible source: `MarketplaceBookingSubscription` or `EntitlementPurchase`; standalone `MarketplaceBooking` is excluded.

The database stores source and event discriminator values as strings. Domain models and API contracts use enums. Explicit switch-based mappings convert each supported persisted string to its domain/API enum and define the unknown-value policy; direct enum parsing is not permitted. Event-specific values use dedicated typed database columns; no serialized JSON/blob payload is used.

| Field | Rule |
|---|---|
| `Id` | Stable unique event identifier. |
| `SourceType`, `SourceId` | Composite purchase identity; the database string maps to the domain/API source enum Subscription or Entitlement. |
| `EventType` | Closed set defined in `contracts/graphql.md`. |
| `OccurredAt` | Business time at which the transition happened. |
| `RecordedAt` | Server time the event was accepted. |
| `IdempotencyKey` | Required stable key supplied by the authoritative write point; unique per source and logical event. |
| Event-specific columns | Dedicated nullable typed columns for status, quantity, amount/currency, reason, cancellation requested/effective dates, refund ID/status, and renewal boundary as applicable; no serialized payload column. |
| `CorrelationId` | Operational correlation only; never customer secret material. |

Uniqueness is `(SourceType, SourceId, IdempotencyKey)`. Event rows are never mutated or hard-deleted during normal operation.

## Derived current snapshot

`MarketplacePurchaseHistory` remains the list read model but is no longer the lifecycle source of truth. It is rebuilt or updated from the ordered event stream and authoritative purchase metadata. It contains the existing searchable scalar fields and the latest relevant subscription, entitlement, payment, refund, cancellation, and quantity state required by `marketplacePurchases`.

Snapshot derivation is deterministic, ignores duplicate event identities, tolerates late-arriving events by sorting on the event ordering contract, and never synthesizes an event from a field that has no corresponding lifecycle write.

## State derivation rules

- Purchase creation establishes purchase date and initial source/product/customer context.
- Subscription start establishes active start state; renewal updates the current renewal boundary; cancellation scheduled records pending cancellation and its requested/effective date; cancellation completed establishes terminal cancellation.
- Entitlement creation establishes granted quantity; credit consumption decreases available quantity by the event quantity; expiration establishes terminal expiration.
- Payment-state and refund-state events update only their respective dimensions; payment confirmation is not refund completion.
- Missing history yields an empty event collection and does not alter source aggregate data.
