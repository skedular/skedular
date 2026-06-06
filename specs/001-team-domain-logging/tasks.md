# Tasks: Team Domain Structured Logging

**Input**: Design documents from `/specs/001-team-domain-logging/`
**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Include unit and targeted integration test tasks because the specification requires test updates (FR-008, FR-009) and behaviour verification for logging outcomes.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Establish implementation scaffolding for the logging rollout and test execution.

- [x] T001 Document the final in-scope component matrix in `specs/001-team-domain-logging/contracts/logging-observability-contract.yaml`
- [x] T002 [P] Add feature execution notes and verification sequence to `specs/001-team-domain-logging/quickstart.md`
- [x] T003 [P] Prepare focused Team-domain test run commands and expected checkpoints in `specs/001-team-domain-logging/quickstart.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Cross-cutting prerequisites that MUST complete before user-story implementation.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [x] T004 Confirm host logging bootstrap remains unchanged and explicitly verified in `team/apis/Team.Api/Program.cs`, `team/jobs/Team.Jobs/Program.cs`, and `team/processors/Team.Processors/Program.cs`
- [x] T005 [P] Add shared safe-log property guidance comments/examples in `specs/001-team-domain-logging/data-model.md`
- [x] T006 [P] Add shared test-fixture guidance for logger dependency injection in `team/apis/Team.Api.UnitTests/GlobalUsings.cs` and `team/shared/Team.Shared.UnitTests/GlobalUsings.cs`
- [x] T007 Define logging verification coverage checklist for story acceptance in `specs/001-team-domain-logging/contracts/logging-observability-contract.yaml`

**Checkpoint**: Foundation ready; story implementation can begin.

---

## Phase 3: User Story 1 - Operational traceability for team and member mutations (Priority: P1) 🎯 MVP

**Goal**: Add structured logging for core team/member mutation and authorization flows.

**Independent Test**: Execute Team.Api unit/integration tests and verify mutation methods log safe structured outcomes with expected levels.

### Tests for User Story 1

- [x] T008 [P] [US1] Add/extend unit tests for `TeamService` mutation logging in `team/apis/Team.Api.UnitTests/Services/TeamServiceTests/AddShould.cs` and `team/apis/Team.Api.UnitTests/Services/TeamServiceTests/DeleteShould.cs`
- [x] T009 [P] [US1] Add/extend unit tests for `TeamMemberService` mutation logging in `team/apis/Team.Api.UnitTests/Services/TeamMemberServiceTests/ChangeRoleShould.cs` and `team/apis/Team.Api.UnitTests/Services/TeamMemberServiceTests/ChangeStatusShould.cs`
- [x] T010 [P] [US1] Add/extend unit tests for authorization decision log levels in `team/apis/Team.Api.UnitTests/Services/Authorization/TeamAuthorizationServiceTests/AuthorizeShould.cs` and `team/apis/Team.Api.UnitTests/Services/Authorization/OrganizationAuthorizationServiceTests/AuthorizeShould.cs`
- [X] T011 [US1] _Note: Domain-level integration tests are handled separately, not in individual projects_

### Implementation for User Story 1

- [x] T012 [P] [US1] Inject `ILogger<TeamService>` and add structured mutation/read-outcome logs in `team/apis/Team.Api/Services/TeamService.cs`
- [x] T013 [P] [US1] Inject `ILogger<TeamMemberService>` and add structured mutation/read-outcome logs in `team/apis/Team.Api/Services/TeamMemberService.cs`
- [x] T014 [P] [US1] Inject `ILogger<CustomerService>` and add structured read/mutation-outcome logs in `team/apis/Team.Api/Services/CustomerService.cs`
- [x] T015 [P] [US1] Inject `ILogger<WorkaroundService>` and add structured request-outcome logs in `team/apis/Team.Api/Services/WorkaroundService.cs`
- [x] T016 [P] [US1] Inject loggers and apply denied/granted level policy in `team/apis/Team.Api/Services/Authorization/OrganizationAuthorizationService.cs` and `team/apis/Team.Api/Services/Authorization/TeamAuthorizationService.cs`
- [x] T017 [P] [US1] Inject loggers and apply denied/granted level policy in `team/apis/Team.Api/Services/Authorization/OrganizationOfferingService.cs` and `team/apis/Team.Api/Services/Authorization/OrganizationSsoAuthorizationService.cs`
- [x] T018 [US1] Inject `ILogger<TeamGrpcService>` and add structured request handling logs in `team/apis/Team.Api/Grpc/TeamGrpcService.cs`
- [x] T019 [US1] Update Team.Api tests for new logger constructor dependencies using AutoFixture/FakeItEasy patterns in `team/apis/Team.Api.UnitTests/GlobalUsings.cs`

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - Traceability for invitation lifecycle (Priority: P2)

**Goal**: Add structured logging for invitation lifecycle, workflow dispatch, and related activities.

**Independent Test**: Execute invitation/service and temporal outbox tests, verifying lifecycle outcomes are logged with safe properties and correct levels.

### Tests for User Story 2

- [x] T020 [P] [US2] Add/extend `InvitationService` lifecycle logging unit tests in `team/apis/Team.Api.UnitTests/Services/InvitationServiceTests/InviteMembersByEmailsShould.cs`, `team/apis/Team.Api.UnitTests/Services/InvitationServiceTests/AcceptInvitationShould.cs`, and `team/apis/Team.Api.UnitTests/Services/InvitationServiceTests/CancelInvitationShould.cs`
- [x] T021 [P] [US2] Add/extend temporal outbox logging unit tests in `team/shared/Team.Shared.UnitTests/Services/TemporalOutboxServiceTests/StartWorkflowInviteToJoinShould.cs` and `team/shared/Team.Shared.UnitTests/Services/TemporalOutboxServiceTests/SignalWorkflowInviteToJoinInvitationStatusChangedShould.cs`
- [X] T022 [US2] _Note: Domain-level integration tests are handled separately, not in individual projects_

### Implementation for User Story 2

- [x] T023 [US2] Inject `ILogger<InvitationService>` and add structured lifecycle logs in `team/apis/Team.Api/Services/InvitationService.cs`
- [x] T024 [P] [US2] Inject `ILogger<TemporalOutboxService>` and add workflow enqueue/log outcome entries in `team/shared/Team.Shared/Services/TemporalOutboxService.cs`
- [x] T025 [P] [US2] Inject `ILogger<InvitationIntegrations>` and add activity dispatch/outcome logs in `team/shared/Team.Shared/Activities/InvitationIntegrations.cs`
- [x] T026 [P] [US2] Inject `ILogger<EmailIntegrations>` and add activity dispatch/outcome logs in `team/shared/Team.Shared/Activities/EmailIntegrations.cs`
- [x] T027 [US2] Update Team.Shared unit tests for logger constructor dependencies in `team/shared/Team.Shared.UnitTests/GlobalUsings.cs`

**Checkpoint**: User Story 2 is independently functional and testable.

---

## Phase 5: User Story 3 - Processor subscriber observability (Priority: P2)

**Goal**: Bring `CustomerSubscriber` logging coverage to parity with existing subscriber patterns.

**Independent Test**: Execute processor subscriber tests and verify stale-event and successful-event handling logs are emitted at expected levels.

### Tests for User Story 3

- [x] T028 [P] [US3] Add unit tests for stale and successful customer event logging in `team/processors/Team.Processors.UnitTests/Subscribers/CustomerSubscriberTests/HandleAsyncShould.cs`
- [X] T029 [US3] _Note: Domain-level integration tests are handled separately, not in individual projects_

### Implementation for User Story 3

- [x] T030 [US3] Inject `ILogger<CustomerSubscriber>` and implement stale/success structured logging in `team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T031 [US3] Align logger-safe property usage with existing subscriber patterns in `team/processors/Team.Processors/Subscribers/LocationSubscriber.cs` and `team/processors/Team.Processors/Subscribers/OrganizationSubscriber.cs`

**Checkpoint**: User Story 3 is independently functional and testable.

---

## Phase 6: User Story 4 - Cache-layer and publisher observability (Priority: P3)

**Goal**: Add low-noise cache and publisher observability with safe structured properties.

**Independent Test**: Execute shared service tests and verify cache miss/eviction and publish operations emit expected logs.

### Tests for User Story 4

- [x] T032 [P] [US4] Add unit tests for cache miss/eviction debug logging in `team/shared/Team.Shared.UnitTests/Services/Cache/CachedTeamServiceTests/RemoveByIdShould.cs`, `team/shared/Team.Shared.UnitTests/Services/Cache/CachedOrganizationServiceTests/RemoveByIdOrCustomDomainShould.cs`, and `team/shared/Team.Shared.UnitTests/Services/Cache/CachedCustomerServiceTests/RemoveShould.cs`
- [x] T033 [P] [US4] Add unit tests for publish count/type logging in `team/shared/Team.Shared.UnitTests/Publishers/TeamOutboxPublisherTests/PublishTeamsShould.cs`

### Implementation for User Story 4

- [x] T034 [P] [US4] Inject `ILogger<CachedTeamService>` and add cache miss/eviction debug logs in `team/shared/Team.Shared/Services/Cache/CachedTeamService.cs`
- [x] T035 [P] [US4] Inject `ILogger<CachedOrganizationService>` and add cache miss/eviction debug logs in `team/shared/Team.Shared/Services/Cache/CachedOrganizationService.cs`
- [x] T036 [P] [US4] Inject `ILogger<CachedCustomerService>` and add cache miss/eviction debug logs in `team/shared/Team.Shared/Services/Cache/CachedCustomerService.cs`
- [x] T037 [P] [US4] Inject `ILogger<TeamOutboxPublisher>` and add publish outcome logs in `team/shared/Team.Shared/Publishers/TeamOutboxPublisher.cs`
- [x] T038 [US4] Decide active usage and either add equivalent logging or document exclusion for `TeamPublisher` in `team/shared/Team.Shared/Publishers/TeamPublisher.cs` and `specs/001-team-domain-logging/contracts/logging-observability-contract.yaml`

**Checkpoint**: User Story 4 is independently functional and testable.

---

## Phase 7: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency, safety, and validation across all stories.

- [x] T039 [P] Run full targeted test suites and capture results in `specs/001-team-domain-logging/quickstart.md`
- [x] T040 [P] Perform secret-safety review of all new log templates/properties in `team/apis/Team.Api/Services/`, `team/processors/Team.Processors/Subscribers/`, and `team/shared/Team.Shared/`
- [x] T041 [P] Verify exception logging-before-rethrow policy across touched files in `team/apis/Team.Api/Services/`, `team/shared/Team.Shared/Services/`, and `team/processors/Team.Processors/Subscribers/`
- [x] T042 Update final implementation coverage and sign-off checklist in `specs/001-team-domain-logging/contracts/logging-observability-contract.yaml`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies.
- **Phase 2 (Foundational)**: Depends on Phase 1; blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2; defines MVP.
- **Phase 4 (US2)**: Depends on Phase 2; can run in parallel with US3/US4 after Phase 2.
- **Phase 5 (US3)**: Depends on Phase 2; can run in parallel with US2/US4 after Phase 2.
- **Phase 6 (US4)**: Depends on Phase 2; can run in parallel with US2/US3 after Phase 2.
- **Phase 7 (Polish)**: Depends on completion of Phases 3–6.

### User Story Dependencies

- **US1 (P1)**: Independent after foundational phase; recommended first delivery (MVP).
- **US2 (P2)**: Independent of US1 except shared logging conventions from Phase 2.
- **US3 (P2)**: Independent of US1/US2 except shared logging conventions from Phase 2.
- **US4 (P3)**: Independent of US1/US2/US3 except shared logging conventions from Phase 2.

### Recommended Delivery Order

1. Complete Phases 1-2.
2. Deliver **US1** as MVP.
3. Deliver **US2**, **US3**, and **US4** in parallel (team-capacity permitting).
4. Complete Phase 7 validation and sign-off.

---

## Parallel Execution Examples

### US1

```bash
# Parallel unit tests for team/member and authorization services
Task: T008
Task: T009
Task: T010

# Parallel implementation on separate files
Task: T012
Task: T013
Task: T014
Task: T015
Task: T016
Task: T017
```

### US2

```bash
# Parallel activity/workflow service implementation
Task: T024
Task: T025
Task: T026

# Parallel test additions
Task: T020
Task: T021
```

### US3

```bash
# Parallel test + implementation (separate projects/files)
Task: T028
Task: T030
```

### US4

```bash
# Parallel cache service updates
Task: T034
Task: T035
Task: T036

# Parallel publisher update and tests
Task: T033
Task: T037
```

---

## Implementation Strategy

### MVP First (User Story 1)

Implement and validate US1 first to deliver immediate operational traceability for the highest-risk
team/member mutations and authorization outcomes.

### Incremental Delivery

1. Foundation and shared conventions (Phases 1-2)
2. MVP operational traceability (Phase 3)
3. Invitation lifecycle observability (Phase 4)
4. Processor subscriber parity (Phase 5)
5. Cache and publisher observability (Phase 6)
6. Cross-cutting hardening and validation (Phase 7)

### Validation Gates

- After US1: Team mutation and authorization logs are present, safe, and tested.
- After US2/US3/US4: Each story independently passes targeted tests.
- Final: All targeted Team unit/integration suites pass; secret-safety and exception-log policy verified.
