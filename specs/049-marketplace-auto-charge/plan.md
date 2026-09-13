# Implementation Plan: Stripe-Billed Marketplace Auto-Renewal

**Branch**: `049-marketplace-auto-charge` | **Date**: 2026-09-06 | **Spec**: [spec.md](./spec.md)

## Summary

Replace manual every-term Checkout with Stripe Billing subscriptions created in the recipient Stripe Connect account. Initial Checkout uses `mode=subscription`; Stripe owns recurring invoices, payment attempts, SCA, and dunning. Existing Connect webhooks reconcile Stripe facts. `invoice.paid` idempotently creates the reservation term or entitlement grant. No application-managed renewal PaymentIntent scheduler is implemented.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript 6/React 19/Next.js 16
**Dependencies**: Stripe.NET Checkout/Billing/Connect, Temporal, EF Core/PostgreSQL, HotChocolate/Fusion, Relay 21
**Storage**: Booking PostgreSQL; existing append-only MarketplacePurchaseHistory; purchase-specific Stripe correlation fields on the renewal aggregates only
**Testing**: Unit, concurrency, Stripe webhook fixture, and Relay UI tests. Integration tests require explicit user approval and are not added when unit tests cover the behavior.
**Constraints**: Direct connected-account charges; no Customer-profile payment method; paid invoice gates grants; generated artifacts are regenerated from sources

**Rollout flag**: `Stripe:EnableMarketplaceBillingAutoRenewal` defaults to `true`. Set it to `false` to keep new AutoRenew purchases on explicit one-time checkout. While the current period is active, option B updates the connected-account Stripe Subscription item to the current recurring Price without customer confirmation, so Stripe uses it when it creates the next invoice.

## Constitution Check

- [x] **Contract-First** — Run `scripts/generate-graphql.sh` for backend GraphQL changes and `pnpm --dir src/web relay` for Relay operations; do not hand-edit generated files.
- [x] **Domain Boundaries** — Booking services/repositories own persistence; workflows and transports do not access EF directly.
- [x] **Testing** — Units cover mapping, adapters and webhook decisions; concurrency and Stripe fixture coverage are preferred. Integration tests require explicit user approval and are not added when unit tests cover the behavior.
- [x] **Frontend** — Relay mutations return rendered fields/stable IDs and use a refetch/declarative update, never reload; use existing typography wrappers and American English.
- [x] **Pattern Consistency** — Reuse connected-account Checkout, Stripe webhook subscriber, catalog records, Temporal reconciliation and MarketplacePurchaseHistory. Stripe Billing replaces the duplicate local payment scheduler.
- [x] **Logging** — Log/capture subscription setup, webhook deduplication, invoice decision, grants, cancellation, recovery, refunds and account failures with correlation IDs and no payment secrets. Dedicated feature metrics are not required.

## Design

1. Review `StripeProductPricingService` mapping. For an auto-renewable offer version, reuse one connected-account Stripe Product and lazily create one immutable recurring `StripePrice` row for each actually purchased pricing snapshot. The same `StripePrice` table also contains one-time Prices. Each recurring row carries the exact price, currency, term, billing mode, and tax treatment.
2. Change eligible initial reservation and entitlement Checkout to subscription mode in that account and write local purchase metadata onto both the Checkout Session and Stripe Subscription. Store purchase-specific Stripe correlation fields only from a completed Checkout Session or subscription webhook; never require `Session.SubscriptionId` during session creation.
3. Add a shared purchase-kind adapter: a paid invoice period becomes one reservation term or one entitlement/credit grant. It has no payment-attempt behavior.
4. Extend the existing Connect webhook subscriber. Deduplicate by Stripe event/object/account and route subscription/invoice facts through the purchase aggregate. The initial `subscription_create` invoice is owned by Checkout completion; only `subscription_cycle` paid invoices enter renewal provisioning. If a cycle invoice arrives before the local cycle is materialized, start an idempotent Temporal provisioning workflow keyed by connected account and invoice ID. Only paid cycle invoices call the renewal grant adapter.
5. Project failure/authentication/disconnection to append-only history and UI. Stripe Billing retry/dunning is authoritative; recovery links re-enter Stripe, not a local retry loop.
6. Synchronize AutoRenew and cancellation intent to Stripe Subscription cancellation before suppressing future local grants. Preserve Booking-owned cancellation/refund policy.
7. Roll out behind an organization capability flag. Legacy AutoRenew purchases without purchase-specific Stripe Subscription correlation retain explicit migration checkout recovery until a connected-account subscription is established; no Customer-profile payment method fallback is permitted.

## Provider Findings

- Stripe Connect supports direct-charge subscriptions when the Customer, Price, and Subscription are created authenticated as the connected account, and recommends provisioning access from `invoice.paid`.
- Stripe Billing owns cycle invoices, PaymentIntents, payment-action-required, and failure/retry events.
- Direct subscriptions support percentage platform fees through `application_fee_percent`; flat/dynamic fees require invoice-level handling and a product decision.
- Confirmed policy: Spaces uses no platform fee; Host uses the existing configured percentage, currently 5%, through `application_fee_percent`.

## Project Structure

```text
src/booking/shared/Booking.Shared/{Activities,Database,Repositories,Services,Workflows}/
src/booking/processors/Booking.Processors/Subscribers/BookingInternalSubscriber.cs
src/booking/api/Booking.Api/GraphQL/
src/web/apps/webapp/src/
specs/049-marketplace-auto-charge/
```

## Complexity Tracking

No local card store, renewal-cycle aggregate, PaymentIntent scheduler, payment-attempt table, or retry engine is planned. Add only the minimal connected-account Stripe Subscription link if existing purchase records cannot carry the association; reuse existing history for lifecycle facts.

## Gates Before Code

- Verify installed Stripe.NET Checkout/Subscription option names and webhook object fields.
- Verify exact recurring interval/count mapping for every supported membership term.
- Confirm direct-charge support/liability for all recipient Connect account types.
- Resolve platform-fee, changed-commercial-term, recovery-surface, and cancellation/current-access policy before enabling affected offers.
