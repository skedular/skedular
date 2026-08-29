# Cleanup Reliability Contract

## Local cleanup

Existing payment and invoice workflows submit this shared Temporal cleanup contract with the customer-facing boundary, reason, effective payment owner or durable failure source, and idempotency key. The service rechecks eligibility, then in one local transaction cancels/deletes generated bookings as required, releases slots/allocations, and persists `Released`. Local release uses at most five delayed/exponential-backoff retries; exhaustion creates an immediate reconciliation candidate. Post-commit status/event work uses the existing outbox. Accounting/provider cleanup is independent and retried through its own durable state.

Repeated requests with the same identity return the durable outcome and do not re-release resources.

## Reconciliation

The scheduled service finds every eligible terminal failure/payment owner with remaining allocations, records an attempt/lease, and automatically enqueues cleanup. It includes subscription-linked bookings and durable terminal invoice/Xero/Stripe failures where no payment record exists; it skips pending, confirmed, no-payment-required, and confirmed-entitlement cases, and tolerates concurrent cleanup.

## Status

- `FailureRecorded`: failure is durable; release has not committed.
- `ReleasePending`: queued, running, retrying, or exhausted.
- `ResourcesReleased`: local release committed.
- `AccountingCleanupPending`: local release committed; provider work remains.
- `AccountingCleanupCompleted` / `AccountingTransitionRequired`: provider outcome.

Mutation payloads return stable IDs and rendered status fields. Relay normalizes them and updates connections or refetches targeted queries; successful mutations never reload the browser.

## Provider failures

Missing Stripe setup responses and provider exceptions create explicit durable failure/cleanup state. Xero, Stripe, invoice, notification, event, or worker failure cannot prevent or undo a committed local release.
