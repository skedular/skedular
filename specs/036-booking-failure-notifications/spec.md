# Feature Specification: Booking Failure Communications

**Feature Branch**: `036-booking-failure-notifications`
**Created**: 2026-07-22
**Status**: Draft
**Input**: Review and improve availability-related marketplace booking failures for Skedular Spaces and Skedular Host, with clear handling and communications for customers, organization owners/administrators, and hosts.

## Current Implementation Review

The public marketplace booking form performs a live availability count before enabling submission. A customer whose chosen time is already unavailable receives a local error message and no booking is submitted. The server independently validates requested resources and can select a compatible resource when the customer did not select one. A rejected date, requested resource, or automatic selection failure returns an operation error before the booking transaction is committed; the public form currently displays that error as a generic "We couldn't complete this booking" message. These rejected attempts are not retained as a failed booking request, and no customer, owner, administrator, or host notification/email is produced.

For a successful immediate submission, the booking, its marketplace-payment record, slot assignments, and payment-workflow request are committed together. The booking initially has a pending payment state, so it can be viewed later by its parties. Payment expiry and payment updates are already persisted and exposed to booking details, but they are distinct from an availability conflict.

Marketplace subscriptions are persisted before the daily resource-reconciliation process creates their individual bookings. When an upcoming day has no available date or no viable opening-hours/resource plan, reconciliation can skip that day or create a resource-less booking shell rather than recording a customer-facing availability failure. Existing instance resource repair logs an insufficient-resource condition but does not turn it into a final failed request or send a notification. This behavior means the current multi-day/recurring outcome is neither consistently all-or-nothing nor clearly explained to the buyer or owner.

The existing refund-notification capability is a reusable example of customer and organization email recipients, but it is not invoked for availability failures. No general in-app availability-failure notification or durable notification-delivery record was found in the reviewed booking paths.

## Clarifications

### Session 2026-07-22

- Q: For a confirmed subscription whose future occurrence cannot be allocated, what communication policy should apply? → A: Notify customer and stakeholders once for each failed recurring occurrence as soon as it is final.
- Q: For a Skedular Spaces availability failure, who should receive stakeholder communications? → A: Every active organization owner and administrator who can view the affected booking.
- Q: Beyond availability conflicts, which final booking failures should use the same customer-and-stakeholder communication model? → B: Final payment failures or expiry, with clearly different messaging.
- Q: For a failed or expired subscription payment, which scope should be released? → B: Release the unpaid current-cycle bookings; keep the subscription itself for later recovery/renewal.
- Q: After a one-time booking's payment fails or expires and capacity is released, what should the customer's next action be? → B: Keep the failed booking immutable and direct the customer to start a new booking from current availability.

### Evidence Reviewed

- Public form and pre-submit/customer error behavior: `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-form.tsx`.
- Public mutation boundary: `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and `src/booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs`.
- Booking validation, persistence, payment initiation, and slot assignment: `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs`, `ResourceService.cs`, and `ResourceRepository.cs`.
- Subscription creation and reconciliation: `MarketplaceBookingSubscriptionService.cs`, `MarketplaceBookingSubscriptionIntegrations.cs`, and `BookMarketplaceBookingSubscriptionResources.cs`.
- Existing email-recipient/template pattern: `MarketplaceRefundNotificationService.cs`.

## Scenario Matrix

| Scenario | Current customer feedback | Current notifications/email | Current retained state | Required outcome |
| --- | --- | --- | --- | --- |
| Time is unavailable before the customer submits | Local availability message; submission blocked | None | None | Keep this as a local validation result; do not notify owners/hosts. |
| Selected resource becomes unavailable before submission reaches final validation | Generic submission error | None | None | Show an availability-specific result; retain a failed attempt only when the submission crossed the defined submission boundary. |
| Two customers submit the same availability | Each path independently checks availability; final concurrency outcome is not explicitly guaranteed by the reviewed flow | None | Successful booking may be retained; rejected attempt is discarded | Exactly one final outcome per request; losing submitted request is recorded as an availability failure and communicated once. |
| Availability changes during payment or another submitted asynchronous step | Payment lifecycle is retained; availability-failure handling is not identified | None for availability | Existing booking/payment state | Do not claim success until availability is secured; record and communicate a final availability failure when it cannot be secured. |
| Server cannot allocate required slots | Generic operation error | None | No failed-request record | Persist a final availability-conflict outcome and notify customer plus owner/admin or host. |
| Multi-day/recurring creation cannot allocate all required dates | Dates may be skipped or a resource-less shell may be created | None | Subscription and some generated instances may remain | Use an explicit, consistent series outcome; never silently present unallocated dates as confirmed. |
| Technical or payment failure without an availability conflict | Generic form error or existing payment lifecycle | Existing payment communications may apply | Payment state where applicable | Keep separate reason and message from availability failure. |

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Explain a Submitted Availability Failure (Priority: P1)

As a customer who has submitted a marketplace booking, I need a clear final result when the requested place or resource can no longer be secured, so I know the booking did not complete and can select another option.

**Why this priority**: Customers must not be left believing a contested booking is pending or confirmed.

**Independent Test**: Submit a booking after availability changes and verify a retained failed outcome, an availability-specific result, a customer notification/email, and a route back to alternative availability.

**Acceptance Scenarios**:

1. **Given** a customer submits a booking for availability that is lost before final allocation, **When** final allocation fails, **Then** the customer sees a result that identifies an availability conflict, identifies the affected booking date/time and product or place, and offers a return to alternative availability.
2. **Given** a customer only views an unavailable time or is blocked before submitting, **When** the local availability check fails, **Then** the customer sees a local validation message and no failure record or stakeholder notification is created.
3. **Given** final booking processing fails for a non-availability reason, **When** the outcome is shown, **Then** the message does not characterize it as an availability conflict.

---

### User Story 2 - Inform Responsible Space Parties (Priority: P1)

As a Skedular Spaces or Skedular Host owner or administrator, I need to be told when a submitted customer booking cannot be completed due to availability, so I can understand the customer impact and follow up if needed.

**Why this priority**: Responsible parties need operational visibility without being distracted by ordinary browsing or pre-submit validation.

**Independent Test**: Cause a final availability conflict for a submitted Spaces booking and a Host booking, then verify the correct party receives one in-app notification and one email containing permitted context and a review link.

**Acceptance Scenarios**:

1. **Given** a final availability conflict for a submitted Spaces booking, **When** the failure is recorded, **Then** the customer and the responsible organization owner/administrator receive a single availability-failure notification and email.
2. **Given** a final availability conflict for a submitted Host booking, **When** the failure is recorded, **Then** the customer and every active authorized Host owner and administrator receive a single availability-failure notification and email.
3. **Given** the customer has not crossed the submission boundary, **When** availability is unavailable, **Then** no owner, administrator, or host notification is sent.

---

### User Story 3 - Preserve a Reliable Conflict Record (Priority: P1)

As an operator, I need a durable and explainable record of a final availability failure, so booking history, support, retries, and notifications remain accurate under concurrent requests and retries.

**Why this priority**: A reliable record prevents duplicate charges, duplicate notifications, and ambiguous support outcomes.

**Independent Test**: Submit competing bookings and replay the final-failure processing; verify one winner, one final failed outcome for each losing submitted request, no partial allocation, and no duplicate communications.

**Acceptance Scenarios**:

1. **Given** two submitted requests contend for the same capacity, **When** final allocation is decided, **Then** each request has one terminal outcome and at most one request holds the contested availability.
2. **Given** final-failure processing is retried or replayed, **When** notifications are dispatched, **Then** each recipient/channel receives no more than one notification for the same failure outcome.
3. **Given** a multi-day or recurring request cannot secure every required occurrence, **When** the final outcome is recorded, **Then** the series is not represented as fully confirmed and any retained instances follow the selected series policy.

### Edge Cases

- Availability becomes unavailable after the browser check but before final allocation.
- A request needs several resources and only a subset remains available.
- A selected date becomes disallowed after the customer starts the process.
- The availability check, final allocation, payment initiation, or communication delivery is retried.
- A payment fails or expires independently of availability.
- A recurring series has mixed available and unavailable dates, including a date whose location is closed.
- A customer or stakeholder has no verified email address or has duplicate recipient addresses.
- The booking is cancelled or deleted while failure communication is awaiting delivery.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The system MUST distinguish a local pre-submit availability validation result from a final availability failure of a submitted booking process.
- **FR-002**: The submission boundary MUST be the point at which the system accepts a customer booking or subscription request for processing; browsing, availability queries, invalid form data, and blocked submit actions are outside that boundary.
- **FR-003**: The system MUST revalidate and secure required availability as part of final booking creation; a prior availability result MUST NOT be treated as a reservation.
- **FR-004**: Each submitted booking or booking-series request MUST end in one durable terminal result for the relevant attempt: confirmed, availability failed, payment failed/expired, validation rejected, cancelled, or technical failure where an outcome can be determined.
- **FR-005**: A final availability failure MUST retain the affected product, requested place/resource where applicable, requested date/time or recurrence window, failure timestamp, reason category, and a correlation identifier; it MUST not imply a confirmed booking or reserved capacity.
- **FR-006**: Customers with a final availability failure, payment failure, or payment expiry MUST receive an in-application result, an in-app notification where supported, and an email when a verified deliverable address exists. Each communication MUST identify the applicable failure category, provide affected booking context, and offer a relevant next action; availability communications MUST offer a route to select another option.
- **FR-007**: For Spaces and Host, every active organization owner and administrator authorized to view the affected booking MUST receive one in-app notification and one email for each final submitted availability failure, payment failure, or payment expiry.
- **FR-008**: Stakeholder communications MUST include only information the recipient is authorized to view and, where available, customer, product, place/resource, requested date/time, failure reason, and a link to the retained request or related record.
- **FR-009**: The system MUST use an idempotent failure identity and delivery state so retries, replayed events, and duplicate processing cannot send duplicate in-app notifications or emails.
- **FR-010**: Recording the final booking outcome MUST succeed independently of an individual notification delivery. Failed communications MUST be retriable and visible to operators without changing the recorded booking outcome.
- **FR-011**: Availability notifications MUST be emitted only after the final outcome is known. A provisional conflict, transient lookup failure, or still-recoverable allocation attempt MUST NOT produce a final-failure notification.
- **FR-012**: Payment, validation, technical, and availability failures MUST have distinct reason categories and customer messages; final payment failures and expiry MUST use the customer/stakeholder communication model, while technical failures remain operator-facing unless safely classified for a customer result. Payment handling and charges MUST not be relabeled as availability failures.
- **FR-013**: A multi-day or recurring request MUST use an explicit documented policy. The default policy for this feature is all-or-nothing for the initial submitted request: if every required occurrence cannot be secured, no occurrence is presented as confirmed, allocated occurrences are released, and the customer and stakeholders receive one series-level availability-failure communication.
- **FR-014**: Existing recurring subscriptions that are reconciled after initial confirmation MUST not silently create resource-less or skipped customer-facing occurrences. If a future occurrence cannot be allocated, the system MUST record it as an occurrence-level availability failure, keep the subscription lifecycle accurate, and immediately notify the customer and responsible stakeholders once for that finalized occurrence without repeatedly notifying for the same occurrence.
- **FR-015**: A final payment failure or expiry for a one-time booking MUST release its capacity and retain a visible final payment-failure record. For a subscription, the system MUST release the unpaid current-cycle bookings while preserving the subscription configuration and its eligibility for later recovery or renewal; it MUST not cancel the entire subscription solely because that cycle's payment failed or expired.
- **FR-016**: After one-time capacity is released for a final payment failure or expiry, the failed booking record MUST remain immutable and direct the customer to begin a new booking from current availability. It MUST NOT resume payment or re-reserve capacity through the failed booking record.
- **FR-017**: The system MUST preserve existing successful booking, cancellation, refund, and payment behaviors except where necessary to prevent an availability failure from appearing successful.
- **FR-018**: Authorized customers and stakeholders MUST be able to view retained final availability failures in the relevant booking/request history, with a clear next action; unauthorized users MUST not be able to discover failure details.
- **FR-019**: The system MUST record enough operational history to determine which request won a conflict, which requests failed, whether allocated capacity was released, and whether required communications were delivered or remain retryable.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The feature MUST emit structured records for submission acceptance, final allocation decision, terminal outcome, notification dispatch, retry, and completion.
- **LOG-002**: The feature MUST record the request/failure correlation identifier, booking or series identifier when present, product/organization context, outcome category, and retry decision without exposing unnecessary customer data.
- **LOG-003**: The feature MUST emit actionable warnings for availability conflicts, partial-allocation rollback, skipped recurring occurrence prevention, and delivery failure, and errors for unrecoverable processing failures.
- **LOG-004**: Operators MUST be able to distinguish availability conflicts from payment, validation, and technical failures through recorded reason and logs.

### Key Entities _(include if feature involves data)_

- **Booking submission attempt**: The customer-initiated request after it crosses the submission boundary, linked to a booking or series when one is created.
- **Booking failure outcome**: A durable terminal record containing the categorized reason, requested scope, finalization time, and correlation identity.
- **Availability conflict**: A booking-failure reason indicating required capacity could not be secured during final processing.
- **Communication delivery**: A recipient/channel-specific record used to prevent duplicates and enable safe retry after the final failure is recorded.
- **Booking series outcome**: The parent-level result for a multi-day or recurring request, including its relationship to occurrence-level failures.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: In controlled concurrent-booking tests, 100% of submitted requests finish with one terminal result and no contested availability is confirmed for more than one request.
- **SC-002**: In tests covering final availability failures, payment failures, and payment expiry, 100% of eligible customers and responsible stakeholders receive the required in-app notification and email once per failure outcome, or a visible retryable delivery record exists when delivery is impossible.
- **SC-003**: In usability review, at least 90% of participants correctly identify that an availability-failed booking was not confirmed and can locate the action to choose another option without assistance.
- **SC-004**: In multi-day and recurring failure tests, 100% of unsuccessful initial series requests leave no occurrence presented as confirmed, and 100% of later failed occurrences have an explicit visible state and one immediate customer/stakeholder communication.
- **SC-005**: In payment-failure tests, 100% of one-time failed bookings release capacity while retaining their immutable failure record and directing the customer to make a new booking, and 100% of subscription payment failures release only the unpaid current-cycle bookings while preserving the subscription configuration.
- **SC-006**: Replaying a completed failure process at least five times produces zero duplicate customer or stakeholder communications.

## Assumptions

- The existing authenticated booking histories and notification surfaces are the appropriate places to expose retained failure outcomes; the detailed navigation design will be decided during planning.
- The organization/host recipient rules use existing ownership and administrator authorization relationships: every active authorized Spaces or Host owner and administrator. Only verified and permitted recipient addresses are used for email.
- A communication delivery failure does not reverse a recorded availability failure or block capacity release; it becomes retryable operational work.
- The all-or-nothing rule applies to the initial multi-day/recurring purchase request. Future recurring occurrence failures require an occurrence-level result, one immediate communication to the customer and responsible stakeholders, and must not silently alter the subscription's customer-facing status.
- This feature adds communications for final payment failure and payment expiry but does not change refund eligibility or payment settlement policy; those remain separate financial decisions. A failed subscription payment releases only its unpaid current-cycle bookings and preserves the subscription configuration.
- The planning phase will verify all existing failure channels and choose the smallest data-model extension that preserves the required audit and idempotency behavior.

## Scope and Dependencies

In scope are public marketplace booking and subscription requests for Skedular Spaces and Skedular Host, availability conflicts during final allocation or later reconciliation, final payment failure/expiry communications, failure history, and customer/stakeholder in-app and email communications. Out of scope are ordinary pre-submit browsing alerts, changing pricing or refund policy, and treating generic technical errors as availability conflicts.

The feature depends on the existing booking allocation, booking history, ownership/administrator authorization, email, and notification capabilities. Planning must assess their extension points and ensure the final delivery design honors domain ownership, durable event processing, generated-contract discipline, workflow ID ownership, and proportionate unit, integration, workflow, notification, email, and concurrency tests.
