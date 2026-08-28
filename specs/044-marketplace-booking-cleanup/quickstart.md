# Quickstart: Validate Cleanup Reliability

## Prerequisites

- Repository dependencies and the Booking integration-test database are available.
- Booking worker/job test dependencies are running.
- Use branch `044-marketplace-booking-cleanup`.

## Validation

1. Run focused Booking.Shared unit tests for card/bank-transfer workflows, arrears invoice workflows, release activities, failure service, and reconciliation decisions.
2. Run Booking integration tests for local transaction release, recurring instance deletion, repository eligibility/lease behavior, and replay/concurrency. Assert persistence through repositories, not `DbContext`.
3. Inject null Stripe product/pricing/customer/session responses and invoice/Xero setup failures that create no payment record; verify an explicit durable failure and cleanup eligibility.
4. Inject Xero/accounting failure; verify local release commits before accounting becomes pending/transition-required.
5. Verify local release retries at most five times with delayed/exponential backoff; exhaust them or simulate worker timeout, then verify an immediate reconciliation candidate, recurring reconciliation, and eventual release.
6. Verify status before and after local commit; confirm the UI never claims release early and mutation success updates Relay state without reload.
7. If GraphQL/Relay contracts change, run `scripts/generate-graphql.sh` and the documented web Relay generator, then run affected web tests/type checks.

Expected result: all terminal payment/invoice paths converge to released local resources, linked subscription ownership is honored, provider cleanup is recoverable, and historical orphaned allocations are repaired.

See [data-model.md](data-model.md) and [contracts/cleanup-reliability.md](contracts/cleanup-reliability.md).
