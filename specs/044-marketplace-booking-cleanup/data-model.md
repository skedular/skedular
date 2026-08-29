# Data Model: Reliable Marketplace Booking Cleanup

## Marketplace booking

Existing one-time or recurring booking with payment/failure state, generated instances, and resource allocations. It may link to a marketplace subscription or another billed payment owner.

## Effective payment owner

The booking payment record or linked subscription/payment aggregate that determines confirmation, expiry, rejection, and entitlement. Use the linked owner when present; otherwise use the booking payment. A durable terminal failure record is an alternative eligibility source when payment creation never completed. Persist the resolved owner or failure source in cleanup decisions.

## Local cleanup operation

Durable operation keyed uniquely by the customer-facing cleanup boundary and reason. Fields: operation ID, booking/subscription ID, effective payment owner ID, reason, status, attempts, last error, timestamps, and workflow/reconciliation correlation ID. States include `Pending`, `InProgress`, `ReleasePending`, `Released`, and `Skipped`.

## Accounting cleanup operation

Durable follow-up linked to local cleanup, with provider, external ID, attempts, and last error. States include `Pending`, `TransitionRequired`, `Completed`, and `Failed`. It cannot roll back local `Released` state.

## Reconciliation run

Durable scan/lease record containing window, candidate count, enqueue count, failures, and completion. Candidates require a rejected/expired effective payment or durable terminal failure record, remaining allocations, and no confirmed entitlement. Retry exhaustion creates an immediate candidate in addition to recurring scans.

## State transitions

```text
FailureRecorded → ReleasePending → Released → AccountingCleanupPending → AccountingCompleted
                                      └──────→ AccountingTransitionRequired
ReleasePending → ReleasePending (retry/replay)
Released → Released (idempotent replay)
```

Subscription-owned failures use the subscription as the customer-facing boundary and must prevent renewal/resource recreation.
