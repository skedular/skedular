# Data Model: Stripe-Billed Marketplace Auto-Renewal

## Principle

Stripe owns payment method, subscription, invoice, PaymentIntent, retry, and dunning state. Booking persists only purchase-specific Stripe correlation fields and authoritative event history required to decide whether to grant marketplace access.

## Stripe Catalog Mapping

For each offer version and recipient connected account, Booking reuses one Stripe Product for auto-renewal. It creates one `StripePrice` row for each actual one-time or recurring Stripe Price. A recurring row is created only when a pricing snapshot is actually used for an auto-renewable checkout or renewal. The row maps the immutable connected-account Stripe Price to the exact local pricing ID, amount, currency, membership term, billing mode, and tax-inclusion value.

This avoids exporting intermediate, unpublished pricing edits to Stripe while retaining the exact catalog Price required for a later renewal. One-time `StripePrice` rows remain available for nonrenewing Checkout and are never selected for AutoRenew.

## Purchase-specific Stripe Billing correlation

There is no separate link table. One current Stripe Billing correlation is stored directly on each auto-renewable marketplace purchase aggregate: `MarketplaceBookingSubscription` for reservations and `EntitlementPurchase` for credit entitlements.

| Field | Meaning |
|---|---|
| Purchase aggregate ID | Local reservation or entitlement identity. |
| `StripeAccountId` | Recipient connected account; required. |
| `StripeCustomerId` | Technical connected-account Stripe Customer reference; never a Customer-profile payment-method link. |
| `StripeSubscriptionId` | Required Stripe Billing subscription identity. |
| `StripePriceId` | Current recurring catalog Price. |
| Stripe Checkout Session / Subscription | Stripe is the authoritative record of the customer’s recurring-payment authorization; Booking stores only the correlation IDs, not a duplicate consent record. |
| `StripeSubscriptionStatus`, `StripeCurrentPeriodEndsAt`, `StripeCancelAtPeriodEnd` | Locally mirrored operational projection only. |

Constraints: each purchase aggregate has a unique `(StripeAccountId, StripeSubscriptionId)` index; no PAN, CVC, client secret, profile PaymentMethod ID, or complete Stripe payload. Both the Checkout Session and Stripe Subscription metadata carry `marketplace_purchase_type`, `marketplace_purchase_id`, and the selected recurring Stripe Price ID so webhook delivery order cannot cause an uncorrelated grant. `StripeAccountId` is a Connect direct-charge account; a destination-charge exception needs explicit approval/modeling.

## Existing MarketplacePurchaseHistory/Event Model

Reuse append-only history for provider and local facts:

- `StripeSubscriptionCreated`, `StripeSubscriptionUpdated`, `StripeSubscriptionCancelled`;
- `StripeInvoicePaid`, `StripeInvoicePaymentFailed`, `StripeInvoiceActionRequired`, `StripeInvoiceFinalizationFailed`;
- `ReservationRenewalGranted`, `EntitlementRenewalGranted`, `CreditAllocationCreated`;
- `RecoveryActionCreated`, `AutoRenewDisabled`, `StripeAccountDisconnected`;
- existing Booking-owned cancellation/refund events.

Persist connected account ID, Stripe event ID, subscription ID, invoice ID, invoice billing period, PaymentIntent ID when supplied, amount/currency, non-sensitive failure category, and local grant ID. Deduplicate provider events by connected account plus Stripe event ID; deduplicate grants by connected account plus paid invoice/period and local purchase.

## State Projection

```text
Checkout complete -> purchase aggregate pending/active
invoice.paid      -> paid-through projection + local grant
invoice failure   -> action-required/past-due projection; no grant
subscription end  -> cancelled/ended projection; no future grant
```

Stripe remains authoritative for payment status. The projection is operational/UI state; history events are historical truth.

## Explicitly Not Modeled

- Local card/payment-method table.
- Local renewal-cycle, payment-attempt, or fallback-payment table.
- Local PaymentIntent or retry/dunning scheduler.
