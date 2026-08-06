# Quickstart: Validate Marketplace Purchases History

## Prerequisites

Use an organization/operator with a confirmed hourly booking, active renewing subscription, period-end subscription, canceled/deleted standalone booking, and refund/payment-failure record.

## Contract and test validation

1. Review [data model](data-model.md) and [GraphQL contract](contracts/graphql.md).
2. Run `scripts/generate-graphql.sh` after source schema work, then regenerate Relay artifacts through the repository workflow.
3. Run focused Booking unit/integration tests and Spaces/Host Vitest suites.

## Operator validation

1. Open the existing `/subscriptions` route in Spaces and Host. Confirm its visible name is Marketplace purchases and old links resolve.
2. Confirm page one shows all retained purchases, newest activity first, with standalone and subscription sources each exactly once.
3. Apply source, lifecycle, payment, refund, customer, product, renewal, cadence, and date filters. Verify counts and results.
4. Switch list/grid views; confirm filters/order/page contents and navigation targets are identical.
5. Page forward/backward and refresh after a state change; confirm no missing or duplicate retained results.
6. Open a subscription and verify generated booking instances have their own filtered/paginated list, not duplicate main entries.
7. From a generated booking, open the parent subscription link. Confirm standalone bookings show no false parent link.
8. Inspect canceled/deleted, failed, and refunded records. Confirm distinct lifecycle, payment, refund, and retained evidence.

## Regression and documentation

- Create an hourly booking and confirm it does not create subscription/renewal/allocation work.
- Cancel immediate and period-end subscriptions; confirm existing workflows remain authoritative.
- Confirm unauthorized users cannot query another organization.
- Review structured logs for scope/counts and incomplete-link warnings without PII/payment data.
- Update Spaces/Host/shared subscription operator documentation to explain the consolidated view and preserve the one-time-booking rule.
