# Feature Specification: End-to-End Refund Reliability

**Feature Branch**: `037-refund-reliability`
**Created**: 2026-07-25
**Status**: Implemented

## Overview

Skedular has partial, inconsistent refund handling spread across booking, payment, and invoice workflows. The purpose of this work is to audit the entire current-state refund implementation, identify every gap and failure scenario, and deliver a production-grade refund system that is financially safe, idempotent, auditable, and reliable for Spaces and Host across Stripe, Xero, and bank transfer. Teams is audit-only: its marketplace payment and refund scope must be explicitly confirmed, while Teams SaaS-subscription billing remains out of scope.

This specification focuses on what customers, operators, and the Skedular platform need from a correct refund system, not on how it should be built.

## Clarifications

### Session 2026-07-25

- Q: What refund states should the formal state machine support? → A: Expanded state set with review gates — `Requested → UnderReview → Approved / Rejected → Processing → ProviderPending → Completed / Failed`, plus `ReconciliationRequired` as a side-state; shared across all payment methods (10 states).
- Q: How often should provider refund status be reconciled? → A: Hybrid — Stripe status driven by webhooks (near-real-time), Xero and bank-transfer refunds reconciled on a scheduled daily batch.
- Q: Should refunds be managed in a new dedicated domain or extend the existing booking domain? → A: Extend the existing booking domain — refund entities, services, and APIs are owned within the booking domain's existing persistence and API boundary.
- Q: Does the Skedular Teams SaaS subscription support refunds or credits when an organization cancels or downgrades? → A: Out of scope — Teams SaaS subscription billing is excluded from this feature entirely.
- Q: When payment succeeds but only some recurring bookings can be created, how is customer acceptance of the partial booking handled? → A: Customer must explicitly accept within 24 hours; if no response, a full refund is issued automatically.
- Scope addition (user-provided): This feature is end-to-end — it includes backend implementation, UI changes across all relevant web applications (`webapp`, `webapp-spaces`), and public documentation updates on the Astro public website.
- Q: Which authorization model should govern elevated refund operations? → A: Existing active organization Owner and Administrator membership roles with explicit per-mutation permission checks; Member and non-members are denied.
- Q: Which refund-ownership rule should the system use? → A: Resolve one canonical billed owner first, then use it consistently for payment status, calculation, currency, allocation, and refund persistence.
- Q: Which retry policy should provider refund operations use? → A: Temporal exponential backoff with classified errors, three automatic attempts, and operational review/dead-letter handling after exhaustion.
- Q: Which post-payout Stripe refund policy should the system use? → A: Prefer transfer reversal, fall back to a platform-funded refund when reversal is unavailable, and route ambiguous cases to reconciliation.
- Q: How should legacy refund states be handled? → A: Exclude legacy manual/accounting states from the canonical refund model; only the ten defined states are valid for new records and transitions.

---

## User Scenarios & Testing

### User Story 1 – Customer Cancels Within the Cancellation Window (Priority: P1)

A customer cancels a confirmed, paid booking before the cancellation deadline defined in the product's cancellation policy. The customer is shown the refund amount before confirming, and after cancellation the refund is processed automatically according to the payment method used. The customer receives clear feedback at every step.

**Why this priority**: This is the most common customer-initiated refund path and has the highest frequency of use. Getting this wrong causes immediate, visible financial harm.

**Independent Test**: Can be fully tested by creating a paid Spaces booking with a cancellation-policy window, cancelling before the deadline, and verifying that the correct refund amount is processed and the customer's booking and payment history reflect the outcome.

**Acceptance Scenarios**:

1. **Given** a paid booking exists with an active cancellation window, **When** the customer requests cancellation, **Then** the customer is shown the estimated refund amount, the non-refundable amount (if any), the cancellation policy being applied, and whether the refund will be automatic or requires review.
2. **Given** the customer confirms cancellation, **When** the refund is submitted to the payment provider, **Then** the booking is marked cancelled, the refund is recorded as processing, and the customer is notified of the submission.
3. **Given** the payment provider confirms the refund, **When** the confirmation is received, **Then** the refund is marked completed, the customer is notified that the money has been returned, and the refund record contains the provider reference.
4. **Given** the cancellation window has not expired, **When** the customer refreshes the booking page after cancellation, **Then** the booking shows as cancelled and the refund shows its current status, clearly distinguishing between "booking cancelled" and "refund completed".

---

### User Story 2 – Customer Cancels Outside the Cancellation Window (Priority: P1)

A customer attempts to cancel a paid booking after the cancellation deadline has passed. The system evaluates the product's cancellation policy snapshot that was in effect at the time of purchase and determines the applicable refund amount (which may be zero, partial, or the cancellation policy may allow full refunds regardless of timing for operator-initiated cancellations). The customer is shown the result clearly.

**Why this priority**: Incorrect out-of-window refund calculations represent direct financial risk. If refunds are issued when they should not be, or denied when they should be permitted, it creates customer disputes and accounting errors.

**Independent Test**: Can be fully tested by creating a paid booking and cancelling it after the cancellation window has closed. Verify that the correct policy snapshot applies and the customer sees the non-refundable explanation.

**Acceptance Scenarios**:

1. **Given** a booking where the cancellation window has expired, **When** the customer requests cancellation, **Then** the system applies the cancellation-policy snapshot from purchase time (not the current product price), and displays the eligible refund amount including any cancellation fees.
2. **Given** a product price has changed since the booking was made, **When** the customer cancels outside the window, **Then** the policy applied is the one that was in effect when the booking was purchased, not the current policy.
3. **Given** the policy specifies zero refund after the window expires, **When** the customer cancels, **Then** the system correctly records a refund of zero, marks the booking cancelled, and does not attempt to process a provider refund.

---

### User Story 3 – Operator Cancels a Customer Booking (Priority: P1)

A Spaces or Host operator cancels a booking on behalf of a customer, or the platform cannot fulfill it because a resource or location becomes unavailable. Provider/platform-caused non-fulfillment must result in a full refund of the undelivered booking value, regardless of the customer cancellation policy. Customer-initiated cancellation continues to use the purchase-time cancellation policy.

**Why this priority**: Operator-initiated cancellations are the most common scenario that overrides normal policy rules. Failing to refund a customer in this case causes serious trust and regulatory harm.

**Independent Test**: Can be tested by a Spaces administrator cancelling a paid booking from the admin panel and verifying a full refund is automatically initiated without requiring cancellation-policy evaluation.

**Acceptance Scenarios**:

1. **Given** an operator cancels a booking they cannot fulfill, **When** the cancellation is processed, **Then** the full paid amount (excluding non-refundable processing fees if applicable) is returned to the customer regardless of cancellation policy.
2. **Given** an operator closes a location or deactivates a resource that has active paid bookings, **When** the system processes the affected bookings, **Then** each affected customer receives a full refund of the undelivered booking value and appropriate notification; already delivered service is not refunded automatically.
3. **Given** an operator partially cancels a recurring booking series, **When** only future occurrences are cancelled, **Then** refunds are calculated and issued only for the undelivered occurrences.
4. **Given** the refund cannot be processed immediately (such as for bank transfer), **When** the operator cancellation is confirmed, **Then** the booking is immediately marked cancelled and a refund record is created in a pending state awaiting manual processing.

---

### User Story 4 – Customer Cancels a Subscription (Priority: P1)

A customer cancels a Spaces marketplace subscription. Depending on the billing model (upfront, arrears, weekly, fortnightly, monthly) and the point in the billing cycle, a pro-rated refund may apply. The system must correctly determine which occurrences have been delivered, which have not, and what refund is owed.

**Why this priority**: Subscription refunds are complex (multiple billing cycles, pro-rated calculations, recurring invoice entanglements) and getting them wrong affects customers who have ongoing financial relationships with the platform.

**Independent Test**: Can be tested by activating a subscription with upfront billing for a monthly cycle, cancelling partway through the cycle, and verifying that the correct pro-rated amount is refunded based on the consumed portion.

**Acceptance Scenarios**:

1. **Given** a customer has an active subscription with unconsumed billing periods, **When** the customer cancels immediately, **Then** a pro-rated refund is calculated for the unused portion and offered to the customer before confirmation.
2. **Given** a customer selects "cancel at period end," **When** the subscription period ends, **Then** no further charges are made, the subscription is not renewed, and no refund is issued for the current period (which has already been consumed).
3. **Given** a subscription with arrears billing is cancelled, **When** the final billing cycle is closed, **Then** only the consumed portion of the cycle is charged and any overpayment is refunded.
4. **Given** a customer cancels a subscription while a recurring invoice (Xero) is outstanding, **When** the cancellation is processed, **Then** the outstanding recurring invoice template is cancelled and future installments are not generated; already-generated and sent invoices are handled as per credit-note policy.

---

### User Story 5 – Payment Succeeds but Booking Creation Fails (Priority: P1)

A customer completes payment but the booking cannot be created due to a resource conflict, system error, or availability issue. The payment provider has already captured funds. The system must automatically initiate a full refund without requiring customer intervention.

**Why this priority**: This is a reliability gap that could leave customers charged with no booking confirmation. It is both a financial and trust risk.

**Independent Test**: Can be simulated by completing a payment and injecting a booking-creation failure, then verifying that a full refund is automatically initiated and the customer is notified.

**Acceptance Scenarios**:

1. **Given** payment has been captured and booking creation fails, **When** the failure is detected, **Then** a full automatic refund is immediately initiated and the customer is notified that no booking was created and a refund is processing.
2. **Given** only some bookings in a recurring series can be created after payment, **When** the partial creation is detected, **Then** the customer is notified of the partial outcome, presented with the available occurrences and the pro-rated refund for the uncreated ones, and given 24 hours to explicitly accept the partial booking. If the customer accepts, only the pro-rated refund for uncreated occurrences is issued. If the customer does not respond within 24 hours, a full refund is issued automatically and all created occurrences are cancelled.
3. **Given** the automatic refund itself fails (for example, due to provider unavailability), **When** the failure is detected, **Then** the refund is queued for retry and an alert is raised for operational review; the customer is informed of the delay.

---

### User Story 6 – Administrator Reviews and Processes a Manual Bank-Transfer Refund (Priority: P2)

A customer paid via bank transfer and is now entitled to a refund. Because bank transfers cannot be automatically reversed, an administrator must manually process the refund through an explicit approval workflow. The system provides a structured review queue to prevent duplicate, lost, or incorrectly processed refunds.

**Why this priority**: Bank-transfer refunds are the highest-risk manual financial operation. Without a structured workflow, there is high exposure to missed refunds, duplicate payments, and unaudited cash movements.

**Independent Test**: Can be tested by recording a bank-transfer payment, triggering a cancellation, navigating to the manual refund queue, approving the refund, recording payment details, and verifying the refund is marked as sent and the audit trail is complete.

**Acceptance Scenarios**:

1. **Given** a refund is pending for a bank-transfer payment, **When** an administrator opens the refund review queue, **Then** the refund appears with full context: original payment reference, amount, customer details, cancellation reason, and calculation breakdown.
2. **Given** an administrator approves and marks a bank transfer as sent, **When** they confirm the action, **Then** they must provide a payment reference number and confirmation date; the system records these and marks the refund as sent.
3. **Given** a refund has already been marked as sent, **When** any administrator attempts to process the same refund again, **Then** the system prevents duplicate action and displays the existing payment record.
4. **Given** a bank-transfer refund is overdue, **When** no administrator action has been taken within a configured threshold, **Then** an alert is raised to the administrator and operations team.

---

### User Story 7 – Customer Views Refund Status in Booking History (Priority: P2)

At any point after requesting a cancellation or refund, a customer can view the current status of their refund, understand what stage it is at, and know what to expect next. The status must accurately reflect what has actually happened, not what is hoped to happen.

**Why this priority**: Customer trust requires accurate, honest communication. Showing a completed refund before money has actually moved is a significant trust failure.

**Independent Test**: Can be tested by cancelling a paid booking at each stage (processing, pending, completed, failed) and verifying the UI shows the correct status with appropriate explanation.

**Acceptance Scenarios**:

1. **Given** a refund is in processing state, **When** the customer views their booking, **Then** they see "Refund processing" with an explanation that it may take several business days, not "Refund completed."
2. **Given** a refund has been completed by the provider, **When** the customer views their booking, **Then** they see "Refund completed" with the amount, date, and method.
3. **Given** a refund has failed, **When** the customer views their booking, **Then** they see an honest failure message and, where appropriate, how to contact support for resolution.
4. **Given** a booking was cancelled but the refund is zero (outside cancellation window), **When** the customer views the booking, **Then** the booking is clearly marked cancelled and the refund shows as "Not applicable" with the reason.

---

### User Story 8 – Administrator Issues a Partial Refund (Priority: P2)

An authorized Spaces or Host administrator determines that a partial refund is appropriate for a situation not covered by automatic policy evaluation (such as a service quality issue or partial delivery). The administrator can initiate a partial refund, provide a reason, and the system issues it to the correct payment provider.

**Why this priority**: Discretionary partial refunds are a common customer service action. Without a structured path, they either don't happen or are processed outside the system, breaking the audit trail.

**Independent Test**: Can be tested by a Spaces admin issuing a partial refund through the admin panel for a paid booking and verifying the correct amount is submitted to Stripe and recorded in the refund history.

**Acceptance Scenarios**:

1. **Given** a paid booking and a Spaces admin with the appropriate permissions, **When** the admin initiates a partial refund, **Then** the system validates that the requested amount does not exceed the remaining refundable balance (original amount minus all previous refunds).
2. **Given** the partial refund amount is valid, **When** it is submitted, **Then** it is sent to the appropriate payment provider, the refund record is created, and both the customer and the admin receive a notification.
3. **Given** an admin attempts to issue a refund that would exceed the paid amount, **When** the action is submitted, **Then** the system rejects it with a clear explanation of the remaining refundable balance.

---

### User Story 9 – Booking Modification Results in a Price Reduction (Priority: P3)

A booking modification reduces the total amount owed (for example, reducing the duration of a booking, removing recurring occurrences, or changing to a lower-priced product tier). The system determines whether a price difference is owed to the customer, presents it as a refund preview, and processes it through the correct payment path.

**Why this priority**: Booking modifications are less common than cancellations and require careful business-rule definition. The risk is lower than full cancellation refunds but still requires a clear defined behavior.

**Independent Test**: Can be tested by modifying a booking to a shorter duration or lower product price and verifying the system calculates the correct credit and presents it for confirmation before processing.

**Acceptance Scenarios**:

1. **Given** a paid booking is modified to reduce duration or price, **When** the modification is confirmed, **Then** the system calculates the price difference and presents it as a refund preview before the modification is applied.
2. **Given** the customer confirms the modification and refund, **When** the refund is processed, **Then** it is submitted to the correct payment provider and the booking reflects the new reduced price and schedule.

---

### User Story 10 – Failed Refund is Recovered by Operations (Priority: P2)

A refund fails after being initiated (for example, due to a Stripe error, Xero API failure, or provider timeout). An operations team member or administrator can locate the failed refund, investigate it, retry it safely, or mark it as manually resolved with supporting documentation.

**Why this priority**: Failed refunds represent real financial obligations that must be tracked and resolved. Without operational tooling, failed refunds can disappear into system logs while customers remain owed money.

**Independent Test**: Can be tested by injecting a provider failure during refund processing, verifying the refund is flagged in an operational view, retrying it, and confirming the retry completes correctly.

**Acceptance Scenarios**:

1. **Given** a refund has failed after retry limits are exhausted, **When** an operations user views the refund dashboard, **Then** the failed refund appears with the failure reason, retry history, provider reference, and customer details.
2. **Given** the underlying cause is resolved, **When** an operations user triggers a retry, **Then** the retry uses a stable idempotency key and submits the same request to the provider without creating a new refund record.
3. **Given** an operations user resolves a failed refund outside the system, **When** they record the resolution, **Then** they must provide the external reference, resolution method, and a note; the refund transitions from `ReconciliationRequired` to `Completed` with an audit entry.

---

### User Story 11 – Public Documentation Covers Refund Policies and Processes (Priority: P2)

A prospective or existing customer visiting the Skedular public website can read clear, plain-language documentation explaining how refunds work for Spaces and Host bookings. The documentation accurately describes cancellation policy windows, refund timelines by payment method, how to request a refund, what to expect at each status stage, and how to contact support if something goes wrong. This documentation is created or updated as part of this feature to reflect the new refund system.

**Why this priority**: Public documentation is the first place customers look when they have a question about money. Outdated or missing documentation creates unnecessary support load and reduces trust. It must go live alongside the feature changes it describes.

**Independent Test**: Can be verified by a reviewer checking that the public website contains accurate refund documentation for each product and payment method covered by this feature, matching the live system behavior.

**Acceptance Scenarios**:

1. **Given** the public website is updated, **When** a customer searches for refund or cancellation information, **Then** they find a dedicated page or section that explains the cancellation policy, the refund process, timelines per payment method (Stripe, Xero invoice, bank transfer), and how to contact support.
2. **Given** the documentation is published, **When** a Spaces or Host administrator reviews it, **Then** it accurately reflects the operator-facing cancellation and refund workflows without contradicting the system behavior.
3. **Given** a payment method (such as bank transfer) requires manual handling, **When** the customer reads the documentation, **Then** it clearly explains that manual bank-transfer refunds require administrator action and sets honest expectations about processing time.
4. **Given** the refund system is updated after initial release, **When** documentation becomes stale, **Then** the public documentation update task is explicitly part of the implementation checklist for the change, not optional.

---

### Edge Cases

- What happens when the customer and an administrator cancel the same booking simultaneously?
- What happens when a provider webhook arrives after the local refund record has already been updated to a terminal state?
- What happens when a provider refunds more than the amount requested?
- How does the system handle a refund for a booking paid across multiple payment methods or invoices?
- What happens when a recurring booking has partially consumed and partially future occurrences at the time of cancellation?
- How are refund amounts handled when GST or other taxes are part of the original charge and the tax rate has changed since purchase?
- What happens when a Stripe payout has already been disbursed to the operator before the refund is initiated?
- What happens when a Xero credit note is created successfully but the corresponding money movement fails?
- What happens when the same cancellation event is delivered more than once through Kafka or a Temporal retry?
- What happens when a refund record exists but the provider reports no matching transaction?

---

## Requirements

### Functional Requirements

**Audit and Investigation**

- **FR-001**: The system MUST produce a complete inventory of all refund-triggering workflows before any implementation changes are made, covering all booking types, subscription types, payment methods, and product lines.
- **FR-002**: The system MUST identify and document all existing refund-related code, including services, commands, event handlers, scheduled jobs, Temporal workflows, database entities, and API endpoints.
- **FR-003**: The system MUST document the current-state refund flow for every discovered refund path, including where it succeeds, where it fails, and where it has no coverage.

**Refund Initiation**

- **FR-010**: The system MUST support refund initiation from all of the following sources: customer self-service cancellation, operator/administrator cancellation, automated booking failure recovery, and manual administrative override.
- **FR-011**: Before confirming a cancellation that involves a refund, the system MUST present the customer with an estimated refund amount, the applicable cancellation policy, the non-refundable amount (if any), and whether the refund is automatic or requires review.
- **FR-012**: Operator-initiated cancellations due to non-fulfillment MUST result in a full refund to the customer regardless of the cancellation policy window.
- **FR-013**: The system MUST NOT initiate a refund record without first confirming that a corresponding confirmed payment exists.
- **FR-014**: When payment has been captured but only a subset of recurring booking occurrences can be created, the system MUST notify the customer of the partial outcome and provide a 24-hour window for the customer to explicitly accept the partial booking. If the customer does not explicitly accept within 24 hours, the system MUST automatically issue a full refund and cancel all created occurrences. A pro-rated refund for uncreated occurrences is issued only after explicit customer acceptance.

**Refund Calculation**

- **FR-020**: Every refund MUST be calculated against the cancellation-policy snapshot that was in effect at the time of purchase, not the current policy on the product.
- **FR-021**: Refund calculations MUST be deterministic: the same inputs (booking, cancellation time, policy snapshot, previously refunded amount) MUST always produce the same result.
- **FR-022**: The refund calculation result MUST be structured to show: original gross amount, eligible refund amount, cancellation deduction, tax adjustment, previously refunded amount, final refundable amount, non-refundable amount, and the reason for each component.
- **FR-023**: All monetary calculations MUST use decimal-safe arithmetic. Floating-point arithmetic MUST NOT be used for any money-related calculation.
- **FR-024**: The calculation snapshot used at the time a refund is approved MUST be persisted and remain associated with the refund record permanently.
- **FR-025**: The system MUST prevent the total of completed and pending refunds from exceeding the original amount paid for any single payment or booking.
- **FR-026**: Every refund MUST allocate its amount to one or more immutable source-payment references. The system MUST support split tenders and multiple invoice/payment allocations without exceeding any source payment's captured amount.

**Refund Domain Model**

- **FR-030**: Refunds MUST be represented as first-class records in the system, not as boolean flags or status fields on booking or payment entities.
- **FR-031**: Every refund record MUST be associated through durable allocation records with at least one payment, the booking or subscription it relates to, the customer, and the organization. Before preview, payment confirmation, calculation, allocation, or persistence, the system MUST resolve one canonical billed owner for the refund scope and use that same owner throughout the operation: the one-time booking payment, the billed recurring booking for a recurring window, or the billed recurring booking/cycle for a subscription window. The subscription root MUST NOT substitute for the billed recurring owner.
- **FR-032**: Refund records MUST maintain a complete status history, including timestamps, actors, and reasons for every state transition.
- **FR-033**: The system MUST define a formal refund state machine with the following canonical states: `Requested`, `UnderReview`, `Approved`, `Rejected`, `Processing`, `ProviderPending`, `Completed`, `Failed`, `Cancelled`, and `ReconciliationRequired`. Permitted transitions include: `Requested → UnderReview / Processing / Cancelled`, `UnderReview → Approved / Rejected / Cancelled`, `Approved → Processing / Cancelled`, `Processing → ProviderPending / Completed / Failed / ReconciliationRequired`, `ProviderPending → Completed / Failed / ReconciliationRequired`, `Failed → Processing / ReconciliationRequired`, and `ReconciliationRequired → Completed / Failed`. Transitions outside the allowed set MUST be rejected.
- **FR-034**: Every refund MUST carry a stable idempotency key that uniquely identifies the refund operation and survives retries, replays, and system restarts.

**Provider Processing**

- **FR-040**: Stripe refunds MUST be processed through the correct account and charge type (direct charge, destination charge, or platform charge) for the specific booking context. The implementation plan MUST document a charge matrix covering Host, Spaces, subscriptions, and post-payout refunds, including the Stripe account, charge type, transfer behavior, and selected refund path.
- **FR-041**: Xero credit notes and cash refunds MUST be distinguished explicitly. The system MUST NOT assume that creating a Xero credit note constitutes returning money to the customer.
- **FR-042**: Bank-transfer refunds MUST follow a structured manual workflow with explicit approval, payment recording, reference number capture, and confirmation steps. The system MUST NOT automatically mark a bank-transfer refund as completed.
- **FR-043**: The system MUST handle partial refunds for Stripe, where multiple partial refunds against a single payment intent are supported, and MUST enforce that the sum of all partial refunds does not exceed the original captured amount.
- **FR-044**: After a Stripe payout has been disbursed to an operator, the system MUST prefer transfer reversal when supported by the original charge context, fall back to a platform-funded refund when reversal is unavailable, and record the selected approach plus provider identifiers. Ambiguous or failed path selection MUST route the refund to reconciliation rather than guessing or silently failing.

**Idempotency and Concurrency**

- **FR-050**: Repeated submission of the same cancellation or refund command, whether due to retry, message replay, or user action, MUST NOT create multiple refund records or submit multiple provider refund requests.
- **FR-051**: The system MUST prevent concurrent refund requests for the same booking or payment from collectively exceeding the refundable amount, using transactional or locking mechanisms.
- **FR-052**: If a provider request succeeds but the local system fails before recording the result, the system MUST be able to detect this on retry and reconcile the state without creating a duplicate provider refund.

**Failure Handling and Recovery**

- **FR-060**: The system MUST execute provider refund operations through Temporal retry policies using exponential backoff, explicit retryable and non-retryable error classification, a maximum of three automatic attempts, and durable operational review/dead-letter handling after exhaustion. Retries MUST preserve the refund operation idempotency key and MUST NOT create a new financial operation.
- **FR-061**: A refund that fails after all retries MUST be surfaced to an operational review queue with full context for human investigation and retry.
- **FR-062**: Provider timeouts during refund operations MUST NOT leave the system assuming success or failure. The system MUST reconcile the actual outcome before updating refund status.
- **FR-063**: A failed refund MUST NOT automatically restore a cancelled booking or reverse any booking-cancellation effects. A provider timeout MUST keep the refund in `ProviderPending` until reconciliation resolves it; only an unresolved or mismatched reconciliation result may transition it to `ReconciliationRequired`.

**Reconciliation**

- **FR-070**: The system MUST support a hybrid reconciliation model: Stripe refund status MUST be updated in near-real-time via provider webhooks; Xero invoice, credit-note, and payment status and bank-transfer refund status MUST be reconciled via a scheduled daily batch process. Both paths MUST compare Skedular refund records with provider records to identify mismatches, missing records, and inconsistent statuses.
- **FR-071**: Reconciliation results MUST be visible to administrators and support staff, including pending refunds beyond expected thresholds, failed provider requests, and external refunds not recorded in Skedular. Reconciliation work MUST use a database-backed lease or claim mechanism so multiple job replicas cannot process the same batch or refund concurrently.
- **FR-072**: Reconciliation MUST NOT automatically correct financial records without human confirmation. Individual refund records MUST be claimed with a short renewable database-backed lease; expired claims MUST be reclaimable by another worker, and active claims MUST prevent concurrent processing.

**Notifications**

- **FR-080**: Notifications MUST be sent at the following points: cancellation confirmed, refund requested, refund approved, refund processing, refund completed, refund failed, manual action required (bank transfer), bank refund sent.
- **FR-081**: Each notification MUST clearly distinguish between "booking cancelled," "refund approved," "refund processing," and "refund completed." These are NOT synonymous and MUST NOT use identical messages. Notification delivery MUST be persisted with a deduplication key composed of refund, event/status, and recipient.
- **FR-082**: Notifications MUST be idempotent. Duplicate events or retries MUST NOT send the same notification more than once to any recipient.

**Authorisation**

- **FR-090**: Customers MUST only be able to initiate refunds for their own bookings and MUST NOT be able to request a refund amount greater than their calculated eligibility.
- **FR-091**: Organisation administrators MUST only be able to access and action refunds belonging to their own organisation.
- **FR-092**: Elevated refund operations (issuing discretionary partial refunds, approving or rejecting refunds, resolving reconciliation, marking bank transfers as sent, and retrying failed refunds) MUST require an active organization Owner or Administrator membership role, with an explicit permission check at each mutation. Members, non-members, and users from another organization MUST be denied. Organization-level isolation MUST still apply.
- **FR-093**: Provider webhook endpoints MUST validate provider signatures before processing any refund status updates.

**Critical Bug Fixes (Confirmed by Code Audit)**

- **FR-094**: A refund in a terminal state (`Completed`, `Rejected`, `Cancelled`) MUST NOT be reset to `Requested` or any non-terminal state by a subsequent cancellation request. The upsert path in refund creation MUST check the current status and return the existing record unchanged if it is terminal. Legacy manual/accounting states are removed from the canonical model and MUST NOT be created by new code.
- **FR-095**: All administrator refund state transitions \u2014 including approval, processing, completion, rejection, cancellation, reconciliation resolution, and failure retry \u2014 MUST check that a confirmed payment exists before applying the transition. The payment confirmation gate MUST NOT be limited to the Xero processing path only.
- **FR-096**: When a Stripe refund status changes via webhook (refund.created, refund.updated, refund.failed), the system MUST raise a GraphQL subscription event and send the appropriate customer notification after updating the local refund record. Webhook-driven status changes MUST be as visible as admin-driven changes.
- **FR-097**: In the Xero credit-note refund flow, the Xero credit-note identifier MUST be persisted to the local refund record immediately after the credit note is successfully created in Xero, before any subsequent allocation or payment step is attempted. If the allocation or payment step fails, the credit-note ID must already be saved so the failure is recoverable by reconciliation or manual retry.
- **FR-098**: A customer may accept or decline a partial recurring-booking outcome before its deadline. Acceptance MUST issue only the allocated prorated refund for uncreated occurrences; decline or deadline expiry MUST cancel created occurrences and issue the full refund. Both outcomes MUST be idempotent and durable across workflow retries.

**Web Application and Documentation Coverage**

- **FR-110**: Refund-related UI changes MUST be implemented in all web applications where customers and administrators interact with bookings and refunds. This includes the customer-facing marketplace application (`webapp`) for booking history, cancellation flows, and refund status; and the Spaces operator application (`webapp-spaces`) for the administrator refund queue, partial refund initiation, and reconciliation views.
- **FR-111**: The Skedular public website MUST be updated with accurate, plain-language documentation covering: how refunds work for Spaces and Host bookings, cancellation policy windows, refund timelines for each payment method (Stripe, Xero invoice, bank transfer), the manual bank-transfer refund process, and how to contact support for refund issues. This documentation MUST go live no later than the corresponding feature changes.

**Teams Scope Boundary**

- **FR-100**: The system MUST document explicitly whether Teams currently involves any customer payment, marketplace invoice, or refund workflow. Based on current platform knowledge, Teams has no marketplace payment or booking-level refund workflow; this MUST be confirmed during the audit phase.
- **FR-101**: Refunds or credits relating to the Skedular SaaS subscription billed to Teams organizations are explicitly out of scope for this feature. Any such billing is a separate platform concern and MUST NOT be mixed with marketplace booking refund workflows.

### Observability and Logging Requirements

- **LOG-001**: Every refund record transition MUST emit a structured log event containing the refund ID, previous status, new status, actor, booking reference, payment reference, and correlation ID.
- **LOG-002**: All provider interactions (Stripe API calls, Xero API calls, webhook receipts) MUST emit structured logs with correlation ID, request/response summary, duration, and outcome.
- **LOG-003**: Refund calculation results MUST be logged at request time with all input components and the final breakdown.
- **LOG-004**: Logs MUST NOT contain card numbers, bank account numbers, access tokens, provider secret keys, or personally identifiable information beyond the minimum required for support investigation.
- **LOG-005**: Operational dashboards MUST be defined for: refunds pending beyond threshold, refunds in failed state, bank refunds approved but not sent, reconciliation mismatches, and cancelled bookings with no refund decision.

### Key Entities

- **Refund**: A first-class record representing a customer's entitlement to returned funds for a specific payment. Holds status, amount components, provider references, idempotency key, calculation snapshot, and audit history.
- **Refund Payment Allocation**: A required durable record that links a refund amount to a source payment or invoice and enforces the remaining refundable balance for that source payment.
- **Refund Allocation**: Optionally breaks a refund payment allocation across booking occurrences, invoice lines, tax lines, platform fees, and operator proceeds.
- **Cancellation Policy Snapshot**: A captured copy of the cancellation policy terms at the time of booking purchase. Must be immutable after capture and must not reflect subsequent policy changes on the product.
- **Refund Calculation Result**: A structured breakdown of how the final refundable amount was determined, including each component. Persisted at the time of approval.
- **Refund Audit Entry**: An immutable record of every state change, action, or decision made on a refund, including actor, timestamp, reason, and before/after values.
- **Bank Transfer Refund Record**: Extends the base refund with manual workflow fields: approval status, approver, payment reference, confirmation date, and proof reference.
- **Provider Refund Reference**: Stores the external identifier, status, and reconciliation result returned by Stripe, Xero, or another provider.

---

## Success Criteria

### Measurable Outcomes

- **SC-001**: Every identified refund-triggering workflow is documented before any implementation change begins. Zero known refund triggers are left without a documented current-state flow.
- **SC-002**: After implementation, the total of all completed and in-flight refunds for any single payment cannot exceed the original captured amount. This must be enforced at the data and application layers.
- **SC-003**: Repeated submission of the same cancellation or refund (due to retry, replay, or double-click) creates exactly one refund record and one provider submission. Zero duplicate financial transactions.
- **SC-004**: Customers can view the accurate current status of their refund at all times. For Stripe refunds, the status shown must reflect provider-confirmed state within minutes of the provider webhook arriving. For Xero and bank-transfer refunds, the status must be updated within one business day via the scheduled daily reconciliation batch.
- **SC-005**: Failed refunds are visible in an operational queue within the retry timeout period, with full context for human investigation. Zero failed refunds are silently lost.
- **SC-006**: Bank-transfer refunds follow a complete, auditable workflow with no possibility of being marked as completed without a payment reference and administrator confirmation.
- **SC-007**: Cancellation-policy calculations are deterministic: given the same inputs, the system always produces the same refund amount. This is verifiable through a comprehensive automated test suite.
- **SC-008**: A provider timeout during refund processing does not leave the system in an unknown state. Within the reconciliation cycle, the actual outcome is resolved and the refund status updated accordingly.
- **SC-009**: All refund-related actions are recorded in an immutable audit trail that includes actor, timestamp, reason, and before/after state. Zero auditable refund actions occur without a corresponding audit entry.
- **SC-010**: Cross-tenant access is not possible at any refund endpoint. An administrator of Organization A cannot view, approve, or action refunds belonging to Organization B.
- **SC-011**: Comprehensive automated test coverage exists for: cancellation policy calculations, pro-rated subscription refunds, partial refunds, duplicate-request prevention, provider failure scenarios, and concurrent-request safety.

---

## Assumptions

- The existing cancellation policy data structure and product-price versioning model will be used as the foundation; the spec does not propose replacing the overall pricing model.
- Skedular Teams does not currently involve direct customer payments or marketplace invoices. Any Teams-related refund scope is limited to documenting whether such workflows exist (audit only). The Skedular SaaS subscription billed to Teams organizations is entirely out of scope for this feature.
- The primary automatic refund payment path for Spaces and Host is Stripe; Xero handles accounting projections and invoicing but is not directly responsible for returning money to customers.
- Stripe Connect is already configured; the spec assumes the platform uses destination charges or separate charges (not direct charges) for Spaces, but this must be confirmed during the audit phase.
- The initial deployment uses a new empty refund schema; historical refund-record migration and backward-compatibility handling are out of scope.

### Session 2026-07-27

- Q: How should Stripe charge behavior be specified? → A: Document the exact Stripe account, charge type, transfer behavior, and refund path for each booking context.
- Q: How should reconciliation coordinate across multiple job replicas? → A: Use a database-backed lease or claim mechanism for reconciliation work.
- Q: How should exhausted refund workflows be handled? → A: Persist them in the refund operations queue with full failure context and authorized retry/resolution actions.
- Q: How should refund notification idempotency work? → A: Persist notification-delivery keys by refund, event/status, and recipient.
- Q: How should database-backed reconciliation leases work? → A: Claim each refund with a short renewable lease; expired claims can be reclaimed by another worker.
- Q: How should unresolved provider timeouts be represented? → A: Keep the refund in `ProviderPending` until reconciliation cannot resolve the outcome; then move it to `ReconciliationRequired`.
- The refund calculation for recurring subscriptions applies pro-rated arithmetic to the billing cycle and occurrence schedule, not to arbitrary calendar periods.
- Notifications use the platform's existing email and in-application notification infrastructure; this spec does not require building new notification delivery channels.
- Time-zone handling for cancellation policy deadline calculation must use the location's or organization's configured time zone, not UTC or the server time zone.
- Refund entities, services, state machine, APIs, and audit trail are owned within the existing booking domain. No new top-level domain or service boundary is introduced for refunds. Cross-domain refund data (such as organization details or location context) is accessed via the established inter-domain patterns (GraphQL federation, gRPC, or Kafka events) rather than direct persistence access.
- The scope of "booking modification triggers a refund" for this specification is limited to changes that reduce the total charge. Modifications that increase the charge are an additional-payment path and are out of scope for this refund specification.
- This feature requires UI changes across multiple web applications: the customer-facing marketplace app (`webapp`) for booking cancellation, refund preview, and refund status; and the Spaces operator admin app (`webapp-spaces`) for the administrator refund queue, partial refund tooling, and reconciliation views. The Teams web app (`webapp-teams`) requires no refund UI changes under this feature.
- The Skedular public website (Astro static site) requires new or updated documentation pages covering the refund process, cancellation policies, payment-method timelines, and support contact paths. Documentation updates ship alongside the feature implementation, not after.
