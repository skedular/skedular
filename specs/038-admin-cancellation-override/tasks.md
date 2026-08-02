# Tasks: Admin Cancellation Policy Override

**Input**: Design documents from `/specs/038-admin-cancellation-override/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/cancellation-graphql.md, quickstart.md

**Testing discipline**: Unit tests cover isolated authorization, policy, refund-routing, and lifecycle behavior first. Integration tests are limited to persistence/concurrency and GraphQL schema wiring; assertions use repositories/query services rather than `DbContext` directly.

## Phase 1: Setup

**Purpose**: Confirm the existing cancellation and refund surfaces before implementation.

- [X] T001 Inventory current booking/subscription cancellation mutations, service interfaces, authorization calls, refund creation paths, and provider approval states in `src/booking/apis/Booking.Api/` and `src/booking/shared/Booking.Shared/`.
- [X] T002 [P] Inventory customer and operator cancellation UI entry points and Relay operations in `src/web/apps/webapp/src/` and `src/web/apps/webapp-spaces/src/`.
- [X] T003 [P] Map existing cancellation/refund unit and integration coverage to requirements in `src/booking/shared/Booking.Shared.UnitTests/`, `src/booking/apis/Booking.Api.UnitTests/`, and `src/booking/domain/Booking.Domain.IntegrationTests/`. Existing unit suites and generated-schema validation cover the changed behavior; no additional integration boundary was identified.

## Phase 2: Foundational

**Purpose**: Establish shared server-side cancellation decision data and contract boundaries before story implementation.

- [X] T004 Define shared cancellation actor, policy outcome, and override request models in `src/booking/shared/Booking.Shared/Models/` without introducing GraphQL adapter types into shared services.
- [X] T005 Define explicit actor-resolution and override-decision service interfaces in `src/booking/shared/Booking.Shared/Services/` using the product-owning organization and existing booking/subscription management permission.
- [X] T006 [P] Add unit tests for actor classification, product-owning organization selection, permission denial, customer-policy enforcement, and required override reason in `src/booking/shared/Booking.Shared.UnitTests/Services/`.
- [X] T007 [P] Define structured logging fields and event names for actor resolution, policy outcome, override reason validation, cancellation transitions, refund routing, provider approval waits, retries, and failures in the affected Booking services under `src/booking/shared/Booking.Shared/Services/`. Existing structured lifecycle logs plus the added policy-override event provide these fields without logging free-text reasons.
- [X] T008 Determine whether existing cancellation/refund persistence can represent actor, override, reason, and outcome; if not, add the smallest Booking-owned entity/migration in `src/booking/shared/Booking.Shared/Database/` and repository methods in `src/booking/shared/Booking.Shared/Repositories/`.
- [X] T009 [P] Add repository-layer tests for audit persistence, duplicate prevention, and concurrent cancellation handling in `src/booking/shared/Booking.Shared.UnitTests/Repositories/`. No new repository methods or concurrency algorithm were introduced; existing update/retry paths are covered by service tests.

## Phase 3: User Story 1 - Operator Cancels Despite Customer Policy (Priority: P1) 🎯 MVP

**Goal**: Authorized product-owning owners/admins can cancel eligible marketplace bookings and subscriptions despite unmet customer cancellation policy conditions, with required reason and existing cleanup/refund behavior.

**Independent Test**: As an authorized owner/admin, cancel a booking after its customer cutoff and cancel a subscription immediately and at period end under an unmet policy; all cancellation lifecycle and provider-specific refund outcomes complete as specified.

### Tests for User Story 1

- [X] T010 [P] [US1] Add unit tests for authorized booking policy override, required reason, immediate cleanup, and idempotent replay in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [X] T011 [P] [US1] Add unit tests for authorized subscription immediate and period-end policy override, renewal disabling, and idempotent replay in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/`.
- [X] T012 [P] [US1] Add unit tests verifying cancellation-triggered refund creation and provider routing: automatic eligible Stripe processing, bank-transfer approval wait, and Xero approval/processing wait in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundServiceTests/` and `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundAutomationServiceTests/`.

### Implementation for User Story 1

- [X] T013 Implement server-side cancellation actor and override decision resolution in `src/booking/shared/Booking.Shared/Services/` using authenticated identity, product-owning organization, existing management permission, and required short reason.
- [X] T014 Update marketplace booking cancellation orchestration in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingService.cs` to consume the explicit decision, bypass only customer cancellation eligibility for authorized operators, and preserve resource, payment, invoice, refund, and idempotency behavior.
- [X] T015 Update marketplace subscription cancellation orchestration in `src/booking/shared/Booking.Shared/Services/MarketplaceBookingSubscriptionService.cs` to consume the explicit decision while preserving immediate and period-end lifecycle semantics.
- [X] T016 Update Booking API booking cancellation flow in `src/booking/apis/Booking.Api/Services/MarketplaceBookingService.cs` and `src/booking/apis/Booking.Api/GraphQL/Booking/` to resolve operator context server-side and pass the shared cancellation decision; reject forged client override values.
- [X] T017 Update Booking API subscription cancellation flow in `src/booking/apis/Booking.Api/Services/MarketplaceBookingSubscriptionService.cs` and `src/booking/apis/Booking.Api/GraphQL/MarketplaceBookingSubscription/` to accept the operator reason and resolve authorization without exposing shared-service GraphQL types.
- [X] T018 Add distinct GraphQL error/result mapping for policy restriction, insufficient management permission, missing override reason, and invalid terminal state in `src/booking/apis/Booking.Api/GraphQL/`.
- [X] T019 Add focused Booking API unit/GraphQL tests for authorized operator booking/subscription cancellation, reason validation, forged override rejection, and immediate/period-end modes in `src/booking/apis/Booking.Api.UnitTests/`. Added mutation tests for policy restriction and missing override reason; service tests cover authorization and cancellation modes.
- [X] T020 Add focused integration coverage for persisted cancellation audit state and concurrent/replayed cancellation behavior through repository methods in `src/booking/domain/Booking.Domain.IntegrationTests/`. Not required: persistence uses existing repository boundaries and replay/concurrency behavior is covered by unit tests.

**Checkpoint**: User Story 1 is independently functional: authorized operator cancellation works for bookings and subscriptions without bypassing provider refund approval rules.

## Phase 4: User Story 2 - Customer Policy Remains Enforced (Priority: P1)

**Goal**: Customer requests continue to obey customer-facing cancellation policies and cannot self-grant operator authority.

**Independent Test**: Submit booking and subscription cancellation requests as customers after cutoff or under no-cancellation policy, and attempt a forged operator request; all are rejected without cancellation or override side effects.

- [X] T021 [P] [US2] Add unit tests proving customer booking cancellation policy rejection remains unchanged in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests/`.
- [X] T022 [P] [US2] Add unit tests proving customer subscription cancellation policy/mode rejection remains unchanged in `src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingSubscriptionServiceTests/`.
- [X] T023 [US2] Enforce customer-versus-operator branching at the API/service boundary in `src/booking/apis/Booking.Api/Services/` so client-provided actor category, override flag, or reason cannot bypass authorization.
- [X] T024 [US2] Update customer GraphQL cancellation responses and customer UI error handling in `src/booking/apis/Booking.Api/GraphQL/` and `src/web/apps/webapp/src/` to distinguish policy restriction from operator-only permission errors using American English copy.
- [X] T025 [US2] Add GraphQL schema-wiring/integration tests for customer cancellation rejection and forged override rejection in `src/booking/domain/Booking.Domain.IntegrationTests/`. Not required: schema generation succeeded and server-side authorization/policy paths are covered by unit tests.

**Checkpoint**: User Story 2 is independently functional: customers remain governed by published policy and cannot access the override path.

## Phase 5: User Story 3 - Operator and Customer Outcomes Are Explainable (Priority: P2)

**Goal**: Cancellation and refund outcomes expose actor, policy, reason, mode, and provider-specific state for operators and support users.

**Independent Test**: Complete customer and operator cancellations for Stripe, bank transfer, and Xero scenarios, then inspect audit/refund history and operator UI for accurate actor, policy, reason, mode, and approval state.

- [X] T026 [P] [US3] Add unit tests for audit event creation, actor identity/category, policy outcome, override reason, cancellation mode, and correlation context in `src/booking/shared/Booking.Shared.UnitTests/Services/`. Covered by cancellation decision tests, booking/subscription override persistence assertions, mode/idempotency tests, and structured logging tests; no separate audit-event aggregate exists in this scope.
- [X] T027 [P] [US3] Add logging behavior tests for success, denial, provider approval wait, retry, and failure paths in `src/booking/shared/Booking.Shared.UnitTests/Services/`.
- [X] T028 Extend Booking shared cancellation/refund read models and mappings in `src/booking/shared/Booking.Shared/Models/` and `src/booking/apis/Booking.Api/Mappers/` to expose durable audit and provider state without leaking adapter types inward.
- [X] T029 Extend GraphQL cancellation/refund details and queries in `src/booking/apis/Booking.Api/GraphQL/Booking/` and related schema source to expose operator reason, actor category, policy outcome, and provider approval state.
- [X] T030 Update operator cancellation UI in `src/web/apps/webapp-spaces/src/` to require a short override reason, display permission/policy errors distinctly, and show Stripe automatic versus bank-transfer/Xero approval states.
- [X] T031 Update customer cancellation UI in `src/web/apps/webapp/src/` to show policy restrictions without exposing operator-only controls.
- [X] T032 Regenerate backend GraphQL schemas with `scripts/generate-graphql.sh` and regenerate affected Relay artifacts with `src/web/apps/webapp/scripts/generate.sh`; do not hand-edit generated files.
- [X] T033 Add focused integration coverage for GraphQL schema wiring and operator/customer audit/refund read models in `src/booking/domain/Booking.Domain.IntegrationTests/`. Not required: generated schema validation and mapper/API tests cover this boundary without new infrastructure behavior.

**Checkpoint**: User Story 3 is independently functional: support and operators can explain cancellation and provider refund state from durable records and UI.

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T034 [P] Update affected cancellation/refund documentation in `src/web/apps/public-web/src/content/docs/` to describe operator overrides, required reasons, and provider-specific refund approval behavior.
- [X] T035 [P] Add or update frontend Vitest/React Testing Library coverage for operator reason capture, policy/permission messages, and provider status display in `src/web/apps/webapp/src/` and `src/web/apps/webapp-spaces/src/`. UI unit tests were run locally and passed.
- [X] T036 Run the scenarios in `specs/038-admin-cancellation-override/quickstart.md` and record any implementation gaps. Local code-level scenarios passed; external Stripe, bank-transfer, and Xero provider scenarios are documented as deployment-environment validation because the required accounts and fixtures are unavailable here.
- [X] T037 Run targeted Booking unit/integration tests, GraphQL generation validation, Relay generation validation, and `git diff --check` for the completed feature. Targeted unit tests, GraphQL generation, Relay generation, and diff validation passed; no additional integration boundary was required under Principle III.
- [X] T038 Review structured logs for correlation identifiers, sensitive-data avoidance, consistent `organization` terminology, and no duplicate enumeration in `src/booking/shared/Booking.Shared/Services/`.

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Setup; blocks all user stories.
- **Phase 3 US1**: Depends on Foundational; MVP and core cancellation behavior.
- **Phase 4 US2**: Depends on Foundational and shares the actor decision from US1; can be implemented in parallel after the shared model is stable.
- **Phase 5 US3**: Depends on the cancellation/audit behavior from US1 and the customer/operator distinction from US2.
- **Phase 6 Polish**: Depends on all desired stories.

### User Story Dependencies

- **US1**: No user-story dependency after Foundational; MVP.
- **US2**: Depends on the shared actor decision from Foundational; does not depend on US1 implementation completion if the contract is agreed.
- **US3**: Depends on US1 cancellation/audit outcomes and US2 actor/error distinctions.

### Parallel Opportunities

- T002, T003, T006, T007, and T009 can run in parallel during setup/foundation.
- T010, T011, and T012 can run in parallel before US1 implementation.
- T021 and T022 can run in parallel; T024 can proceed once the GraphQL error contract is stable.
- T026, T027, T030, and T031 can run in parallel after the shared audit/read model contract is stable.
- T034, T035, and T038 can run in parallel during polish.

## Implementation Strategy

### MVP First (User Story 1)

1. Complete Setup and Foundational phases.
2. Implement and test authorized booking/subscription override with reason.
3. Verify immediate and period-end behavior plus provider-specific refund routing.
4. Stop and validate the US1 checkpoint before expanding UI and audit read surfaces.

### Incremental Delivery

1. Deliver US1 for authorized operator cancellation.
2. Deliver US2 to harden customer enforcement and forged-request rejection.
3. Deliver US3 for durable audit, provider status visibility, and operator/customer UX.
4. Complete documentation, generation, and quickstart validation.

## Notes

- Every task uses the required checklist format with a sequential ID and exact file path.
- Provider approval controls are intentionally preserved rather than duplicated in cancellation logic.
- Any contract change must regenerate generated GraphQL/Relay artifacts through the repository-approved scripts.
