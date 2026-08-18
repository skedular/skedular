# Feature Specification: Credit-Based Booking Entitlements

**Feature Branch**: `041-credit-based-entitlements`
**Created**: 2026-08-09
**Status**: Draft
**Input**: User description: "Add an entitlement-based fulfillment model where customers purchase fixed usage credits and later use them to create eligible bookings without pre-reserving resources."

## Clarifications

### Session 2026-08-09

- Q: When an entitlement reaches its end date, should the refund option refund only unused credits, the entire purchase, or let the customer choose? → A: Refund only unused credits, and allow the product pricing offering to define whether that refund is permitted.
- Q: When product pricing allows unused-credit refunds, should the refund happen automatically at the entitlement end date or require a customer/admin request? → A: Initiate the refund automatically at the end date; Stripe may settle automatically, bank-transfer refunds require manual settlement, and Xero currently creates a credit note without automatic settlement.
- Q: Should the same unused-credit refund eligibility apply when a customer cancels an entitlement before its configured end date? → A: Yes; use the same product-pricing refund eligibility for expiry and early cancellation, subject to the cancellation deadline.
- Q: How should the refund amount for unused credits be calculated when the purchase includes discounts, taxes, or fees? → A: Pro-rate the net credit purchase amount across the total credits and refund the unused share, excluding non-refundable fees and taxes unless the existing refund policy includes them.
- Q: If payment is not confirmed when an entitlement expires or is canceled, should the system create a refund, or close the entitlement without refunding? → A: Do not create a refund without confirmed payment; close the entitlement and record unused credits as expired/forfeited while preserving the payment workflow outcome.
- Q: Should starting a credit purchase create a booking before payment, or only when a credit is later used? → A: A credit purchase is a standalone purchase/order. It must never create a booking, schedule, resource allocation, reservation, or booking quota usage; a booking is created only when the customer later spends a credit.
- Q: Which payment methods should a standalone credit purchase support? → A: Support Stripe card payments and manual bank-transfer invoices; keep Xero as an accounting projection/manual settlement path rather than a separate customer payment method.
- Q: What should the customer receive immediately after starting a standalone credit purchase? → A: Return the purchase status and the applicable payment action: a Stripe checkout URL for card payments or bank-transfer invoice/instructions for manual payment.
- Q: What should happen when a standalone credit purchase reaches its payment deadline without confirmed payment? → A: Mark the purchase expired, stop payment processing, retain the audit record, and grant no credits.
- Q: How should repeated requests for the same credit purchase be identified? → A: Follow the existing non-entitlement purchase/payment retry and deduplication behavior; do not introduce a new client idempotency-key requirement for entitlement purchases.
- Q: Should token-based entitlements support renewal? → A: Yes. They follow the existing reservation-based entitlement renewal pattern: when the current token validity cycle ends, an auto-renewing entitlement starts the next cycle using the configured pricing and payment behavior; a non-renewing entitlement ends without creating another cycle.
- Q: Who may create and manage bookings using purchased tokens? → A: The customer may create, modify, and cancel bookings. Authorized Skedular Spaces and Skedular Host administrators/owners may perform the same actions on the customer’s behalf, including selecting and using an eligible token entitlement.
- Q: If an auto-renewal payment is pending or fails at the token cycle boundary, should the current token entitlement expire immediately, or remain temporarily usable during the payment-retry period? → A: The current cycle expires at its configured end date while renewal payment retries; no new tokens are granted until payment is confirmed.
- Q: When a token entitlement renews, should it use the current product pricing configuration at renewal time, or the original pricing snapshot from the first purchase? → A: Renewal uses the current active product pricing configuration; historical purchase and entitlement cycles preserve their original snapshots.
- Q: Should Skedular Spaces and Host owners/administrators be allowed to use a customer’s token whenever their existing organization role permits it, without an additional customer approval step? → A: Existing authorized owner/admin roles may act on behalf of customers; every action records the operator and customer.
- Q: If the current pricing no longer supports token fulfillment or auto-renewal when renewal is due, should renewal fail without creating a new token cycle? → A: Renewal fails safely; no new token cycle is created, and the expired cycle and history are preserved.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Purchase Future-Use Credits (Priority: P1)

As a customer, I want to purchase a fixed quantity of usage credits so I can choose eligible booking dates later.

**Why this priority**: This is the foundation of the feature and creates value without requiring customers to commit to dates at purchase time.

**Independent Test**: Purchase an entitlement offering with a successful payment and verify the entitlement, balance, validity, and absence of bookings or resource reservations.

**Acceptance Scenarios**:

1. **Given** an offering grants four credits valid for 30 days, **When** a customer starts checkout, **Then** one pending purchase is created and no booking, schedule, resource allocation, reservation, or booking quota usage is created.
2. **Given** a pending purchase with a successful payment, **When** payment confirmation is processed, **Then** one entitlement is created with four available credits and activation and expiry timestamps.
3. **Given** payment processing or webhook delivery is retried, **When** the same purchase is processed again, **Then** the customer has only one entitlement and one grant for that purchase.
4. **Given** an administrator configures an entitlement offering, **When** it is saved, **Then** quantity, validity, weekday restrictions, product/resource scope, cancellation policy, expiry policy, and supported payment methods are visible and reviewable.

### User Story 2 - Book Using Credits (Priority: P1)

As a customer, I want to use an available credit to reserve an eligible resource on a date I choose.

**Why this priority**: This is the primary fulfillment journey and converts purchased entitlement into actual service.

**Independent Test**: Start with an active entitlement and available resource, create a qualifying booking, and verify one credit is consumed exactly once.

**Acceptance Scenarios**:

1. **Given** four active credits and an available eligible resource, **When** the customer books a qualifying date, **Then** the booking is created, linked to the entitlement and credit transaction, and three credits remain available.
2. **Given** the date is outside validity, has a forbidden weekday, the product is outside scope, no eligible resource is available, or no credit remains, **When** the customer attempts to book, **Then** the booking is rejected and no credit is consumed.
3. **Given** two concurrent requests compete for the last available credit, **When** both are submitted, **Then** at most one booking succeeds and the credit is consumed at most once.
4. **Given** multiple eligible entitlements exist, **When** a booking is created, **Then** the entitlement with the earliest expiry is selected deterministically.

### User Story 3 - Manage Credit-Funded Bookings (Priority: P1)

As a customer, I want to view, modify, and cancel bookings made with credits while understanding whether my credit is restored.

**Why this priority**: Customers need control over the reservations created from their purchase and clear consequences for changes.

**Independent Test**: Create a credit-funded booking, modify it, cancel it under different policy windows, and verify reservation and ledger outcomes.

**Acceptance Scenarios**:

1. **Given** a future credit-funded booking, **When** the customer moves it to another qualifying available date, **Then** the reservation moves atomically and exactly one credit remains consumed.
2. **Given** a future booking within the restoration window, **When** it is canceled, **Then** the resource is released and one credit is restored with one release ledger entry.
3. **Given** a completed booking, a late cancellation, or a no-show according to policy, **When** the booking is canceled or closed, **Then** the credit is not restored and the ledger records the forfeiture outcome.
4. **Given** a cancellation or restoration request is retried, **When** it is processed again, **Then** the outcome and balance do not change a second time.

### User Story 4 - Renew Token Entitlements (Priority: P1)

As a customer, I want a token-based entitlement to renew at the end of its validity cycle so I can continue using the same offering without purchasing it again manually.

**Why this priority**: Renewal is part of the existing reservation-based entitlement payment pattern and is required for recurring token offerings.

**Independent Test**: Configure an auto-renewing token entitlement, advance it to the cycle boundary, and verify the next entitlement cycle is created only after the existing payment flow confirms payment; verify a non-renewing entitlement creates no next cycle.

**Acceptance Scenarios**:

1. **Given** a token entitlement is configured for auto-renewal, **When** its current validity cycle ends, **Then** the renewal payment flow starts using the same supported payment method and pricing rules as reservation-based renewal.
2. **Given** renewal payment is confirmed, **When** renewal processing completes, **Then** exactly one new entitlement cycle is granted with the configured token quantity, validity, restrictions, and audit linkage to the prior cycle.
3. **Given** renewal payment is pending, rejected, or fails, **When** the cycle boundary is processed, **Then** no duplicate entitlement cycle is granted and the existing renewal/payment retry behavior remains responsible for retry or terminal state.
4. **Given** auto-renew is disabled, **When** the current token cycle ends, **Then** no payment or entitlement renewal is created.

### User Story 5 - Book and Manage Tokens on Behalf of a Customer (Priority: P1)

As an authorized Skedular Spaces or Skedular Host administrator or owner, I want to create, modify, and cancel a customer’s token-funded booking when the customer cannot do it themselves.

**Why this priority**: Operators must be able to provide the same service to customers through support or front-desk workflows without bypassing entitlement rules.

**Independent Test**: An authorized operator selects a customer’s eligible token entitlement, creates a booking, changes its date/time or resource, and cancels it; an unauthorized operator cannot perform those actions.

**Acceptance Scenarios**:

1. **Given** an authorized operator and an eligible customer token, **When** the operator creates a booking, **Then** the booking uses one token and records the operator as actor/source without weakening normal eligibility or availability validation.
2. **Given** a customer or operator changes the booking date, time, or attached resource, **When** the new selection is validated, **Then** the booking is modified atomically and the token remains consumed exactly once.
3. **Given** a customer or authorized operator cancels the booking, **When** the applicable cancellation policy is evaluated, **Then** the resource and token are restored or forfeited according to the same customer-facing rules.
4. **Given** an unauthorized user attempts to create, modify, or cancel on behalf of a customer, **When** the request is evaluated, **Then** it is denied without changing booking, resource, or token state.

### User Story 6 - Monitor Entitlements and Expiry (Priority: P2)

As a customer or administrator, I want to see balances, restrictions, expiry, and history so I can make informed booking decisions and support customers.

**Why this priority**: Transparency reduces failed booking attempts and makes credit accounting auditable.

**Independent Test**: Inspect a customer entitlement and its history before and after consumption, restoration, forfeiture, and expiry.

**Acceptance Scenarios**:

1. **Given** an active entitlement, **When** the customer views it, **Then** granted, consumed, remaining, expired, and forfeited quantities, expiry date, restrictions, and linked bookings are shown.
2. **Given** an entitlement reaches its end date with two unused credits, **When** end-date processing runs, **Then** the credits become unusable and the configured product-pricing outcome is applied: two credits are recorded as expired/forfeited without carry-over, or an eligible confirmed-payment purchase creates one prorated unused-credit refund through the existing refund workflow.
3. **Given** an administrator inspects an entitlement, **When** ledger history is opened, **Then** each grant, consumption, release, forfeiture, expiry, or adjustment includes entitlement, quantity, booking when applicable, time, actor/source, reference key, and relevant metadata.
4. **Given** an organization or customer is not authorized for an entitlement, **When** they request its details or attempt to use it, **Then** access is denied and no balance or booking state is changed.

### Edge Cases

- Activation and expiry boundaries use the applicable location timezone and precise timestamps; a booking exactly at the valid boundary is accepted according to the inclusive/exclusive policy documented for the offering.
- An unavailable date, invalid duration, forbidden weekday, mismatched product/resource, or insufficient balance must leave both booking and credit state unchanged.
- Entitlement cancellation blocks future credit use, records the configured expiry/forfeiture or eligible unused-credit refund outcome, and does not alter existing bookings.
- An auto-renewal payment that is pending or fails does not extend the current entitlement; the current cycle expires at its configured end date, renewal retries continue according to the existing payment workflow, and no new entitlement cycle is granted before confirmed payment.
- Renewal re-evaluates the current active product pricing configuration, while historical purchase and entitlement-cycle snapshots remain unchanged.
- Operator-created, modified, and canceled token-funded bookings use existing organization-role authorization and do not require an additional customer approval step; each action records both the acting operator and the customer.
- If the current pricing no longer supports token fulfillment or auto-renewal at renewal time, renewal fails safely without falling back to reservation pricing or a stale snapshot; the expired cycle and its history remain preserved.
- An expiry process, purchase retry, booking retry, modification retry, cancellation retry, or credit adjustment retry must be idempotent.
- Historical bookings and ledger entries remain intact after expiry or entitlement cancellation.
- A pending, rejected, canceled, or expired entitlement purchase never creates a booking, schedule, resource allocation, reservation, or booking quota usage.
- Existing reservation-based offerings, ad-hoc bookings, recurring bookings, and subscription behavior remain unchanged.

### Entitlement Lifecycle Outcomes

| Payment state | Pricing permits unused-credit refund | End-date outcome | Financial settlement state |
|---|---|---|---|
| Confirmed | No | Close entitlement and record unused credits as `FORFEITED` | No refund created |
| Confirmed | Yes | Close entitlement and record unused credits as `EXPIRED` | Stripe refund may settle automatically; bank transfer remains manual; Xero creates a credit note pending downstream settlement |
| Unconfirmed, pending, rejected, or expired | No | Close entitlement and record unused credits as `FORFEITED` | No refund created; preserve the payment workflow state |
| Unconfirmed, pending, rejected, or expired | Yes | Close entitlement and record unused credits as `FORFEITED` | No refund created; preserve the payment workflow state |

In every row, existing bookings and historical ledger entries remain intact, and repeated expiry or cancellation processing produces no duplicate lifecycle entry or refund aggregate.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Pricing offerings MUST declare a fulfillment type of `RESERVATION` or `ENTITLEMENT`; existing reservation behavior MUST remain the default and unchanged.
- **FR-002**: An entitlement offering MUST configure a fixed credit quantity, a fixed validity duration, activation rules, expiry rules, allowed weekdays, applicable product/resource scope, booking cancellation policy, and supported payment methods. Each credit represents one qualifying booking.
- **FR-003**: Starting an entitlement purchase MUST create a standalone pending purchase/order that records the customer, organization, pricing snapshot, amount, currency, payment method, payment state, checkout/invoice context, and expiry. Starting or retrying payment MUST create no booking, schedule, resource allocation, reservation, or booking quota usage.
- **FR-003b**: Standalone entitlement purchases MUST support automatic Stripe card checkout and manual bank-transfer invoicing. Xero MUST remain an accounting projection/manual settlement path and MUST NOT be treated as a separate customer payment method for the initial release.
- **FR-003c**: The purchase response MUST return the pending purchase status and the applicable payment action: a Stripe checkout URL for card payments or bank-transfer invoice/payment instructions for manual settlement.
- **FR-003d**: An unconfirmed standalone purchase MUST expire at its payment deadline, stop its payment/invoice workflow, retain its audit record, and grant no credits.
- **FR-003e**: Standalone entitlement purchases MUST reuse the existing non-entitlement purchase/payment retry and deduplication behavior. The initial release MUST NOT require a new client-provided idempotency-key contract.
- **FR-003a**: A confirmed entitlement purchase MUST create exactly one customer entitlement, calculate activation and expiry using the applicable location timezone, and grant the configured quantity. A booking may be created only by a later qualifying credit-use request.
- **FR-004**: Purchase retries and payment/webhook retries MUST be idempotent and MUST NOT duplicate entitlements or grants.
- **FR-005**: The system MUST represent credits as fungible quantities and maintain an auditable ledger with grant, consumption, release, forfeiture, expiry, and adjustment transaction types.
- **FR-006**: Every ledger entry MUST record entitlement, quantity, transaction type, timestamp, actor or source, idempotency/reference key, relevant metadata, and booking when applicable.
- **FR-007**: A credit-funded booking MUST pass all existing booking validation and MUST verify customer authorization, active validity, date validity, weekday restrictions, product scope, duration/resource constraints, resource availability, and sufficient available credits.
- **FR-008**: Resource reservation and credit consumption MUST succeed or fail as one atomic operation; failed bookings MUST consume no credit.
- **FR-009**: Concurrent booking attempts MUST prevent double spending, and credit selection across eligible entitlements MUST be deterministic using earliest expiry first by default.
- **FR-010**: Credit-funded bookings MUST retain references to the entitlement and the consuming credit transaction and MUST expose those relationships to authorized users and administrators.
- **FR-011**: Eligible future cancellation MUST release the resource, restore exactly one credit, and record one release transaction; completed bookings, late cancellations, and no-shows MUST follow the configured forfeiture policy.
- **FR-012**: Booking cancellation, credit restoration, expiry, and administrative adjustment MUST be idempotent.
- **FR-013**: Modification of a credit-funded booking MUST validate the new date and availability, move the reservation atomically, and preserve exactly one consumed credit; failure MUST leave the original state unchanged.
- **FR-014**: On the configured entitlement end date, unused credits MUST become unusable, existing bookings and history MUST remain intact, and the product pricing offering MUST explicitly determine whether the unused balance is recorded as `EXPIRED`/forfeited or enters the existing refund workflow. If auto-renewal is enabled, renewal processing MUST be evaluated separately from expiry/refund processing and MUST not duplicate the ending cycle.
- **FR-014a**: When the product pricing permits unused-credit refunds, the system MUST initiate the refund automatically at the configured end date. Stripe refunds MAY settle automatically; bank-transfer refunds MUST enter manual settlement; and Xero-backed refunds MUST create the applicable credit note while remaining pending manual/downstream settlement until supported.
- **FR-014b**: When unused credits enter the refund workflow, the credit ledger MUST record an `EXPIRED` entry for the unused quantity with metadata linking to the refund. The existing refund aggregate remains the financial source of truth; no separate `REFUNDED` credit-ledger transaction type is introduced.
- **FR-015**: Entitlement cancellation MUST be separate from booking cancellation, block future use, and apply the configured unused-credit outcome: forfeiture/expiry or an eligible refund request. If refunding is enabled for the product pricing, only unused credits may be refunded and the amount MUST be calculated according to the pricing/refund policy. Existing bookings MUST remain intact unless the existing cancellation policy explicitly governs them.
- **FR-015a**: Early entitlement cancellation and natural entitlement expiry MUST use the same product-pricing refund eligibility, subject to the configured cancellation deadline; both paths MUST refund only the unused credit quantity when eligible.
- **FR-015d**: Entitlement refund eligibility and cancellation deadlines MUST reuse the existing product-pricing cancellation policy and refund rules; the initial release MUST NOT introduce a separate entitlement-specific deadline model.
- **FR-015b**: An eligible unused-credit refund MUST be calculated by prorating the net credit purchase amount across the total granted credits and applying the unused quantity. Non-refundable fees and taxes MUST be excluded unless the existing refund policy explicitly includes them.
- **FR-015c**: If payment is not confirmed when expiry or cancellation is processed, the system MUST NOT create a refund; it MUST close the entitlement, record unused credits as expired/forfeited, and preserve the pending, rejected, expired, or other payment workflow state.
- **FR-016**: Authorization MUST prevent cross-customer, cross-organization, unrelated-product, and unauthorized administrative credit use or adjustment. Authorized Skedular Spaces and Skedular Host administrators/owners may act on behalf of a customer, without an additional customer approval step, only within their organization and only through the same booking, resource, token, cancellation, and modification rules; every operator action MUST record the operator and customer.
- **FR-017**: Customers MUST be able to view active entitlements, balances, validity, restrictions, ledger outcomes, eligible availability, and bookings linked to consumed credits.
- **FR-018**: The customer experience MUST clearly explain whether a cancellation restores or forfeits a credit.
- **FR-019**: Administrators MUST be able to configure entitlement rules and inspect balances and complete ledger history.
- **FR-020**: Carry-over, top-ups, bonus credits, shared pools, transfers, promotional credits, variable credit costs, and multiple expiry lots MUST remain outside the initial release. Token auto-renewal is in scope and MUST reuse the existing reservation-based entitlement renewal and payment behavior. Refunds are limited to unused credits, only when enabled by the product pricing, and must use existing refund eligibility/workflow rules.
- **FR-022**: Token auto-renewal MUST honor the pricing renewal setting, use the existing Stripe or manual bank-transfer payment flow, preserve the configured token quantity/validity/restrictions for the new cycle, and remain idempotent across workflow, payment, and webhook retries.
- **FR-023**: When auto-renewal payment is pending or fails at the cycle boundary, the current entitlement MUST expire at its configured end date; payment retries MUST continue through the existing renewal workflow, and no new entitlement cycle or tokens MUST be granted until payment is confirmed.
- **FR-024**: A token renewal MUST re-evaluate the current active product pricing configuration for quantity, validity, restrictions, renewal setting, price, currency, and supported payment methods; historical purchase and entitlement-cycle snapshots MUST remain immutable.
- **FR-025**: If no compatible active token pricing with auto-renewal exists when renewal is due, the renewal MUST fail without creating a new entitlement cycle, falling back to reservation pricing, or mutating historical snapshots; the expired cycle and renewal failure state MUST remain auditable.
- **FR-026**: Customer, Skedular Spaces operator, and Skedular Host operator booking surfaces MUST support creating, modifying date/time/resource, and canceling token-funded bookings. Operator actions MUST record the acting user and preserve the same authorization, availability, cancellation, restoration, and forfeiture rules.
- **FR-021**: The feature MUST preserve existing product-tag matching, resource allocation, payment methods, ad-hoc booking, recurring booking, reservation-based subscription, and subscription cancellation behavior.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs for entitlement purchase, grant, booking consumption, restoration, forfeiture, expiry, cancellation, and modification start/completion.
- **LOG-002**: The feature MUST emit structured logs for eligibility decisions, credit selection, idempotency outcomes, and concurrency/atomicity branches.
- **LOG-003**: The feature MUST emit actionable warning/error logs for failed payment linkage, rejected bookings, failed restoration, expiry recovery, and administrative adjustment failures.
- **LOG-004**: Feature logs MUST include correlation and workflow/request context, entitlement and booking references where safe, and MUST avoid payment credentials and other sensitive data.

### Key Entities

- **Pricing Offering**: Commercial definition that specifies reservation or entitlement fulfillment and, for entitlements, credit quantity, validity, restrictions, scope, and policies.
- **Purchase/Order**: Standalone commercial transaction that records the customer’s credit purchase and payment outcome without representing a booking or reserving a resource.
- **Entitlement**: Customer-authorized right to future usage with activation, expiry, scope, restrictions, and lifecycle state.
- **Credit Ledger Entry**: Immutable auditable quantity transaction associated with an entitlement and optionally a booking.
- **Booking**: Actual resource reservation that may reference the entitlement and consuming ledger entry.
- **Entitlement Policy**: Rules governing cancellation restoration, forfeiture, no-show handling, expiry, and administrative adjustments.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In acceptance testing, 100% of started entitlement purchases create no booking, schedule, resource allocation, reservation, or booking quota usage; 100% of confirmed purchases create exactly one entitlement and grant the configured quantity.
- **SC-002**: In concurrency testing, 100% of attempts to spend a single remaining credit result in no more than one successful booking and no duplicate consumption.
- **SC-003**: At least 95% of valid credit-funded booking attempts complete successfully when an eligible resource is available, while invalid attempts consume zero credits.
- **SC-004**: Customers and authorized administrators can view entitlement balance, granted/consumed/remaining quantities, product and weekday restrictions, activation and expiry dates, linked bookings, ledger history, refund status/amount, and the associated purchase payment-action state; pending purchases show the correct Stripe checkout link or manual bank-transfer instructions, while confirmed purchases show the linked entitlement.
- **SC-005**: 100% of credit-affecting lifecycle actions produce a ledger entry containing the required audit fields and an idempotency/reference key.
- **SC-006**: In renewal acceptance testing, 100% of eligible auto-renewing token cycles create at most one next cycle after confirmed payment, while non-renewing or failed-payment cycles create no duplicate entitlement.
- **SC-007**: Customers and authorized Spaces/Host operators can create, modify date/time/resource, and cancel token-funded bookings through their respective UI surfaces, with identical token and cancellation outcomes.
- **SC-008**: Regression testing shows no change in expected outcomes for existing reservation-based offerings, ad-hoc bookings, recurring bookings, payment methods, or subscription cancellation behavior.

## Assumptions

- Customers and administrators use the existing authenticated product and organization boundaries.
- Credits are fungible within one entitlement and have a fixed cost of one credit per qualifying booking; variable credit costs are deferred.
- Entitlement validity is based on usage date and precise timestamps in the applicable location timezone.
- An entitlement offering explicitly chooses the unused-credit end-date outcome: expire/forfeit or enter the existing refund workflow. Product pricing separately defines whether unused-credit refunds are permitted.
- Refund eligibility and amount follow booking-owned refund policy and confirmed-payment requirements; refund is separate from entitlement cancellation and credit expiry.
- When permitted, the refund amount covers only the unused credit quantity and never credits already consumed, forfeited, or expired.
- Refund-routed unused credits are recorded as `EXPIRED` in the credit ledger with a refund reference; the refund aggregate remains the financial source of truth.
- Automatic initiation MUST NOT be represented as guaranteed settlement: payment-method-specific refund status, manual action, and downstream accounting state remain visible to authorized users.
- Confirmed payment is required before creating an unused-credit refund; unconfirmed payment follows entitlement closure and expiry/forfeiture instead.
- Existing payment, purchase, booking, resource availability, authorization, and notification capabilities are reused where their semantics match.
- Entitlement purchase payment and invoice records are distinct from booking payment and invoice records; only their shared payment-method and accounting rules are reused.
- Existing product-pricing cancellation policy and refund rules are reused for entitlement cancellation and end-date refund eligibility.
- The first release supports one entitlement lot per purchase cycle; automatic renewal creates a new cycle using the existing reservation-based renewal model, while multiple expiry lots within one cycle remain deferred.
- Customer-facing and operator-facing help documentation will be reviewed and updated during implementation if eligibility, expiry, or cancellation behavior is exposed publicly.
- The initial release includes the customer and operator-facing entitlement experience in both Skedular Spaces and Skedular Host; their product-specific terminology and navigation may differ, but entitlement, refund, and audit rules remain identical.

## Required Discovery Before Implementation

Before creating the implementation plan, inspect the existing codebase and identify product/product-tag, pricing, purchase/payment, subscription, booking, resource allocation, authorization, API/frontend, persistence/migration, workflow, event, and test surfaces. Do not assume the names in this specification match existing code. Produce a technical plan covering schema, domain, services, APIs, UI, payment behavior, allocation, cancellation, modification, expiry, authorization, migrations, reporting, tests, documentation, and backward-compatibility risks. Any unresolved business rule must be documented as an assumption or clarified before implementation.
