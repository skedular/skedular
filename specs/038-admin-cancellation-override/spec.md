# Feature Specification: Admin Cancellation Policy Override

**Feature Branch**: `038-admin-cancellation-override`  
**Created**: 2026-08-02  
**Status**: Draft  
**Input**: User description: "Allow coworking space owners and admins to override cancellation policies for subscriptions and bookings while preserving policy enforcement for customers."

## Clarifications

### Session 2026-08-02

- Q: Should an administrative cancellation override automatically authorize a refund? → A: No. The override cancels the booking or subscription, while refund eligibility and amount remain separate decisions.
- Q: Which organization grants cancellation-override authority? → A: Only the product-owning coworking space or host organization.
- Q: Which administrators may override cancellation policy? → A: Owners and administrators with existing booking/subscription management permission.
- Q: Should an override reason be mandatory? → A: Yes. A short reason is required for every owner/admin policy override.
- Provider behavior clarification: Stripe refunds remain automatically processed when a refund is created and eligible; bank-transfer refunds remain pending owner/admin approval and subsequent transfer confirmation; Xero refunds remain subject to the existing owner/admin approval and Xero processing workflow. Cancellation-policy override MUST NOT bypass provider-specific refund approval or settlement controls.
- Q: When an admin overrides cancellation policy, should the refund request be created automatically? → A: Yes. Create the refund request according to existing payment/refund rules, then apply the existing provider-specific approval behavior.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Operator Cancels Despite Customer Policy (Priority: P1)

As a coworking space owner or authorized administrator, I want to cancel a customer's booking or subscription even when the customer-facing cancellation policy would reject the request, so that I can resolve operational, safety, service, or goodwill situations under my control.

**Why this priority**: Operators currently cannot reliably complete a legitimate administrative cancellation when customer policy conditions are not met.

**Independent Test**: Use an authorized owner/admin account to cancel a booking and an active subscription after the customer cancellation cutoff or under a no-cancellation policy; both cancellations complete and trigger the existing cancellation cleanup without being rejected by the customer policy.

**Acceptance Scenarios**:

1. **Given** an active booking whose customer cancellation window has closed, **when** an authorized owner or administrator cancels it, **then** the booking is cancelled and the normal payment, invoice, resource, and refund decision workflows run.
2. **Given** an active subscription under a no-cancellation or otherwise unmet policy, **when** an authorized owner or administrator requests immediate cancellation, **then** the subscription is cancelled immediately and future renewal and generated booking activity are stopped according to the existing cancellation behavior.
3. **Given** an active subscription under an unmet policy, **when** an authorized owner or administrator requests cancellation at period end, **then** the subscription remains active through the current cycle, renewal is disabled, and it transitions at the cycle boundary.

### User Story 2 - Customer Policy Remains Enforced (Priority: P1)

As a customer, I want the published cancellation policy to continue governing my own cancellation requests, so that the terms presented at purchase remain meaningful and consistent.

**Why this priority**: The override must not become a way for customers or untrusted callers to bypass commercial policy.

**Independent Test**: Submit the same booking and subscription cancellation requests from a customer context after the cutoff or under a no-cancellation policy; the requests are rejected or remain unavailable exactly as the current customer rules require.

**Acceptance Scenarios**:

1. **Given** a customer-owned booking after its cancellation cutoff, **when** the customer requests cancellation, **then** the request is denied and the booking remains unchanged.
2. **Given** a customer subscription with an unavailable cancellation mode or unmet policy, **when** the customer requests cancellation, **then** the request is denied and no administrative override is applied.
3. **Given** a caller attempts to identify itself as an owner/admin without valid organization authority, **when** it requests cancellation, **then** the request is denied and the policy is enforced as for an untrusted caller.

### User Story 3 - Operator and Customer Outcomes Are Explainable (Priority: P2)

As an operator or support user, I want cancellation results to identify whether policy rules were enforced or overridden, so that financial follow-up and customer communication can be handled correctly.

**Why this priority**: Cancellation and refund are separate decisions; operators need a reliable record of why a cancellation occurred and who initiated it.

**Independent Test**: Complete one customer cancellation and one authorized override, then inspect the resulting cancellation/refund history and operational logs for actor type, actor identity where available, policy outcome, and requested cancellation mode.

**Acceptance Scenarios**:

1. **Given** an authorized override cancellation, **when** the cancellation succeeds, **then** its audit history records that an owner/admin initiated it and that the customer cancellation policy was overridden.
2. **Given** a customer cancellation, **when** it succeeds or is rejected, **then** its result records that it was customer-requested and whether policy conditions were met.
3. **Given** a cancellation that creates a refund decision, **when** the cancellation completes, **then** the refund calculation remains a separate outcome and is not implied solely by the operator override.
4. **Given** an authorized policy override produces a refund request, **when** the payment provider is Stripe, **then** an eligible refund follows the existing automatic processing path.
5. **Given** an authorized policy override produces a refund request, **when** the payment provider is bank transfer or Xero, **then** the refund waits for the existing owner/admin approval and settlement workflow.

### Edge Cases

- A user may have access to the organization but lack the specific permission to manage bookings or subscriptions; that user must not receive override authority.
- A caller may be an owner/admin of one organization involved in a booking but not the product-owning organization; authorization must use the organization that owns the cancellable commercial offering and any existing cross-organization access rules.
- Repeated immediate cancellation requests, workflow retries, and concurrent cancellation requests must remain idempotent and must not create duplicate refund boundaries or duplicate provider cancellation actions.
- An operator override must not retract an invoice or guarantee a refund; existing invoice-cancellation and refund ownership rules still apply.
- An operator override must not bypass provider controls: Stripe remains automatic where currently eligible, while bank-transfer and Xero refunds require the existing owner/admin approval workflow before completion.
- If the booking or subscription is already cancelled, the operation must return the existing terminal state without changing its original actor or policy outcome.
- Cancellation of a parent subscription must preserve the existing boundary that customer-facing refund handling is not duplicated across every generated child booking.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST determine the cancellation actor from authenticated server-side identity and organization permissions, distinguishing at minimum customer-requested cancellation from owner/admin-requested cancellation.
- **FR-002**: The system MUST allow an authorized owner or administrator of the product-owning coworking space or host organization to cancel an eligible marketplace booking even when the customer cancellation policy conditions are not met.
- **FR-003**: The system MUST allow an authorized owner or administrator of the product-owning coworking space or host organization to cancel an eligible marketplace subscription immediately or at period end even when the customer cancellation policy conditions are not met.
- **FR-004**: The system MUST continue to enforce cancellation policy conditions for customer-requested booking and subscription cancellations.
- **FR-005**: The system MUST reject cancellation override attempts from callers who cannot prove the required owner/admin organization permission, including administrators without the existing booking/subscription management permission.
- **FR-006**: The system MUST preserve the selected cancellation mode and existing semantics: immediate cancellation stops the entitlement now, while period-end cancellation disables renewal and keeps the current cycle active until its boundary.
- **FR-007**: The system MUST continue existing cancellation cleanup for resources, generated booking activity, payment workflows, accounting invoices, and refunds, subject to the established rules for each payment state and cancellation outcome.
- **FR-008**: The system MUST keep cancellation authorization separate from refund eligibility and amount calculation; an operator override MUST cancel the booking or subscription without automatically authorizing a full, partial, or any refund.
- **FR-008a**: When a cancellation produces a refund outcome, the system MUST preserve provider-specific processing: eligible Stripe refunds may process automatically, while bank-transfer and Xero refunds MUST remain subject to their existing owner/admin approval and settlement workflows.
- **FR-008b**: An authorized cancellation override MUST create any refund request required by the existing payment and refund rules without requiring a second cancellation request; refund processing MUST then follow the applicable provider workflow.
- **FR-009**: The system MUST record the cancellation actor category, actor identity when available, organization context, requested mode, whether policy was overridden, and the resulting cancellation outcome in durable audit history or equivalent operational history.
- **FR-009a**: The system MUST require and retain a short operator-provided reason for every cancellation that overrides customer policy.
- **FR-010**: The system MUST make the authorization and policy decision server-authoritative; client-provided flags or labels MUST NOT be sufficient to obtain an override.
- **FR-011**: The system MUST preserve idempotent behavior for retries and repeated requests, including avoiding duplicate refunds, invoice cancellation actions, or external provider cancellation actions.
- **FR-012**: User-facing operator and customer messages MUST clearly distinguish insufficient permission from a customer cancellation-policy restriction and MUST use American English.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured logs when a booking or subscription cancellation starts and completes, including cancellation mode and actor category.
- **LOG-002**: The feature MUST emit structured logs for authorization outcome, policy evaluation outcome, and whether an administrative override was applied.
- **LOG-003**: The feature MUST emit actionable warning/error logs for denied authorization, failed cleanup, retry, and recovery paths.
- **LOG-004**: Feature logs MUST include correlation context and relevant booking/subscription identifiers while avoiding payment credentials and unnecessary personal data.

### Key Entities

- **Cancellation Request**: The requested cancellation target, mode, authenticated actor, organization context, and resulting outcome.
- **Cancellation Actor**: The customer, owner, or administrator whose authenticated authority determines whether policy enforcement or override rules apply.
- **Cancellation Policy Outcome**: The policy evaluation result, including whether conditions were met, bypassed by an authorized operator, or caused rejection.
- **Cancellation Audit Event**: Durable history explaining who requested cancellation, what authority was used, what policy decision occurred, and what cleanup/refund outcome followed.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In acceptance testing, 100% of authorized owner/admin cancellation requests can complete for eligible bookings and subscriptions when customer policy conditions are unmet.
- **SC-002**: In acceptance testing, 100% of customer cancellation requests that violate the configured customer policy remain rejected or unavailable, with no customer-accessible override path.
- **SC-003**: At least 99% of repeated or retried cancellation requests produce one consistent cancellation outcome and no duplicate refund boundary or external cancellation action.
- **SC-004**: Operators can identify the actor category, policy result, and cancellation mode for 100% of cancellations created after this feature is released.
- **SC-005**: In usability testing, at least 90% of operators can complete an authorized policy-override cancellation without incorrectly interpreting a permission error as a policy error.
- **SC-006**: No existing cancellation workflow loses its established separation between cancellation, invoice handling, and refund decision as measured by the regression test suite for customer, owner/admin, immediate, period-end, paid, and unpaid cases.

## Assumptions

- Owner/admin means an authenticated owner or an administrator with the existing organization-level permission to manage the relevant booking or subscription; this feature does not create a new role hierarchy.
- The relevant organization is the product-owning coworking space or host organization; membership in another organization involved in the booking does not grant override authority.
- The feature applies to marketplace bookings and marketplace subscriptions in coworking Spaces and Host organizations; unrelated invitation, team, or non-marketplace cancellation flows are out of scope.
- Existing refund, accounting-invoice, payment-workflow, resource cleanup, and subscription lifecycle rules remain authoritative unless a separate requirement explicitly changes them.
- Administrative cancellations may still require a separate refund decision, including no refund, partial refund, manual review, or provider reconciliation.
- Existing authenticated sessions and organization authorization services provide the identity and permission evidence needed by the cancellation decision.
