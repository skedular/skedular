# Feature Specification: Modify Marketplace Bookings

**Feature Branch**: `040-modify-marketplace-bookings`
**Created**: 2026-08-07
**Status**: Draft
**Input**: Enable customers and authorized organization staff to change the dates and selected resources of completed marketplace purchases, including subscription-created bookings, while retaining the purchased product and resource entitlement.

## Clarifications

### Session 2026-08-07

- Q: When moving a subscription-created booking, must the new date remain within that occurrence’s current subscription cycle? → A: Keep the replacement date within the same current subscription cycle.
- Q: Which payment states should permit a customer or operator to change a marketplace booking? → A: Only confirmed or no-payment-required bookings may be changed.
- Q: Should a confirmed future booking be changeable until it starts, or should the purchased offer’s cancellation cutoff also block date/resource changes? → A: Allow changes until the booking starts; do not use cancellation cutoffs.
- Q: When an authorized operator changes a customer’s booking, how should the customer be informed? → A: Notify the customer for every operator-made change.
- Q: Should every booking modification require a reason? → A: Yes. Require a reason for both customer and authorized operator changes.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Reschedule a marketplace booking (Priority: P1)

A customer who has purchased a future marketplace product can change the date and time of that booking from their customer-facing purchase view. They can select only options that remain valid for the product they bought and that can be fulfilled with the purchased resource entitlement.

**Why this priority**: This is the core self-service outcome: customers can correct or move an upcoming reservation without cancelling a valid purchase and starting again.

**Independent Test**: A customer with a confirmed future marketplace booking selects an eligible alternative time, confirms it, and sees the updated reservation and assigned resources in their booking list and details.

**Acceptance Scenarios**:

1. **Given** a customer has a future marketplace booking with confirmed or no-payment-required status, **When** they select an available eligible date and time and confirm the change, **Then** the booking moves to that time without changing its purchased product, price, quantity, or payment status.
3. **Given** no eligible resource can satisfy the requested time, **When** the customer attempts to confirm the change, **Then** the original booking remains unchanged and the customer receives a clear availability explanation.

---

### User Story 2 - Modify a booking for a customer (Priority: P1)

An authorized coworking-space owner, host owner, or administrator can change an eligible marketplace booking on behalf of the customer from the appropriate organization-facing booking experience. The customer remains the booking participant and can see the revised result in their own view.

**Why this priority**: Operators need to resolve customer service and availability issues without requiring the customer to repeat the action.

**Independent Test**: An authorized organization user changes a customer's eligible upcoming booking and the change is visible to both the operator and the customer; an unauthorized user cannot initiate or complete it.

**Acceptance Scenarios**:

1. **Given** an organization user has permission to manage the booking, **When** they provide a reason and move it to an eligible time, **Then** the system records the same booking with its original purchaser and commercial terms intact.
2. **Given** an organization user lacks the required permission, **When** they attempt to modify a marketplace booking, **Then** the action is unavailable or rejected without exposing customer booking details beyond their existing access.
3. **Given** an organization user successfully changes a customer's booking, **When** the change is completed, **Then** the customer receives a booking-change notification and can see the current date, time, and resource assignment in their booking details.

---

### User Story 3 - Choose different resources (Priority: P1)

A customer or authorized operator can replace the resources assigned to an eligible future marketplace booking, either while changing its date or while retaining its date. They can choose from the available resource types and individual resources included by the purchased product, up to the number covered by that booking.

**Why this priority**: Customers must be able to correct an unsuitable initial allocation rather than treating an assigned resource as permanently fixed.

**Independent Test**: An eligible actor opens a future marketplace booking, selects a different eligible available resource set within the purchased limit, confirms it, and sees the replacement resources without a new charge or a change to the original purchase.

**Acceptance Scenarios**:

1. **Given** two or more eligible resource types are available to the purchased product, **When** the actor selects a different available type, **Then** the system accepts it when the selected resource remains within the product's entitlement.
2. **Given** the booking covers more than one resource, **When** the actor selects a replacement set at or below the allowed resource count, **Then** the system accepts the valid selection and preserves the commercial terms.
3. **Given** the actor selects too many resources, an ineligible resource, or a resource that became unavailable, **When** they confirm the change, **Then** the system explains the issue and retains the original reservation unchanged.

---

### User Story 4 - Move one subscription occurrence (Priority: P2)

A customer or authorized operator can change one eligible future booking generated by a marketplace subscription without silently changing the subscription's purchased pricing, renewal, or normal recurring schedule.

**Why this priority**: Subscription customers need occasional flexibility while their purchased recurring access remains predictable.

**Independent Test**: A customer moves one future subscription occurrence; that occurrence remains at the new eligible time while later occurrences continue to follow the subscription's established schedule.

**Acceptance Scenarios**:

1. **Given** a future booking belongs to an active marketplace subscription, **When** an eligible actor moves that occurrence to an eligible date within its current subscription cycle, **Then** only that occurrence changes and the subscription's subsequent planned occurrences are not changed.
2. **Given** a moved subscription occurrence is later reviewed, **When** the subscription's future bookings are displayed, **Then** the moved occurrence is distinguishable from the normal recurring schedule.
3. **Given** a subscription occurrence cannot be fulfilled at the requested time, **When** the actor confirms the request, **Then** the subscription schedule and the original occurrence remain unchanged.

### Edge Cases

- The booking has started, has ended, is cancelled, or is otherwise no longer eligible for a date or resource change; cancellation cutoffs do not independently prevent a change before the booking starts.
- The requested date is outside the purchased offer's permitted booking window, falls on an unavailable day, violates its duration rules, or conflicts with opening hours.
- The request selects more resources than the booking covers, a resource excluded from the purchased product, or an unavailable resource.
- An operator attempts to complete a change without a reason; the system prevents completion and explains that a reason is required.
- Two people attempt to modify the same booking at nearly the same time; only one valid change is applied and the other actor is shown the current result.
- A customer notification cannot be delivered after an operator change; the completed change remains visible in the customer's booking view and is available for later notification recovery.
- The customer has a pending, failed, expired, rejected, or otherwise unconfirmed payment; the system rejects the change and never treats a date or resource change as a new purchase.
- A future subscription occurrence has already been individually changed; later subscription reconciliation must preserve that explicit exception.
- A subscription occurrence is requested outside its current subscription cycle; the system rejects the request without changing the occurrence or the subscription.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST let an eligible customer change the date and time of an eligible future marketplace booking in every relevant customer-facing Scheduler Host and Scheduler Spaces booking view, and select replacement resources wherever the purchased product exposes selectable resource alternatives.
- **FR-002**: The system MUST let an authorized product-owner organization user change the date and time of an eligible customer's marketplace booking on that customer's behalf in the relevant organization-facing booking view, and select replacement resources wherever the purchased product exposes selectable resource alternatives.
- **FR-003**: The system MUST show the change action only when the actor has permission, the booking remains eligible, and payment is confirmed or no payment is required; direct attempts that do not satisfy those conditions MUST be rejected.
- **FR-004**: A date change MUST retain the purchased product, product version, pricing option, quantity, payment state, purchaser, and commercial terms. It MUST NOT create a new purchase, price adjustment, refund, invoice, or subscription renewal.
- **FR-004a**: A confirmed or no-payment-required marketplace booking remains eligible for date and resource changes until its start time, subject to all other eligibility and availability rules. Cancellation and refund cutoffs MUST NOT independently block a change.
- **FR-005**: The system MUST offer and accept only dates, times, durations, days, resource types, and individual resources that are permitted by the purchased offer and can be fulfilled under current availability and opening rules.
- **FR-006**: The system MUST let the actor explicitly choose eligible available resources, including a different eligible resource type, up to the number of resources covered by the booking. It MUST NOT substitute an ineligible resource or require the actor to select a different product.
- **FR-007**: When an actor has not selected a replacement resource, the system MAY retain the existing resource when available or assign an eligible available resource under the booking's established allocation rules.
- **FR-008**: A failed or conflicting change MUST leave the original booking and its resource assignment intact and explain the reason in actionable terms.
- **FR-009**: A successful change MUST be confirmed to the acting user and reflected consistently in customer, organization, booking-detail, and list views.
- **FR-009a**: When an authorized organization user changes a customer's booking, the system MUST notify the customer of the completed change and retain an in-app change record when notification delivery fails.
- **FR-009b**: Every actor MUST provide a non-empty reason before completing a change, including customers and authorized organization users.
- **FR-010**: For a marketplace subscription, a date or resource change to an individual future occurrence MUST remain within that occurrence's current subscription cycle, create an explicit occurrence-level exception, and MUST NOT alter the subscription's pricing, renewal settings, selected recurring days, requested-resource preferences, or other future occurrences.
- **FR-011**: The system MUST preserve an explicit subscription occurrence exception when it maintains later subscription bookings.
- **FR-012**: The system MUST record who changed the booking, when, the original and resulting schedules and resources, the reason when supplied, and whether the change was customer-initiated or performed for a customer by an authorized organization user.
- **FR-013**: The system MUST provide a clear reason when a booking cannot be changed because of status, authorization, product rules, resource eligibility, resource availability, or concurrent changes.
- **FR-014**: The system MUST update the public documentation for booking, resource availability, marketplace products, and subscriptions, plus all relevant Scheduler Spaces and Scheduler Host customer/operator guidance, so it accurately describes date changes, resource-selection availability and limits, eligibility, and subscription-occurrence behavior.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST record the start and completion of each marketplace booking date-change attempt with booking and actor correlation context.
- **LOG-002**: The feature MUST record the decision outcome for eligibility, authorization, explicit resource selection or automatic assignment, and subscription-occurrence handling without logging sensitive customer data.
- **LOG-003**: The feature MUST emit actionable warning or error records when a change is rejected, conflicts with another change, or cannot preserve the original booking safely.
- **LOG-004**: The feature logs MUST include enough context to trace a customer or operator action across booking and subscription maintenance without exposing payment or personal data.
- **LOG-005**: The feature MUST record notification creation, delivery outcome, and recovery-needed status for operator-made changes without exposing message content or sensitive customer data.

### Key Entities _(include if feature involves data)_

- **Marketplace booking**: A customer purchase and its resulting reservation, including the purchased offer, payment state, schedule, participants, and resource allocation.
- **Marketplace subscription**: A recurring marketplace purchase whose individual future reservations normally follow a recurring schedule.
- **Subscription occurrence exception**: A durable record that one subscription-created reservation was intentionally moved and must not be overwritten by normal schedule maintenance.
- **Purchased offer entitlement**: The product, pricing option, permitted cadence, duration, day, quantity, and eligible-resource rules that bound a booking change.
- **Booking change audit record**: The actor, reason, original and resulting schedules and resources, and outcome of a booking-change attempt.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: At least 95% of valid self-service date changes complete successfully on the first attempt without staff intervention.
- **SC-003**: 100% of rejected date changes leave the original reservation, purchased commercial terms, and payment state unchanged.
- **SC-004**: 100% of successful subscription-occurrence changes preserve the subscription's later recurring schedule unless a separate subscription-management action is taken.
- **SC-005**: 100% of completed changes have an attributable audit entry with the before and after schedules.
- **SC-005a**: 100% of operator-made changes create a customer notification or a recoverable in-app notification record.
- **SC-005b**: 100% of completed booking changes include an attributable reason in the booking-change audit record.
- **SC-006**: Customer and operator acceptance testing covers date-only changes in both Scheduler Host and Scheduler Spaces, plus resource-only and combined changes for every marketplace product that exposes selectable resource alternatives and all supported subscription-created bookings.

## Assumptions

- The initial release changes a single future reservation at a time; changing the recurring pattern or moving an entire subscription remains a separate future capability.
- The purchased product version and pricing option, rather than later catalog changes, define the customer's entitlement for a date change.
- Existing booking eligibility, payment, cancellation, availability, opening-hours, and compatible-resource rules remain authoritative and are reused for date changes.
- An actor may select any available resource type and individual resource included in the purchased product, up to the booking's covered resource count; a resource change never changes the product, price, or quantity purchased.
- If an actor does not select replacement resources, the system may retain the original resource when possible or use established automatic allocation rules; it does not promise the original named resource.
- The customer-facing booking hub, organization storefront booking views, and the organization-facing booking experience are all in scope where they expose marketplace bookings for Scheduler Host or Scheduler Spaces.
- Scheduler Host currently books an entire place and manages its underlying resource automatically. It is therefore in scope for date/time change and documentation, but does not expose individual resource selection unless a Host product is subsequently designed to offer alternatives without becoming a different purchased product.
- Marketplace subscriptions are currently supported by Scheduler Spaces. Subscription-occurrence change is in scope there; enabling subscriptions in Scheduler Host is not part of this feature.
- A subscription-created booking can move only to an eligible date within its current subscription cycle; moving access into a different billing or renewal cycle is out of scope.
- Pending, rejected, expired, and otherwise unconfirmed payment states are not eligible for marketplace date or resource changes, even for an authorized organization user.
- Cancellation and refund policy cutoffs do not apply to date or resource changes; an otherwise eligible confirmed booking can be changed until it starts.
- Operator-made changes notify the customer. A notification-delivery failure does not reverse a completed valid booking change; the in-app change record remains available for recovery.
- Every actor must provide a reason for a booking change.
- Product changes, price changes, quantity changes, participant changes, cancellation, refunds, and changing a subscription's recurring pattern or future resource preferences are out of scope for this feature.
