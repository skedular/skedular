# Tasks: Stripe-Billed Marketplace Auto-Renewal

**Status**: Implementation in progress. Open product/payment policy gates remain explicitly tracked below.

## Phase 0 — Resolve provider and product gates

- [X] T001 Inventory current Product/Price mapper and all `MembershipTerm` values; document recurring Stripe interval/count mapping and unsupported-term behavior.
- [X] T002 Confirm direct-charge eligibility/liability for each recipient Connect account type and approve platform-fee configuration. Decision: Skedular Spaces uses a direct connected-account subscription with no platform commission; Skedular Host retains the existing configured commission, currently 5%, through Stripe Billing `application_fee_percent`.
- [X] T003 Stripe recovery uses Customer Portal; option B selected: at each renewal, the current matched marketplace price and term are adopted automatically without customer confirmation, subject to Stripe subscription-price update success.
- [X] T004 Review the partial uncommitted local renewal-PaymentIntent implementation against this design; obtain explicit approval before deleting/replacing any of it. The user approved removal on 2026-09-06; the five-table model, its migration, and dependent custom payment code were removed.

## Phase 1 — Shared payment boundary

- [X] T005 Add purchase-specific Stripe Billing correlation fields to reservation and entitlement aggregates, with unique connected-account subscription correlation and a forward-only migration.
- [X] T006 Make AutoRenew Product/Price provisioning recurring in the recipient connected account: reuse one Stripe Product per offer version/account and lazily map each actually-used immutable recurring Price; retain one-time prices for nonrenewing purchases.
- [X] T007 Change eligible initial reservation and entitlement Checkout to Stripe subscription mode; persist only completion and purchase-specific Stripe authorization-correlation metadata.
- [X] T008 Add shared paid-invoice provisioner adapter contract for reservations, entitlements, and future AutoRenew types.

## Phase 2 — Webhook and lifecycle

- [X] T009 Extend existing Connect webhook subscriber for subscription/invoice/account events with event/account/invoice-period idempotency.
- [X] T010 Implement reservation paid-invoice grant using existing term/availability/resource workflows; no payment logic in booking cadence.
- [X] T011 Implement entitlement paid-invoice grant with credit quantity and validity period, exactly once.
- [X] T012 Synchronize AutoRenew and cancellation intent with Stripe Subscription cancellation; preserve Booking refund ownership.
- [X] T013 Persist event-backed history, structured logs, notifications, and approved Stripe recovery actions. Stripe lifecycle handlers persist history, emit purchase GraphQL change notifications, and expose Customer Portal recovery.

## Phase 3 — APIs, rollout, validation

- [X] T014 Add GraphQL/Relay automatic-payment status/history/recovery UI, regenerate schemas/artifacts, and test targeted Relay updates. Recovery uses separate subscription and entitlement mutations and Stripe Customer Portal.
- [X] T015 Add capability-flag rollout and legacy migration-checkout behavior; prohibit Customer-profile payment-method fallback. `Stripe:EnableMarketplaceBillingAutoRenewal` gates newly-created Billing subscriptions; disabling it preserves one-time checkout recovery, and existing linkless purchases never use Customer-profile payment methods.
- [X] T016 Add unit, concurrency, and Stripe webhook fixture coverage; run focused builds/tests and generated-artifact validation. Existing webhook/concurrency coverage and new payment-status unit coverage are retained; Booking API, Booking Shared, and Booking Processors builds pass. Do not add integration tests unless the user explicitly approves them; use unit tests when they cover the behavior.

- [X] T018 Add durable Temporal recovery for a paid Stripe invoice that races local marketplace-cycle creation. Use connected-account/invoice identity for idempotency and retry the existing provisioner; add no local payment-attempt table, counter, metric, or integration-test suite.

## Dependencies

`T001-T004 -> T005-T008 -> T009-T013 -> T014-T016`. `T010` and `T011` may proceed in parallel after `T008-T009`.
