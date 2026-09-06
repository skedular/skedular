# Stripe Contract: Marketplace Renewal Payment

## Initial or fallback authorization

1. Resolve the purchase, Connect account context, and commercial values.
2. Create checkout/authorization flow with explicit future off-session consent for that purchase only.
3. Use Stripe’s supported future-payment setup during payment or setup-only authorization.
4. On provider-confirmed completion, persist only Stripe references, account context, consent evidence, and non-sensitive usability state in `PurchaseRenewalAuthorization`.

The flow neither reads nor updates Skedular’s existing customer-profile payment method for renewal eligibility.

## Off-session renewal

1. Load authoritative renewal cycle and authorization.
2. Recalculate price, currency, tax, billing mode, membership term, reservation rules, or entitlement quantity/validity.
3. If amount/tax changed from the original purchase, generate three-day fallback checkout; do not auto-charge.
4. Otherwise create/reuse the cycle’s idempotent off-session PaymentIntent using the purchase-specific Stripe credential.
5. Preserve direct/destination charge semantics and verify account/credential ownership.
6. Reconcile confirmed provider outcome before materialization.

## Webhook rules

Process succeeded, failed, canceled, processing/asynchronous, and authentication-required PaymentIntent outcomes using account context plus provider and local attempt IDs. Duplicate/out-of-order delivery must converge through valid local transitions and event idempotency.

## Safety rules

- Never persist card data, client secrets, or full provider payloads in logs.
- No retry after confirmed success/cancellation, expired/detached authorization, or authentication-required outcome.
- Account/credential/tax ambiguity produces no charge/no grant and fallback/manual recovery.
