# Tasks: Booking Failure Communications

**Input**: Design documents from `specs/036-booking-failure-notifications/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts](./contracts/booking-failure-notifications.md)

**Tests**: Required by the specification and constitution: unit, repository/integration, workflow/activity, notification/email, web, and concurrency coverage.

**Organization**: Tasks are grouped by user story after the shared booking-domain foundation so each delivered slice has a verifiable outcome.

## Phase 1: Setup

**Purpose**: Establish the feature’s source and test locations without changing behavior.

- [x] T001 Create the booking-failure entity, repository, service, activity, workflow, template, and test file skeletons under `src/booking/shared/Booking.Shared/` and `src/booking/shared/Booking.Shared.UnitTests/`.
- [x] T002 [P] Create the booking-failure GraphQL type/input/resolver and test skeletons under `src/booking/apis/Booking.Api/GraphQL/Booking/` and `src/booking/apis/Booking.Api.UnitTests/GraphQL/Booking/`.
- [x] T003 [P] Create marketplace booking/subscription failure-presentation test skeletons under `src/web/apps/webapp/src/components/marketplaceProductBooking/` and `src/web/apps/webapp/src/components/marketplaceProductSubscription/`.

---

## Phase 2: Foundational Booking Outcome Infrastructure

**Purpose**: Build the shared durable outcome, delivery, and atomic allocation prerequisites that block all stories.

**⚠️ CRITICAL**: Complete this phase before story work; it defines the finalization and idempotency boundary.

- [x] T004 Add `MarketplaceBookingFailure`, `MarketplaceBookingFailureEvent`, and `MarketplaceBookingFailureDelivery` entities/configurations to `src/booking/shared/Booking.Shared/Database/Entities/` and expose their sets from `src/booking/shared/Booking.Shared/Database/BookingDbContext.cs`.
- [x] T005 Add failure category, scope, customer-action, delivery audience/channel/status constants and model mappings in `src/booking/shared/Booking.Shared/Models/` and `src/booking/shared/Booking.Shared/Mappers/`.
- [x] T006 Add booking-owned failure and delivery repository interfaces/implementations, including unique failure-key and failure-recipient-channel lookup methods, in `src/booking/shared/Booking.Shared/Repositories/` and `src/booking/shared/Booking.Shared/Repositories/RepositoryFactory.cs`.
- [x] T007 Generate the Booking EF migration for the failure/event/delivery schema in `src/booking/shared/Booking.Shared/Database/Migrations/` from the entity model; do not hand-edit migration metadata.
- [x] T008 Implement repository-owned complete-set resource-slot claim/release operations using an EF-managed serializable transaction with bounded retry and a typed conflict result—without raw SQL—in `src/booking/shared/Booking.Shared/Repositories/ResourceRepository.cs`.
- [x] T009 Implement `MarketplaceBookingFailureService` finalization, event append, stable failure-key generation, and idempotent delivery-row creation in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingFailureService.cs`.
- [x] T010 Implement recipient resolution and customer/internal email template rendering by extracting/reusing the verified-email and active owner/administrator logic in `src/booking/shared/Booking.Shared/Services/MarketplaceRefundNotificationService.cs` and adding `MarketplaceBookingFailureNotificationService.cs` plus templates under `src/booking/shared/Booking.Shared/EmailTemplates/`.
- [x] T011 Register repositories, failure services, notification service, and workflow/activity dependencies in `src/booking/shared/Booking.Shared/Extensions.cs`.
- [x] T012 Add foundational unit tests for failure-key uniqueness, finalization idempotency, recipient authorization/deduplication, and template category content in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingFailureServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingFailureNotificationServiceTests/`.
- [x] T013 Add repository integration coverage for successful atomic claim, incomplete claim conflict, serializable-retry competing claims, and persisted delivery uniqueness in `src/booking/domain/Booking.Domain.IntegrationTests/Repositories/MarketplaceBookingFailureRepositoryShould.cs`.
- [x] T014 Add structured allocation/finalization logs with correlation, booking/series identifiers, category, and safe retry context in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingFailureService.cs` and `src/booking/shared/Booking.Shared/Repositories/ResourceRepository.cs`.

**Checkpoint**: A failure can be finalized once, audited, and safely queued with no duplicate recipient/channel delivery rows; contested slots cannot be double-claimed.

---

## Phase 3: User Story 1 - Explain a Submitted Availability Failure (Priority: P1) 🎯 MVP

**Goal**: Give the customer a durable availability-specific outcome and a safe rebook action after a submitted booking cannot secure capacity.

**Independent Test**: Submit a booking after capacity changes and verify one retained availability failure, no allocation, a customer-safe typed result, and a fresh-booking action.

### Tests for User Story 1

- [x] T015 [P] [US1] Add `MarketplaceBookingService` unit tests for requested-resource and automatic-assignment conflicts finalized as availability failures in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/MarketplaceBookingFailureShould.cs`.
- [x] T016 [P] [US1] Add GraphQL mutation contract tests for typed booking submission failure versus generic validation/technical errors in `src/booking/apis/Booking.Api.UnitTests/GraphQL/Booking/RootMutationTests/AddMarketplaceBookingFailureShould.cs`.
- [x] T017 [P] [US1] Add integration tests proving a losing submitted booking retains the failure record and no resource association in `src/booking/domain/Booking.Domain.IntegrationTests/Services/MarketplaceBookingAvailabilityFailureShould.cs`.
- [x] T018 [P] [US1] Add marketplace booking form/details UI tests for availability-specific result and rebook link in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-form.test.tsx` and `marketplace-product-booking-details.test.tsx`.

### Implementation for User Story 1

- [x] T019 [US1] Replace the read-then-attach allocation path in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs` with the foundational atomic claim result and finalization path for submitted one-time availability conflicts.
- [x] T020 [US1] Map a finalized immediate availability conflict to the customer-safe mutation outcome in `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and `src/booking/apis/Booking.Api/GraphQL/Booking/RootMutation.cs`.
- [x] T021 [US1] Add failure outcome/details choice types and authorized query mapping in `src/booking/apis/Booking.Api/GraphQL/Booking/` and `src/booking/apis/Booking.Api/Mappers/GraphQlMapper.cs`.
- [x] T022 [US1] Add the returned failure fields to the collocated mutation/query operations and customer booking details/history presentation in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-form.tsx` and `marketplace-product-booking-details.tsx`.
- [x] T023 [US1] Implement the immutable one-time payment-failure/expiry rebook action and category-specific copy in `src/web/apps/webapp/src/components/marketplaceProductBooking/marketplace-product-booking-details.tsx`.
- [x] T024 [US1] Regenerate Booking GraphQL exports and web Relay artifacts with `scripts/generate-graphql.sh`; review and commit only generated outputs produced by the script.
- [x] T025 [US1] Add final customer-result and allocation-conflict logging assertions to `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/MarketplaceBookingFailureShould.cs`.

**Checkpoint**: A customer can independently submit a contested one-time booking, receive a correct durable outcome, and begin a fresh booking without restoring the old capacity.

---

## Phase 4: User Story 2 - Inform Responsible Space Parties (Priority: P1)

**Goal**: Deliver one durable in-app/history notification and email to the customer and authorized Spaces or Host owners/administrators after a finalized availability/payment failure.

**Independent Test**: Finalize a failure, replay dispatch, and verify each eligible recipient/channel has exactly one delivery while an email failure is retryable and does not change the booking outcome.

### Tests for User Story 2

- [x] T026 [P] [US2] Add unit tests for customer, Spaces owner/administrator, and Host owner/administrator recipient resolution plus verified-address filtering in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingFailureNotificationServiceTests/ResolveRecipientsShould.cs`.
- [x] T027 [P] [US2] Add email/template tests for availability, payment-failed, and payment-expired messages and permitted context in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingFailureNotificationServiceTests/RenderShould.cs`.
- [x] T028 [P] [US2] Add retry/idempotency activity tests for one delivery per failure-recipient-channel in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingFailureNotificationIntegrationsTests/DispatchShould.cs`.
- [x] T029 [P] [US2] Add authorized booking-history failure visibility tests for customer, organization stakeholder, and unauthorized caller in `src/booking/apis/Booking.Api.UnitTests/GraphQL/Booking/MarketplaceBookingFailureQueryShould.cs`.

### Implementation for User Story 2

- [x] T030 [US2] Add `MarketplaceBookingFailureNotificationIntegrations` activity to dispatch pending failure deliveries, update the existing delivery row, and log retry/success/failure in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingFailureNotificationIntegrations.cs`.
- [x] T031 [US2] Add an idempotent failure-notification workflow and workflow ID factory method in `src/booking/shared/Booking.Shared/Workflows/NotifyMarketplaceBookingFailure.cs` and `src/booking/shared/Booking.Shared/Services/WorkflowIdService.cs`.
- [x] T032 [US2] Add Temporal outbox start support for the notification workflow in `src/booking/shared/Booking.Shared/Services/TemporalOutboxService.cs` and invoke it only after durable finalization in `MarketplaceBookingFailureService.cs`.
- [x] T033 [US2] Expose retained in-application delivery/outcome state in authorized customer booking history and organization/host booking surfaces through `src/booking/apis/Booking.Api/GraphQL/Booking/RootQuery.cs` and `src/web/apps/webapp/src/components/booking/`.
- [x] T034 [US2] Add organization/host failure cards or badges with only authorized context and category-specific next actions in `src/web/apps/webapp/src/components/booking/bookings/booking-card.tsx` and `src/web/apps/webapp/src/components/booking/myBookings/my-booking-card.tsx`.
- [x] T035 [US2] Regenerate Booking GraphQL exports and Relay artifacts with `scripts/generate-graphql.sh` after the authorized failure-history schema changes.
- [x] T036 [US2] Add structured dispatch/delivery logs and log-behavior tests in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingFailureNotificationIntegrations.cs` and `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingFailureNotificationIntegrationsTests/DispatchShould.cs`.

**Checkpoint**: Each eligible party sees a retained authorized outcome and receives one email; delivery retries do not duplicate communications or alter booking capacity/outcome.

---

## Phase 5: User Story 3 - Preserve a Reliable Conflict Record (Priority: P1)

**Goal**: Make concurrent submission, payment cleanup, initial series, and later recurring-occurrence outcomes final, auditable, and capacity-safe.

**Independent Test**: Replay concurrent allocation, payment expiry, initial series conflict, and recurring reconciliation; verify one terminal outcome per scope, correct release scope, and no duplicate notifications.

### Tests for User Story 3

- [x] T037 [P] [US3] Add concurrent booking integration tests using repository/query assertions for one winner and one availability-failure loser through serializable retry in `src/booking/domain/Booking.Domain.IntegrationTests/Services/MarketplaceBookingConcurrentClaimShould.cs`.
- [x] T038 [P] [US3] Add one-time card and bank-transfer workflow/activity tests preserving payment failure reason while reusing capacity release in `src/booking/shared/Booking.Shared.UnitTests/Activities/BookingIntegrationsTests/ReleaseBookingResourcesAsyncShould.cs` and `src/booking/shared/Booking.Shared.UnitTests/Workflows/PayBookingViaCardShould.cs`.
- [x] T039 [P] [US3] Add recurring card and bank-transfer workflow/activity tests that release only the unpaid cycle and retain subscription configuration in `src/booking/shared/Booking.Shared.UnitTests/Activities/MarketplaceBookingSubscriptionIntegrationsTests/ReleaseRecurringBookingResourcesAsyncShould.cs` and `src/booking/shared/Booking.Shared.UnitTests/Workflows/PayRecurringBookingViaCardShould.cs`.
- [x] T040 [P] [US3] Add initial-series all-or-nothing and later-occurrence availability-failure integration tests in `src/booking/domain/Booking.Domain.IntegrationTests/Activities/MarketplaceBookingSubscriptionIntegrationsShould.cs`.

### Implementation for User Story 3

- [x] T041 [US3] Finalize payment failed/expired outcomes before the existing one-time release path overwrites terminal detail in `src/booking/shared/Booking.Shared/Activities/BookingIntegrations.cs` and `src/booking/shared/Booking.Shared/Workflows/PayBookingViaCard.cs`.
- [x] T042 [US3] Apply the same classified finalization and notification queueing to bank-transfer expiry/rejection in `src/booking/shared/Booking.Shared/Workflows/PayBookingViaBankTransfer.cs` and `src/booking/shared/Booking.Shared/Activities/BookingIntegrations.cs`.
- [x] T043 [US3] Preserve subscription configuration while recording recurring-cycle payment failure and limiting resource release to unpaid-cycle instances in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`, `PayRecurringBookingViaCard.cs`, and `PayRecurringBookingViaBankTransfer.cs`.
- [x] T044 [US3] Add an atomic all-or-nothing initial-series materialization/rollback path with one series failure in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` and `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- [x] T045 [US3] Replace later recurring resource-less shell/skip behavior with one occurrence-level availability finalization and notification queue in `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.
- [x] T046 [US3] Publish the existing GraphQL topic after outcome transitions and add correlation-safe transition/release logs in `src/booking/shared/Booking.Shared/Activities/BookingIntegrations.cs` and `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingSubscriptionIntegrations.cs`.

**Checkpoint**: Concurrent buyers cannot double-book, initial series cannot partially confirm, payment expiry releases exactly the agreed scope, and future occurrence failures are visible/communicated once without cancelling the subscription.

---

## Phase 6: Polish & Cross-Cutting Validation

**Purpose**: Verify generated artifacts, operational behavior, security boundaries, and full quickstart scenarios.

- [x] T047 [P] Verify GraphQL contracts, generated schema artifacts, and Relay artifacts are synchronized by running `scripts/generate-graphql.sh` and reviewing changes under `api-definitions/graphql/` and `src/web/apps/webapp/src/queries/__generated__/`.
- [x] T048 [P] Review all new customer/operator copy and UI imports in `src/web/apps/webapp/src/components/marketplaceProductBooking/`, `marketplaceProductSubscription/`, and `booking/` for American English and `@skedular/ui` typography compliance.
- [x] T049 [P] Run the focused Booking unit, integration, workflow/activity, notification/email, and marketplace web tests specified in `specs/036-booking-failure-notifications/quickstart.md`.
- [x] T051 Review structured logs produced by the feature paths for correlation, no sensitive data leakage, category distinction, and actionability in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingFailureService.cs` and `src/booking/shared/Booking.Shared/Activities/MarketplaceBookingFailureNotificationIntegrations.cs`.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1**: No dependencies.
- **Phase 2**: Depends on Phase 1 and blocks all stories.
- **US1**: Depends on Phase 2; delivers the MVP immediate availability outcome.
- **US2**: Depends on Phase 2 and the finalized outcome contract from US1; its service/activity tests can begin after Phase 2.
- **US3**: Depends on Phase 2 and reuses the failure finalizer from US1/US2; payment/recurrence tests can begin after Phase 2.
- **Phase 6**: Depends on every desired story.

### User Story Dependencies

- **US1**: First delivery slice and MVP.
- **US2**: Adds durable stakeholder/customer delivery to US1 and payment outcomes.
- **US3**: Extends the same outcome/delivery model across concurrency, payment, initial series, and later recurrence.

### Parallel Opportunities

- T002 and T003 can run alongside T001.
- T004/T005 and T010 can begin in parallel; T006–T011 follow their required entities/interfaces.
- T015–T018, T026–T029, and T037–T040 are parallel test preparation tasks once their foundational interfaces are stable.
- T047–T049 can run in parallel after story implementation.

## Parallel Example: User Story 1

```text
T015: Service conflict-finalization unit tests
T016: GraphQL typed-outcome contract tests
T017: Integration test for losing submitted booking
T018: Marketplace UI result/rebook tests
```

## Implementation Strategy

### MVP First

1. Complete Phases 1–2.
2. Complete US1 through T025.
3. Verify one-time contested submission and rebook behavior independently.

### Incremental Delivery

1. Add durable failure finalization and atomic one-time allocation.
2. Add customer/stakeholder delivery and retained authorized history.
3. Extend the same model to payment and subscription/series behavior.
4. Run full generated-artifact and quickstart validation.

## Notes

- Every task uses the required checkbox, sequential ID, story label where applicable, and exact path format.
- Integration tests must inspect persistence through repositories/query surfaces, never direct `DbContext` usage.
- Generated GraphQL schema and Relay output are regenerated, never hand-edited.
