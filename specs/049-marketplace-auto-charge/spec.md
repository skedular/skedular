# Feature Specification: Stripe-Billed Marketplace Auto-Renewal

**Feature Branch**: `049-marketplace-auto-charge`
**Created**: 2026-09-06
**Status**: Draft

## Outcome

For every card-paid marketplace purchase with `AutoRenew`, the initial purchase uses Stripe Checkout in subscription mode. Stripe Billing then automatically invoices and charges the card for each contractual term. Skedular reacts to the existing Stripe Connect webhook: when Stripe reports a paid invoice, it creates the next reservation term or credit entitlement.

The recipient’s Stripe Connect account receives the money. The default is a Stripe **direct charge**: the Product, recurring Price, Customer, Subscription, invoices, and charges exist in that connected account. This is not a platform charge followed by a destination transfer.

Skedular does not build a card vault, a renewal PaymentIntent scheduler, or a retry engine. Stripe owns payment methods, subscription billing, PaymentIntents, automatic collection, 3DS/SCA recovery, and retry/dunning. Skedular keeps only the minimum local association and authoritative history needed to provision the marketplace purchase and support operations.

The Customer-domain/customer-profile `StripePaymentMethod` is explicitly excluded. It is never selected, copied, or used as a fallback. The technical Stripe Customer/payment method created by the marketplace subscription checkout belongs only to that Stripe Subscription in the connected account.

## User Scenarios

### Reservation subscription (P1)

**Given** a card-paid reservation purchase with AutoRenew, **when** the initial Checkout subscription succeeds, **then** Stripe automatically collects subsequent membership-term invoices without asking the customer to open a payment link.

**Given** Stripe sends `invoice.paid` for a renewal, **then** Skedular creates or advances exactly one paid reservation term and applies availability, resource assignment, and booking rules separately from payment.

### Credit entitlement (P1)

**Given** a card-paid AutoRenew credit-entitlement purchase, **when** Stripe sends `invoice.paid`, **then** Skedular creates exactly one renewed entitlement purchase and credit allocation with its configured credit quantity and validity period.

Credit quantity and validity period are not booking cadence.

### Failure and recovery (P1)

**Given** Stripe cannot collect payment or needs customer authentication, **when** it sends its invoice/subscription webhook, **then** Skedular grants neither access nor credits, records the fact, notifies the customer, and exposes the approved Stripe recovery action. A three-day hosted recovery checkout/link remains available when required by the product policy.

### History and support (P2)

**Given** a subscription, invoice, grant, payment failure, cancellation, expiry, or refund event occurs, **then** the customer/admin history is driven by a persisted event—not inferred from current mutable fields.

## Requirements

- **FR-001**: Use one shared Stripe Subscription integration for all card-paid marketplace purchase types that support `AutoRenew`: reservations, credit entitlements, and future purchase types through a purchase-kind adapter.
- **FR-002**: `AutoRenew` is the sole switch for whether a marketplace purchase renews. `MembershipTerm`/`PurchaseCadence` is the contract term, not reservation booking frequency.
- **FR-003**: Initial eligible checkout MUST use Stripe Checkout `mode=subscription`, with a recurring Stripe Price representing the contract term. The existing `mode=payment` checkout remains for non-auto-renewing purchases and approved recovery flows.
- **FR-004**: Create the recurring Product/Price, Customer, Checkout Session, and Subscription while authenticated as the recipient connected account. The charge proceeds directly to that account.
- **FR-005**: The initial Stripe subscription checkout MUST capture the Stripe-required recurring-payment authorization. It is scoped to the resulting marketplace subscription and connected account; it is not the Customer-profile payment-method feature.
- **FR-006**: Stripe Billing owns invoices, recurring PaymentIntents, payment retries/dunning, billing dates, and 3DS/SCA. Skedular MUST NOT create a local per-term off-session PaymentIntent or retry loop.
- **FR-007**: Stripe Product/Price is catalog data. AutoRenew MUST reuse one connected-account Stripe Product per offer version and create an immutable recurring Price only when that exact pricing snapshot is used; do not create a Product/Price per invoice or renewal term.
- **FR-008**: Grant a reservation term, entitlement, or credit allocation only after durable, idempotent reconciliation of the connected-account `invoice.paid` webhook. Neither checkout completion, a subscription status change, nor a PaymentIntent creation is paid proof for a future term.
- **FR-009**: Reservation provisioning remains independent of payment and continues applying the renewed offer, membership term, availability, resource requirements, and booking rules.
- **FR-010**: Entitlement provisioning remains independent of reservation cadence and creates one entitlement/credit allocation only after its paid invoice.
- **FR-011**: Reconcile Checkout Session, Subscription, Invoice, and webhook events using connected-account ID plus Stripe event/object IDs and the paid invoice period. Replays, concurrent webhooks, workflows, retries, and recovery MUST NOT cause a duplicate grant or duplicate refund.
- **FR-012**: Handle `checkout.session.completed`, `customer.subscription.created|updated|deleted|paused|resumed`, `invoice.paid`, `invoice.payment_action_required`, `invoice.payment_failed`, invoice finalization failure, relevant PaymentIntent outcomes, and connected-account deauthorization/status events through the existing Stripe webhook path.
- **FR-013**: On action-required, failed, expired-card, insufficient-funds, missing subscription, disconnected account, or provider failure, do not grant. Persist an event, notify the customer, and show the approved Stripe recovery route; never fall back to a Customer-profile card.
- **FR-014**: If price, currency, tax, billing mode, term, entitlement quantity, or validity changes, do not silently alter the customer’s recurring commercial commitment. Require the approved customer-confirmation flow before changed billing/granting.
- **FR-015**: Disabling AutoRenew, immediate cancellation, and cancel-at-period-end MUST synchronize with Stripe Subscription cancellation and stop future grants while preserving already-paid terms and history.
- **FR-016**: Booking owns refund eligibility after confirmed payment. A subscription cancellation is one customer-facing reservation refund boundary; entitlement refunds consider unused credits only under the cancellation policy.
- **FR-017**: Existing AutoRenew purchases without a new connected-account Stripe Subscription remain safely non-automatic and receive an explicit migration/recovery checkout at their next renewal. No historical consent is inferred from a one-time checkout.
- **FR-018**: UI/API must show automatic-payment readiness, Stripe subscription/invoice status, next term, failure/recovery action, and authoritative history without exposing card data or using a browser reload after Relay mutations.
- **FR-019**: Persist append-only provider/local events for subscription lifecycle, invoice outcomes, local grants, cancellation, expiry, refund, recovery, and manual intervention. Aggregate fields may summarize but never fabricate history.
- **FR-020**: Emit correlated structured logs and audit/history events for subscription creation, webhook receipt/deduplication, invoice outcomes, grants, cancellations, refunds, recovery, and connected-account failures, excluding payment secrets and card data.

## Minimal Local State

Persist purchase-specific Stripe Billing correlation directly on each auto-renewable purchase aggregate:

- local purchase type and ID (the aggregate row is the local identity);
- connected Stripe account ID;
- technical Stripe Customer ID and Stripe Subscription ID;
- recurring Stripe Price ID and consent version/time;
- reservation values live on `MarketplaceBookingSubscription`; entitlement values live on `EntitlementPurchase`;
- Stripe Checkout Session and Subscription metadata contain the purchase type, purchase ID, and selected recurring Price for operational correlation; and
- mirrored operational status, current paid-through period, and cancellation intent.

Reuse the existing append-only marketplace purchase history/event model for Stripe invoice and local provisioning facts. A provider event ID and invoice/period identity provide idempotency. Do not add local renewal-cycle, payment-attempt, payment-method, fallback, or retry tables unless later evidence proves the existing history/idempotency store cannot provide this minimal correlation.

## State Model

```text
Initial subscription Checkout -> Stripe subscription active
Stripe invoice.paid           -> exactly one local term / entitlement granted
invoice payment failed/action -> no grant; notify and show Stripe recovery
Stripe invoice.paid after recovery -> exactly one grant
AutoRenew disabled/cancelled  -> synchronize subscription; no future grant
```

## Success Criteria

- Normal eligible renewals need no customer action; Stripe bills them automatically.
- 100% of local grants trace to one paid invoice in the receiving connected account.
- Webhook replays and concurrent processing create no duplicate reservation term, entitlement, credit allocation, charge, or refund.
- Existing purchases without a Stripe subscription remain recoverable but are never charged using a Customer-profile card.

## Unresolved Decisions

- Confirm every recipient Connect account type can use direct-charge subscriptions. Destination charges are an explicit exception only, not the default.
- Confirm whether platform fees are a stable percentage (`application_fee_percent`) or require a policy-driven invoice fee.
- Choose the approved recovery UI: Stripe Customer Portal, hosted invoice payment/authentication, or replacement Checkout. Keep the three-day recovery link requirement.
- Define the customer-confirmed migration for changed price/tax/term/credit quantity/validity; safe default is no silent change and no grant.
- Verify all supported membership terms map exactly to Stripe recurring interval/count; define a product policy for unsupported terms.

## Dependencies

- Existing connected-account Product/Price provisioning, current hosted Checkout, Stripe Connect webhook subscriber, MarketplacePurchaseHistory, Temporal reservation reconciliation, entitlement services, GraphQL/Relay, and forward-only EF migrations.
- This is design-only. Implementation waits for internally consistent `spec.md`, `plan.md`, `research.md`, `data-model.md`, contracts, quickstart, and dependency-ordered tasks.
