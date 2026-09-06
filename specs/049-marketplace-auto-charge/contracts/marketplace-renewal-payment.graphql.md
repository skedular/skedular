# GraphQL Contract: Marketplace Renewal Payment

This is the contract design. Source GraphQL definitions must be updated first and generated artifacts regenerated; this document is not a generated schema.

## Query surface

Customer and authorized administrator purchase views expose:

- `purchaseSpecificRenewalAuthorization`: readiness, consent/usability state, account-context summary, captured/revoked times, and a safe display summary if policy permits.
- `automaticRenewalPayment`: eligibility, current renewal status, next renewal, last attempt outcome, failure category, retry/expiry, fallback action, and authorized recovery actions.
- `renewalCycles`: stable IDs, term window, amount/currency/tax, status, attempt summary, fallback status, and entitlement quantity/validity when applicable.
- `lifecycleEvents`: append-only events with occurrence time, safe amount/currency data, outcome, and action state.

The customer-profile payment-method feature must never appear as the source of marketplace renewal readiness.

## Actions

Service-backed mutations/actions:

- Capture or refresh purchase-specific renewal authorization.
- Revoke purchase-specific renewal authorization.
- Create fallback checkout for a renewal.
- Retry a renewal payment.
- Complete authorized manual recovery.
- Cancel renewal/subscription with explicit immediate or period-end semantics.

Each response returns stable purchase and renewal IDs plus every field rendered by the initiating UI. It includes fallback URL/expiry only to an authorized viewer.

## Relay rules

- Normalize returned stable IDs and fields into the Relay store.
- Use declarative connection updates or targeted refetches for purchase/history connections.
- Never reload the browser after mutation success.
- Represent unavailable authorization, pending, action-required, failure, expired fallback, and permission-denied states explicitly.
