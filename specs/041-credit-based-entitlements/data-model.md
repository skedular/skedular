# Data Model: Credit-Based Booking Entitlements

## ProductPricing

Existing pricing gains/uses entitlement fulfillment configuration: fulfillment type, token quantity, validity, activation/expiry rules, allowed weekdays, product/resource scope, cancellation/refund policy, supported payment methods, and auto-renew setting. Persisted enum-like values remain strings with explicit mappings.

## EntitlementPurchase

Booking-owned standalone commercial purchase.

- `Id`, `CustomerId`, `OrganizationId`, `ProductId`, `ProductVersionId`, `ProductPricingId`
- immutable pricing snapshot: quantity, validity, restrictions, refund/renewal policy, amount, currency
- payment method/state, Stripe checkout/session context, invoice context, payment deadline
- lifecycle state, confirmed timestamp, entitlement-cycle link, renewal reference, audit timestamps

No booking, resource, or quota foreign key is created for purchase initiation.

## Entitlement

Customer right to future qualifying usage.

- customer/organization/product/pricing ownership and immutable cycle snapshot
- configured and available token quantities
- activation and expiry timestamps in location timezone
- lifecycle state: pending, active, expired, canceled, renewal-failed, refunded/settlement-pending as applicable
- auto-renew and cycle/parent purchase references
- refund policy/status linkage

## CreditLedgerEntry

Immutable quantity movement for grant, consumption, release, forfeiture, expiry, or adjustment. Transaction type is stored as a string and exposed as an enum in models/transport. Metadata follows the repository’s JSON/JSONB type convention.

## Booking linkage

An ordinary marketplace booking may reference the entitlement and consuming ledger entry. The relationship is created only when the customer or authorized operator later makes a qualifying booking. Resource allocation remains atomic with token consumption.

## Renewal state

Renewal tracks source entitlement/purchase, current pricing match, next renewal time, auto-renew/cancel-at-period-end choice, payment workflow state, retry/failure reason, and the newly confirmed cycle. A unique idempotency/reference key prevents duplicate cycles.

## State rules

- Purchase pending/rejected/expired: no entitlement grant.
- Confirmed purchase: exactly one entitlement cycle and grant.
- Cycle expiry: unused balance becomes expired/forfeited or enters the configured refund workflow; existing bookings/history remain.
- Auto-renew success: exactly one new cycle after confirmed payment.
- Auto-renew pending/failure: current cycle expires; no new tokens before confirmation.
- No compatible current token pricing: renewal fails safely and remains auditable.
- Booking cancellation/modification: existing cancellation policy determines restore/forfeit; modification preserves one consumed token.

## Validation

Use existing authorization, product scope, weekday, validity, duration, resource availability, cancellation deadline, payment confirmation, and repository concurrency rules. Operator actions must be scoped to the organization and record operator/customer actors.
