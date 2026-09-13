# Tasks: Stripe-Billed Marketplace Auto-Renewal

**Status**: Implementation complete; deployment and live Stripe validation remain operational gates.

## Phase 0 — Resolve provider and product gates

- [X] T001 Inventory current Product/Price mapper and all `MembershipTerm` values; document recurring Stripe interval/count mapping and unsupported-term behavior.
- [X] T002 Confirm direct-charge eligibility/liability for each recipient Connect account type and approve platform-fee configuration. Decision: Skedular Spaces uses a direct connected-account subscription with no platform commission; Skedular Host retains the existing configured commission, currently 5%, through Stripe Billing `application_fee_percent`.
- [X] T003 Stripe recovery uses Customer Portal; Option B is selected: at each renewal, the current exact marketplace price and term are adopted automatically without customer confirmation, subject to Stripe subscription-price update success.
- [X] T004 Review the partial uncommitted local renewal-PaymentIntent implementation against this design; obtain explicit approval before deleting/replacing any of it. The user approved removal on 2026-09-06; the five-table model, its migration, and dependent custom payment code were removed.

## Phase 1 — Shared payment boundary

- [X] T005 Add root-purchase Stripe Billing correlation fields to reservation and entitlement aggregates, with invoice/payment correlation on renewal records and a forward-only migration.
- [X] T006 Make AutoRenew Product/Price provisioning recurring in the recipient connected account: reuse one Stripe Product per offer version/account and lazily map each actually-used immutable recurring Price; retain one-time prices for nonrenewing purchases.
- [X] T007 Change eligible initial reservation and entitlement Checkout to Stripe subscription mode; persist only completion and purchase-specific Stripe authorization-correlation metadata.
- [X] T008 Add shared paid-invoice provisioner adapter contract for reservations, entitlements, and future AutoRenew types.

## Phase 2 — Webhook and lifecycle

- [X] T009 Extend existing Connect webhook subscriber for subscription/invoice/account events with event/account/invoice-period idempotency.
- [X] T010 Implement reservation paid-invoice grant using existing term/availability/resource workflows; no payment logic in booking cadence.
- [X] T011 Implement entitlement paid-invoice grant with credit quantity and validity period, exactly once.
- [X] T012 Synchronize AutoRenew and cancellation intent with Stripe Subscription cancellation; preserve Booking refund ownership.
- [X] T013 Persist event-backed history, structured logs, notifications, and approved Stripe recovery actions. Stripe lifecycle handlers persist history, emit purchase GraphQL change notifications, and expose purchase-specific Customer Portal recovery.

## Phase 3 — APIs, rollout, validation

- [X] T014 Add GraphQL/Relay automatic-payment status/history/recovery UI, regenerate schemas/artifacts, and test targeted Relay updates. Recovery uses separate subscription and entitlement mutations and Stripe Customer Portal.
- [X] T015 Use Stripe Billing directly for new AutoRenew purchases; prohibit Customer-profile payment-method fallback. Existing purchases without a purchase-specific Stripe subscription remain non-automatic until the customer completes an explicit new authorization checkout.
- [X] T016 Add unit, concurrency, and Stripe webhook fixture coverage; run focused builds/tests and generated-artifact validation. Existing webhook/concurrency coverage and new payment-status unit coverage are retained; Booking API, Booking Shared, and Booking Processors builds pass. Do not add integration tests unless the user explicitly approves them; use unit tests when they cover the behavior.

- [X] T018 Add durable Temporal recovery for a paid Stripe invoice that races local marketplace-cycle creation. Use connected-account/invoice identity for idempotency and retry the existing provisioner; add no local payment-attempt table, counter, metric, or integration-test suite.
- [X] T019 Add durable Temporal recovery for failed Stripe subscription cancellation synchronization, covering reservation and entitlement cancellation without introducing a local payment-attempt table, counter, or metric.
- [X] T020 Ensure recurring Price lookup never falls back to a non-matching recurring price, register paused/resumed subscription webhooks, and persist fallback reservation paid-invoice history.
- [X] T021 Apply organization billing-cycle installment pricing to Stripe subscriptions for longer in-arrears membership terms; gate reservation materialization and entitlement grants on each paid Stripe billing period.

## Dependencies

`T001-T004 -> T005-T008 -> T009-T013 -> T014-T016`. `T010` and `T011` may proceed in parallel after `T008-T009`.
