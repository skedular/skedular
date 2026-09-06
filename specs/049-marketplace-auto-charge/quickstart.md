# Quickstart Validation: Automatic Marketplace Renewal Charging

## Prerequisites

- Branch `049-marketplace-auto-charge` and normal local dependencies.
- Stripe test fixtures for success, insufficient funds, authentication required, canceled, and delayed outcomes.
- AutoRenew reservation subscription and credit-entitlement purchases.
- Purchase-specific authorization captured through that purchase’s checkout—not a customer-profile card.

## Unit validation

Verify:

1. Missing/revoked/detached/expired authorization and profile-card-only state are ineligible.
2. Renewal freezes current pricing, tax, billing mode, term, and entitlement values.
3. Amount/tax changes create fallback rather than off-session charge.
4. Automatic retry stops after three attempts/three days and skips non-retryable failures.
5. Workflow/webhook replay yields one provider attempt and one materialization.
6. Reservation cycles and credits are created only after confirmed payment.
7. Refund policy returns only unused credits and stays separate from cancellation.

## Integration validation

1. Apply the forward-only migration to a disposable database and verify foreign keys, uniqueness, and event indexes.
2. Exercise direct and destination charge contexts; invalid ownership yields no charge/no grant.
3. Deliver success, failed, canceled, processing, authentication-required, duplicate, and out-of-order webhooks.
4. Replay after workflow timeout before webhook delivery and verify no duplicate charge.
5. Expire fallback after three days and verify prior active term is not implicitly canceled.

## GraphQL and UI validation

1. Query ready, missing authorization, pending, action-required, failed, fallback-required, paid, canceled, and expired states.
2. Verify customer-profile card status never determines renewal readiness.
3. Verify authorization/fallback/retry/recovery mutations update Relay without reload.
4. Verify customer/admin history contains persisted lifecycle events only.
5. Verify permissions, accessible actions, loading/error states, and American English copy.

## Operational validation

- Locate payment-pending and webhook-conflict records by correlation IDs.
- Replay/manual-recover safely and verify idempotency.
- Confirm logs include purchase, renewal, attempt, workflow, PaymentIntent, and event IDs but no sensitive payment data.
