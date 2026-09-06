# Marketplace Renewal Event Contract

## Guarantees

- Events are append-only and are the history UI’s source of truth.
- Every transition contains a source identity, event ID, occurrence time, and idempotency key.
- Webhook-linked events retain safe provider event and PaymentIntent identifiers.
- Workflow retry, webhook replay, fallback completion, and manual recovery cannot create duplicate payment-success or materialization events.
- Out-of-order events are retained for audit but only advance current state through valid transitions.

## Consumer rule

Customer/admin history must render persisted events and must not infer historical events from mutable aggregate payment status, timestamps, entitlement state, or reservation state.
