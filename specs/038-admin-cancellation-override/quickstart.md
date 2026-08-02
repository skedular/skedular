# Quickstart: Admin Cancellation Policy Override

## Prerequisites

- A product-owning Spaces or Host organization.
- One owner/admin with existing booking/subscription management permission.
- One administrator without that permission.
- A customer account.
- Test booking and subscription records with a policy that currently rejects customer cancellation.
- Test payment variants for Stripe, bank transfer, and Xero where supported.

## Validation scenarios

1. **Customer booking rejection**
   - Request cancellation after the booking cutoff as the customer.
   - Expect a policy restriction, unchanged booking state, and no override audit event.

2. **Authorized booking override**
   - Request cancellation as the product-owning owner/admin with a short reason.
   - Expect cancellation to succeed despite the customer policy and the reason/actor/policy outcome to be auditable.

3. **Unauthorized administrator**
   - Repeat the operator request as an administrator without booking/subscription management permission.
   - Expect an authorization failure and no cancellation or refund side effect.

4. **Subscription modes**
   - As an authorized operator, exercise immediate and period-end cancellation.
   - Expect immediate cancellation to stop entitlement now and period-end cancellation to disable renewal while preserving the current cycle.

5. **Provider refund matrix**
   - Trigger an eligible Stripe refund and verify automatic processing.
   - Trigger an eligible bank-transfer refund and verify it waits for owner/admin approval and transfer confirmation.
   - Trigger an eligible Xero refund and verify it waits for the existing approval and Xero processing/reconciliation path.

6. **Idempotency and audit**
   - Replay an immediate cancellation and exercise a concurrent/retried request.
   - Expect one cancellation boundary, no duplicate refund/provider action, and one coherent audit outcome.

## Test guidance

- Add unit coverage first for actor resolution, permission checks, reason validation, policy bypass, mode semantics, and provider routing.
- Add focused integration coverage only for persistence/audit concurrency and GraphQL schema wiring.
- Assert persistence through repository/query methods rather than direct Entity Framework access in integration tests.

## Validation status

The code-level portions of this quickstart are covered by Booking shared/API unit tests, generated GraphQL schema
validation, Relay generation, and local webapp unit tests. Full provider scenarios were not executed in this workspace
because they require configured Stripe, bank-transfer settlement, and Xero accounts plus authenticated owner/admin and
customer fixtures. Those scenarios remain a deployment-environment verification step; no code gap was identified from
the available local test boundary.
