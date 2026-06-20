# Quickstart Validation: Booking Failure Communications

## Prerequisites

- Run the Booking test dependencies through the repository's existing local/Aspire setup.
- Apply the Booking migration created for failure outcomes and deliveries.
- Regenerate GraphQL schema/Relay artifacts after source GraphQL contract changes:

  ```bash
  scripts/generate-graphql.sh
  ```

## Automated Validation

Run the focused Booking unit, integration, and workflow/activity tests added for this feature, then the marketplace web component tests. Follow the package/project commands selected in `tasks.md`; do not query the database directly from integration tests.

## End-to-End Scenarios

1. **Pre-submit unavailable time**: Open a product with no capacity for the selected window. Confirm local availability feedback, no failure aggregate, and no email/delivery record.
2. **Competing one-time submissions**: Submit two customers against one resource/window concurrently. Confirm exactly one allocation, one retained availability failure for the loser, one customer/authorized stakeholder email + retained in-app outcome, and no duplicate delivery on replay.
3. **Selected-resource conflict**: Submit after a selected resource becomes unavailable. Confirm typed availability result and rebook action rather than generic error.
4. **One-time payment expiry**: Allow checkout/payment to expire. Confirm slot release, retained immutable payment-failure record, customer/stakeholder communications, and only a new booking can recheck availability.
5. **Initial series conflict**: Request a multi-day/series purchase with one unavailable occurrence. Confirm no occurrence is presented as confirmed, all claimed capacity is released, and one series-level failure is delivered.
6. **Later recurring occurrence conflict**: Make a later occurrence unavailable during reconciliation. Confirm one occurrence failure and one immediate communication; the subscription and unrelated occurrences remain intact.
7. **Recurring-cycle payment expiry**: Expire a cycle payment. Confirm only unpaid-cycle bookings release, the subscription configuration remains, and duplicate workflow delivery does not duplicate communications.
8. **Delivery failure/retry**: Force email delivery failure. Confirm final booking outcome remains authoritative, delivery is retryable/visible, and a later retry changes the existing delivery record rather than generating another communication.

## Expected Observability

For each scenario, inspect structured logs for a common correlation identifier and the sequence: submission/allocation decision, finalization, capacity release where applicable, dispatch queued, delivery result/retry. Confirm logs distinguish availability, payment, validation, and technical categories.

## Validation Record

- Focused Booking shared failure/subscription unit and workflow/activity suites pass, including the explicit initial-series unavailable-day activity fixture.
- Marketplace web suite: 74 files and 225 tests passed.
- Booking domain integration failure/claim repository suite: 6 tests passed against the Aspire/PostgreSQL test dependencies.
- Booking subscription activity integration suite: 3 tests passed, including durable in-application delivery dispatch.
- Booking domain integration project builds successfully, including repository-backed concurrent claim and failure-retention tests.
- The automated unit, activity, repository, and web suites above are the feature validation boundary. Manual end-to-end execution is not required for this implementation.
