# Feature Specification: Automatic Marketplace Renewal Charging

**Feature Branch**: `049-marketplace-auto-charge`
**Created**: 2026-09-06
**Status**: Draft
**Input**: User description: Create a shared automatic credit-card charging flow for every auto-renewable marketplace purchase, including reservation subscriptions and credit-entitlement purchases, with off-session payment, fallback checkout, lifecycle history, and recovery.

## Clarifications

### Session 2026-09-06

- Q: Should a customer’s off-session payment consent apply only to one marketplace purchase, to all eligible auto-renewable purchases for the same merchant, or globally across the customer’s purchases? → A: Consent applies to all eligible auto-renewable marketplace purchases for the same merchant and customer, with explicit revocation. Superseded by the later scope correction below: renewal authorization is purchase-specific and the customer-profile card is excluded.
- Q: When the current renewal amount or tax differs from the original purchase, should the system charge automatically, require customer confirmation for any difference, or use a configured material-change threshold? → A: Require customer confirmation through fallback checkout for any amount or tax difference.
- Q: When a renewed credit entitlement is cancelled or refunded after some credits have been used, should the refund cover only unused credits, allow a full refund, or require administrator review? → A: Refund only unused credits according to the cancellation policy.
- Q: How many automatic payment attempts should the system make after an initial renewal failure, and over what period? → A: Three automatic attempts over three days, then fallback/manual recovery.
- Q: If the payment method originally associated with a marketplace purchase is removed or becomes unusable, may the system automatically charge the customer’s current default saved card? → A: No. Use only the purchase-specific Stripe-side renewal authorization; require customer action to create or replace that purchase authorization.
- Q: How long should a fallback checkout link remain valid after it is created? → A: Three days.
- Scope correction: The existing card stored on the customer profile is explicitly out of scope and MUST NOT be used for marketplace renewal charging. Marketplace renewal uses a separate purchase-specific Stripe-side payment authorization created or retained by the purchase checkout flow.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Automatically pay for a reservation renewal (Priority: P1)

As a customer with an auto-renewable reservation offer, I want the next reservation term to be charged using the purchase’s separately authorized Stripe-side renewal payment authorization so that my access continues without opening a new checkout page.

**Why this priority**: Reservation renewal is the current auto-renewal path and is the primary customer-visible reduction in manual work.

**Independent Test**: Start with an active auto-renewable reservation purchase, a valid purchase-specific Stripe-side renewal payment authorization and explicit off-session consent. Advance the renewal time, confirm one renewal charge, and verify that the renewed reservation cycle is not treated as paid until payment confirmation is received.

**Acceptance Scenarios**:

1. **Given** an active card-paid reservation subscription with AutoRenew enabled, a valid purchase-specific Stripe-side renewal payment authorization, and recorded off-session consent, **When** its renewal date arrives, **Then** the system calculates the new cycle from the renewed purchase and attempts one automatic charge for its current amount and currency.
2. **Given** the renewal PaymentIntent succeeds, **When** the success is confirmed, **Then** the renewed subscription/cycle becomes paid and eligible reservation resources are created or retained according to the renewed offer, term, availability, and booking rules.
3. **Given** the automatic charge cannot complete, **When** the failure is classified, **Then** the renewal remains unpaid, the customer receives a fallback checkout link, and AutoRenew is not silently represented as a successful renewal.

### User Story 2 - Automatically pay for a credit-entitlement renewal (Priority: P1)

As a customer with an auto-renewable credit-entitlement purchase, I want the renewed quantity of credits to be charged and granted only after payment succeeds, so that my balance and validity period remain accurate.

**Why this priority**: Credit entitlements are a distinct auto-renewable purchase type and must use the same payment promise without being modeled as a reservation.

**Independent Test**: Start with an active auto-renewable entitlement, a valid purchase-specific Stripe-side renewal payment authorization and consent. Advance renewal, verify the renewed quantity, price, and validity period are calculated from the renewed purchase, then confirm that the entitlement is created only after payment confirmation.

**Acceptance Scenarios**:

1. **Given** an active card-paid auto-renewable credit entitlement with purchase-specific Stripe-side renewal payment authorization, **When** its validity/renewal boundary arrives, **Then** the system calculates the renewed entitlement quantity, price, currency, tax, billing mode, and validity period from the renewed purchase and attempts one automatic charge.
2. **Given** the renewal charge succeeds, **When** payment confirmation is received, **Then** exactly one renewed entitlement purchase and its credit ledger allocation are created, with the renewed validity period and quantity recorded in authoritative history.
3. **Given** the charge fails or requires customer authentication, **When** the failure is handled, **Then** no renewed credits are granted, the current entitlement is not falsely marked as renewed, and a fallback checkout action is made available.

### User Story 3 - Manage payment consent and recovery (Priority: P1)

As a customer or authorized administrator, I want to see whether the purchase-specific renewal payment authorization is ready, understand failures, and complete a fallback payment or renewal-authorization flow when needed.

**Why this priority**: Automatic charging must be transparent, recoverable, and safe for purchases that do not yet have a purchase-specific renewal authorization.

**Independent Test**: Exercise ready, missing-authorization, expired-card, authentication-required, insufficient-funds, disconnected-account, and generic-failure states and verify that each state exposes an accurate status, next action, and non-duplicating recovery path.

**Acceptance Scenarios**:

1. **Given** an auto-renewable purchase without a valid purchase-specific Stripe-side renewal payment authorization and consent, **When** the customer views the purchase, **Then** the purchase shows automatic payment as unavailable and provides a new purchase authorization or fallback checkout action; the card stored on the customer profile is not offered for this purpose.
2. **Given** an automatic renewal requires customer authentication, **When** the customer follows the provided action, **Then** the customer can complete authentication or hosted checkout and the renewal is finalized only after confirmed payment.
3. **Given** a fallback checkout payment succeeds, **When** the payment outcome is processed, **Then** the pending renewal is reconciled idempotently and the customer is not charged again by a retry or webhook replay.

### User Story 4 - Review authoritative renewal and payment history (Priority: P2)

As a customer support administrator or customer, I want renewal, payment, failure, fallback, cancellation, expiry, and refund events to appear in the purchase history so that the displayed timeline explains what actually happened.

**Why this priority**: Payment outcomes affect entitlement and service access and must be auditable independently of mutable current-state fields.

**Independent Test**: Process a successful renewal, a failed attempt, a fallback success, a cancellation, an expiry, and a refund, then verify the history contains the persisted events in order and does not infer events solely from aggregate timestamps or statuses.

**Acceptance Scenarios**:

1. **Given** a persisted renewal/payment lifecycle event, **When** a customer or authorized administrator opens purchase history, **Then** the event is displayed with its occurrence time, purchase type, amount/currency when applicable, outcome, and available next action.
2. **Given** an aggregate status changes without a corresponding persisted event, **When** history is rendered, **Then** the UI does not fabricate a lifecycle event.

### Edge Cases

- The purchase-specific Stripe-side renewal payment authorization is revoked, detached, expired, unusable, or no longer associated with the purchase between renewal scheduling and charging.
- The card is expired, declined, has insufficient funds, or is invalid for the renewed currency or destination-charge configuration.
- Stripe returns `requires_action`, an asynchronous outcome, an unknown PaymentIntent state, or a webhook after the renewal workflow timed out.
- A renewal workflow, retry, fallback checkout, or webhook handler runs concurrently or is replayed after a prior success.
- The renewed offer is no longer available, the current price/currency/tax changed, the product no longer supports AutoRenew, or no matching current pricing exists.
- A reservation cannot be materialized for the renewed availability/booking rules, or its resource assignment changes after payment.
- A credit entitlement has zero quantity, a changed validity period, an expired prior entitlement, an existing partial allocation, or an invalid credit ledger transition.
- AutoRenew is disabled, the purchase is cancelled immediately, cancellation is scheduled at period end, or the purchase expires while payment is pending.
- A refund is requested before payment confirmation, after a partial renewal, after a fallback payment, or for a high-level subscription that generated multiple child bookings.
- A notification cannot be delivered; the payment and renewal state must remain recoverable without relying on notification success.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST define one shared automatic-payment model for every marketplace purchase that supports AutoRenew and is paid by card, including reservation offers/subscriptions, credit-entitlement purchases, and future purchase types that declare AutoRenew support.
- **FR-002**: AutoRenew MUST remain the sole control for whether the marketplace purchase renews; membership term/PurchaseCadence MUST remain the contractual renewal term and MUST NOT be interpreted as booking frequency.
- **FR-003**: The initial purchase checkout or an explicit renewal-authorization checkout MUST collect and persist clear consent for future off-session charges for that specific marketplace purchase or subscription, including the terms presented at consent time. Consent MUST be tied to the purchase and merchant context, support explicit revocation, and MUST NOT authorize charging unrelated purchases. The system MUST NOT attempt an off-session charge without valid purchase-specific consent.
- **FR-004**: Each eligible purchase MUST have its own purchase-specific Stripe-side renewal payment authorization, created or retained by the purchase checkout or explicit renewal-authorization flow and linked to that purchase. This authorization MUST be distinct from the customer-profile card/payment-method feature; the renewal flow MUST NOT read, select, replace, or fall back to a card stored on the customer profile.
- **FR-005**: When an eligible renewal is due, the system MUST calculate the renewal from the renewed purchase and current pricing, including amount, currency, tax, billing mode, membership term, entitlement quantity/credits, and any purchase-type-specific attributes. If the amount or tax differs from the original purchase, the system MUST require customer confirmation through fallback checkout rather than charging off-session.
- **FR-006**: The system MUST attempt the eligible renewal with an off-session Stripe PaymentIntent using the purchase-specific Stripe-side renewal payment authorization and preserve existing Stripe Connect destination-charge behavior where applicable.
- **FR-007**: The system MUST not mark a renewal, reservation cycle, entitlement purchase, or credit allocation as paid until Stripe confirms the payment outcome.
- **FR-008**: Reservation renewal MUST create or advance the renewed reservation subscription/cycle only according to the renewed offer, membership term, availability, resource requirements, and booking rules. Reservation availability and booking rules MUST remain separate from payment renewal.
- **FR-009**: Credit-entitlement renewal MUST keep credit quantity and credit-validity period separate from reservation booking cadence, and MUST create the renewed entitlement and credit ledger allocation only after confirmed payment.
- **FR-010**: Each renewal attempt MUST have a stable idempotency identity derived from the purchase/subscription, renewal cycle, and payment attempt. Repeated workflow execution, retry, checkout completion, or webhook delivery MUST resolve to the same renewal outcome and MUST NOT create a second charge or second entitlement allocation.
- **FR-011**: The system MUST protect the renewal decision and payment state against concurrency between renewal workflows, payment workflows, retry handlers, manual recovery, and webhook processing, with one authoritative transition per renewal attempt.
- **FR-012**: The system MUST classify Stripe outcomes including success, failure, cancellation, asynchronous processing, authentication required, and unknown/unusable states, and MUST preserve the raw provider identifiers needed for reconciliation without exposing sensitive card data.
- **FR-013**: When off-session payment cannot complete because of missing purchase-specific consent/authorization, expired or detached purchase credential, insufficient funds, disconnected Stripe purchase/account association, authentication requirement, provider failure, or another recoverable condition, the system MUST retain the renewal as unpaid or payment-pending, record the reason, and generate a fallback hosted checkout/payment link.
- **FR-014**: Fallback checkout MUST remain available for every eligible renewal, including every renewal whose amount or tax differs from the original purchase. Each link MUST remain valid for three days, represent the specific renewal amount/currency/tax and purchase being recovered, and customer notification MUST include the outcome, expiry/retry guidance, and fallback action without claiming the renewal is paid. Completing fallback checkout MAY create or refresh the purchase-specific Stripe-side renewal authorization only for that purchase and only after the customer explicitly authorizes future off-session charging.
- **FR-015**: Automatic-payment retry behavior MUST be bounded to three automatic attempts over three days, MUST not retry non-retryable authentication or card-state failures, and MUST then converge to a failed/manual-recovery or cancelled/expired state without duplicate charging.
- **FR-016**: 3DS/SCA and other customer authentication requirements MUST be honored. An off-session attempt that requires action MUST transition to an action-required/pending state and provide a customer-authenticated recovery path; it MUST NOT be treated as successful merely because a PaymentIntent was created.
- **FR-017**: Stripe webhooks for PaymentIntent success, failure, cancellation, processing, and other asynchronous outcomes MUST reconcile the corresponding renewal attempt idempotently, even when delivered out of order or after a workflow timeout.
- **FR-018**: Renewal and entitlement state transitions MUST distinguish at least scheduled, payment-pending, action-required, paid/succeeded, failed, fallback-required, manually-recovered, cancelled, and expired. A pending or failed renewal MUST NOT grant reservation access or credits prematurely.
- **FR-019**: Disabling AutoRenew MUST prevent future renewal attempts while preserving completed cycles and payment history. Immediate cancellation and cancel-at-period-end MUST have distinct behavior and must not be inferred from pre-mutated aggregate state.
- **FR-020**: Refund and cancellation boundaries MUST remain separate. Reservation cancellation MUST stop future reservation service and use booking-owned refund eligibility; credit-entitlement cancellation MUST stop or invalidate future entitlement use according to its policy. A refund MUST be based on confirmed payment and the applicable purchase policy, not provider invoice state alone.
- **FR-021**: A high-level reservation subscription cancellation MUST not create separate customer-facing refunds for every generated child booking. A credit-entitlement renewal refund MUST identify the renewed purchase and its credit allocation; only unused credits MAY be refunded according to the applicable cancellation policy, and already-used credits MUST NOT be silently treated as unused.
- **FR-022**: Customer-facing and admin-facing API/UI surfaces MUST show purchase-specific renewal-authorization readiness, consent status, automatic-payment status, last attempt/outcome, failure reason, next retry or expiry time, fallback checkout action, and manual recovery action where authorized. These surfaces MUST NOT represent the customer-profile card/payment-method status as marketplace renewal readiness.
- **FR-023**: GraphQL/Relay mutation payloads and queries MUST return the stable purchase/renewal identifiers and fields required to update rendered state without browser reloads; connection or related-record changes MUST use declarative updates or targeted refetches.
- **FR-024**: Audit/history views MUST be driven by authoritative persisted payment, renewal, entitlement, cancellation, expiry, refund, and fallback events. Mutable aggregate fields MAY summarize current state but MUST NOT be used to fabricate historical events.
- **FR-025**: Existing auto-renewable purchases without a purchase-specific Stripe-side renewal payment authorization or valid purchase-specific off-session consent MUST continue to renew through the existing fallback checkout path, or transition to an explicitly recoverable payment-required state; they MUST NOT be silently charged and MUST NOT become eligible merely because the customer has a card stored on their profile.
- **FR-026**: The migration and rollout MUST support phased enablement, safe backfill or absence of consent, feature measurement, rollback of automatic attempts without deleting history, and explicit handling of in-flight renewal attempts.
- **FR-027**: The system MUST emit structured, correlated logs, durable audit events, metrics, and operational recovery signals for renewal scheduling, payment attempts, provider outcomes, retries, fallback generation, notification, webhook reconciliation, cancellation, expiry, refund, and manual intervention.
- **FR-028**: The implementation MUST preserve the distinction between the Stripe catalog and the purchase-specific renewal payment authorization. Stripe Product/Price records MAY identify a stable marketplace offer and exact catalog price, but they MUST NOT be confused with, or replace, the purchase-specific authorization used for future off-session charging.
- **FR-029**: When the renewed amount and tax exactly match a valid current Stripe catalog Price for the renewed purchase, the renewal checkout and payment records SHOULD reference that Product/Price. When the renewed amount, tax, billing calculation, destination-charge presentation, or other commercial value is calculated dynamically, the renewal MUST use the calculated amount/currency on the payment flow and MUST NOT create a new catalog Product/Price solely for that renewal.
- **FR-030**: The implementation MUST NOT introduce Stripe Billing subscriptions merely to obtain recurring card charging unless planning proves that Stripe-managed subscription schedules can represent AutoRenew, membership term, current pricing, reservation materialization, entitlement quantity/validity, tax, billing mode, cancellation, refund, and Connect behavior without making the domain’s authoritative lifecycle ambiguous.
- **FR-031**: The purchase-specific Stripe-side renewal authorization MAY be represented by Stripe primitives that technically reference a Stripe Customer and PaymentMethod because Stripe requires a customer-associated reusable credential for many off-session flows, but it MUST remain isolated from the application’s customer-profile payment-method record, UI, selection, replacement, and eligibility rules.

### Stripe Integration Research and Decision

Repository findings and Stripe guidance establish the following baseline for planning:

- Skedular currently creates Stripe Products and Prices through `StripeProductPricingService.UpsertProductPricingAsync`. Standard reservation and entitlement Checkout Sessions use the persisted Stripe Price ID for the marketplace pricing option.
- The current in-arrears recurring reservation checkout builds an inline `price_data` line item from the calculated invoice draft. Host destination-charge checkout can also replace catalog Price references with inline price data. Therefore, the system does not currently use one uniform Stripe catalog strategy for every payment path.
- Stripe documents Products as the catalog description of what is sold and Prices as the amount/currency/recurrence definition. Checkout can use a Price ID, or inline `price_data` when the application owns the catalog or the amount is transaction-specific. Inline Prices are not reusable catalog entries.
- Stripe documents Checkout `payment_intent_data.setup_future_usage=off_session` for saving the payment method used in a payment for later off-session charges, and Checkout setup mode/SetupIntents when collecting authorization without an initial charge. Stripe also requires explicit customer consent and compliance with applicable laws and network rules.
- Stripe documents that later off-session charging is performed with a PaymentIntent using the saved Stripe-side credential. This supports the feature’s purchase-specific authorization model, but the implementation must keep the resulting Stripe Customer/PaymentMethod identifiers scoped to the marketplace purchase rather than exposing them through the existing customer-profile payment-method feature.
- Stripe documents destination charges as charges created on the platform and transferred to the connected account, with platform responsibility for Stripe fees, refunds, and chargebacks. The renewal design must preserve the existing charge type and explicitly validate where the purchase-specific credential is stored and where the off-session PaymentIntent is created for each Connect path.

**Recommended design direction**: use the existing Stripe Product/Price mapping as a catalog reference when it exactly represents the renewed purchase; use a purchase-specific Checkout authorization during the initial/fallback payment to obtain the Stripe-side reusable credential; and create one idempotent off-session PaymentIntent per renewal cycle for the calculated renewal amount. Do not create Stripe catalog objects per renewal and do not delegate the domain renewal lifecycle to an opaque Stripe Billing subscription by default.

**Research boundary for planning**: confirm the exact Stripe account/customer/payment-method ownership for direct versus destination charges, confirm whether the current Checkout configuration captures `setup_future_usage=off_session` or an equivalent explicit authorization, and confirm the tax behavior when a persisted Price is replaced by dynamic amount calculation. These are implementation-validation tasks, not permission to silently alter the existing Connect or tax model.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Feature MUST emit structured logs for the start and completion of each renewal evaluation and automatic payment attempt.
- **LOG-002**: Feature MUST emit structured logs for consent eligibility, purchase-specific renewal-authorization selection, renewal calculation, idempotency decision, state transition, retry classification, fallback generation, and manual recovery.
- **LOG-003**: Feature MUST emit actionable warning/error logs for provider failures, webhook reconciliation conflicts, duplicate/concurrent attempts, missing associations, notification failures, and unrecoverable state mismatches.
- **LOG-004**: Feature logs MUST include correlation context such as purchase/subscription ID, renewal-cycle ID, payment-attempt ID, PaymentIntent ID when available, workflow ID, and event ID, while excluding full card numbers, security codes, client secrets, and other sensitive payment data.
- **LOG-005**: Operators MUST be able to identify renewals stuck in payment-pending, action-required, fallback-required, failed, or reconciliation-conflict states and safely replay or manually recover them without creating a duplicate charge.

### Key Entities

- **Auto-renewable marketplace purchase**: A reservation subscription, credit-entitlement purchase, or future purchase type whose offer supports AutoRenew; owns the renewal policy and purchase-specific renewal data.
- **Renewal cycle**: The immutable business instance representing one attempted next term, including the source purchase, calculated commercial values, lifecycle state, and links to payment and fallback recovery.
- **Purchase-specific renewal payment authorization**: The purchase-scoped Stripe-side payment authorization and credential created or retained by one marketplace purchase’s checkout or renewal-authorization flow, including its consent, merchant context, usability, and revocation state. It is intentionally separate from the card/payment method stored on the customer profile.
- **Renewal payment attempt**: The idempotent record of one automatic or fallback payment attempt, including amount/currency, provider identifiers, outcome, authentication requirement, retry data, and timestamps.
- **Reservation renewal**: A renewed reservation subscription/cycle whose access and resource bookings depend on confirmed payment and renewed booking rules.
- **Credit-entitlement renewal**: A renewed entitlement purchase whose quantity, validity period, and credit ledger allocation depend on confirmed payment.
- **Marketplace lifecycle event**: An append-only authoritative record for renewal, payment, fallback, entitlement, cancellation, expiry, refund, and manual-recovery transitions used by audit/history UI.
- **Fallback checkout recovery**: A hosted checkout/payment link tied to one renewal cycle and its unpaid payment attempt, with expiry and notification state.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: At least 99.9% of eligible renewal cycles have no more than one successful charge or one confirmed fallback payment per cycle, measured by reconciliation of renewal-cycle and provider payment identifiers.
- **SC-002**: 95% of eligible renewals with valid purchase-specific consent and a usable purchase-specific Stripe-side renewal payment authorization reach a confirmed payment outcome without customer interaction within 10 minutes of the renewal attempt starting.
- **SC-003**: 100% of successful automatic and fallback payments result in exactly one corresponding paid reservation cycle or credit-entitlement renewal, and 0% of unpaid renewals grant new credits or paid reservation access.
- **SC-004**: 100% of automatic-payment failures expose a customer-visible reason category and an actionable fallback or purchase-specific renewal-authorization recovery path within 5 minutes of classification.
- **SC-005**: 100% of renewal/payment/refund history entries displayed to customers and administrators map to a persisted authoritative event, with no history entry inferred solely from mutable aggregate fields.
- **SC-006**: Existing auto-renewable purchases without valid reusable payment authorization continue to have a usable checkout-link recovery path, with no silent off-session charges.
- **SC-007**: Operators can locate and safely recover 99% of payment-pending, action-required, fallback-required, and webhook-reconciliation exceptions using the admin surface and correlation identifiers, without issuing a duplicate charge.
- **SC-008**: In usability testing, at least 90% of customers can identify whether automatic renewal is ready and complete the appropriate payment-method update, authentication, or fallback checkout action on the first attempt.

## Unresolved Product and Payment-Policy Decisions

These decisions must be resolved during clarification/planning before implementation; this specification uses the stated safe default where one is available.

- **Connect failure ownership**: Confirm the operator/customer behavior when a destination-charge account is disconnected or cannot receive the renewal. Safe default: do not charge or grant renewal, preserve the failure, and expose an administrator recovery path.

## Assumptions

- Card-paid means the marketplace purchase has a purchase-specific Stripe-side renewal payment authorization captured or retained during its checkout or explicit renewal-authorization flow; non-card payment methods are outside this feature unless they later satisfy the same purchase-specific off-session authorization contract.
- The existing customer-profile card/payment-method and SetupIntent feature is explicitly outside this renewal-payment model. Marketplace renewal eligibility MUST NOT depend on, read, select, or fall back to the card stored on the customer profile.
- The renewed purchase is evaluated against current eligible pricing and offer state at renewal time, while the historical renewal calculation and payment amount remain immutable after the attempt begins.
- Reservation resource availability, booking frequency, booking rules, membership term, credit quantity, and credit validity period are separate concepts and are evaluated by their owning purchase behavior.
- Stripe provider identifiers, PaymentIntent status, and webhook delivery are external facts, but local persisted payment and lifecycle events are authoritative for customer/admin history and recovery state.
- Existing hosted checkout links remain supported for missing consent, missing/invalid purchase-specific renewal authorization, authentication, provider failures, and explicit customer confirmation.
- Existing purchase records are not assumed to have reusable-payment consent. No historical consent is backfilled from a prior hosted checkout completion alone.
- Authorization, privacy, customer notification preferences, and organization/admin permissions reuse existing platform policies unless a payment-policy decision above changes them.
- The feature package is design-only. Implementation must wait until spec.md, plan.md, data-model.md, contracts/, quickstart.md, and dependency-ordered tasks.md are complete and consistent.

## Dependencies

- Existing marketplace purchase checkout and Stripe-side purchase payment-authorization lifecycle; the customer-profile card/payment-method feature is not a dependency for renewal eligibility.
- Existing marketplace reservation subscription renewal and credit-entitlement purchase/expiry/redemption services.
- Existing Stripe Checkout Session and PaymentIntent integration, Connect destination-charge behavior, and webhook subscriber.
- Existing Temporal renewal/payment workflows and centralized workflow-ID conventions.
- Existing append-only MarketplacePurchaseHistory event model and purchase/refund UI/API surfaces.
- Existing GraphQL/Fusion schema, Relay generated artifacts, authorization, notification, and organization/admin surfaces.
- Forward-only persistence migrations and generated-contract/artifact workflows required by repository governance.
