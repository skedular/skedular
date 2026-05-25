---
description: "Task list for Customer Readiness Tracking implementation"
---

# Tasks: Customer Readiness Tracking

**Input**: Design documents from `/specs/016-customer-readiness-tracking/`
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, contracts/ ✅, quickstart.md ✅

**Tests**: Included — spec.md FR-036 explicitly requires test coverage across all readiness scenarios.

**Organization**: Tasks grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies on incomplete tasks)
- **[Story]**: Which user story this task belongs to (US1–US4)
- Exact file paths included in all descriptions

---

## Phase 1: Setup — Event Contract & Code Generation

**Purpose**: Define the `customer_readiness` protobuf event contract, regenerate outputs, and add the handwritten metadata companion. Contract-first gate — all other phases depend on this contract being in place.

- [x] T001 Add `customer_readiness_v1_key.proto` (Key message with `customerId` field) in `api-definitions/events/skedular/customer_readiness_v1_key.proto`
- [x] T002 [P] Add `customer_readiness_v1_value.proto` (Domain enum, Type enum, Event/Metadata/Data/CustomerIdentityProvisioned messages per contract) in `api-definitions/events/skedular/customer_readiness_v1_value.proto`
- [x] T003 Run `api-definitions/events/generate.sh` to regenerate protobuf C# outputs into `shared/Api.Shared.Clients/obj` (do not check in generated `*V1Key.g.cs` or `*V1Value.g.cs`)
- [x] T004 Add handwritten metadata companion implementing `IEvent`/`IEventMetadata<T>` for `customer_readiness` topic in `shared/Api.Shared.Clients/Events/Skedular/CustomerReadiness/V1/CustomerReadinessMetadata.cs`
- [x] T005 [P] Add unit test verifying `CustomerReadinessMetadata` reports the correct topic name (and partition key shape) in `shared/Api.Shared.Clients.UnitTests/Events/Skedular/CustomerReadiness/V1/CustomerReadinessMetadataShould.cs`
- [x] T005b [P] Add unit test asserting the `Domain` enum contains exactly the eight confirmed participating domains (Booking, Organisation, Team, Marketplace, Location, Core, Slack, MsTeams) and excludes unspecified, unknown, none, and customer values; also assert these values align with the set returned by `RequiredCustomerReadinessDomainService` in `shared/Api.Shared.Clients.UnitTests/Events/Skedular/CustomerReadiness/V1/CustomerReadinessDomainEnumShould.cs`

**Checkpoint**: Event contract compiles, metadata companion confirms `customer_readiness` topic, Domain enum shape is verified — participating-domain and consumer implementation can now begin

---

## Phase 2: Foundational — Customer Domain Persistence & Shared Services

**Purpose**: Core customer-domain persistence layer and shared services that ALL user stories depend on. No user story implementation can begin until this phase is complete.

**⚠️ CRITICAL**: All downstream phases depend on the repository and derivation services defined here.

- [x] T006 Add `CustomerIdentityProvisioningState` entity (customerId, overallStatus, lastUpdatedAt, activatedAt, collection of domain states) in `customer/shared/Customer.Shared/Models/CustomerIdentityProvisioningState.cs` — stored on existing `Customer` row, not a separate table
- [x] T007 [P] Add `CustomerIdentityProvisioningDomainState` entity (customerId, domain enum, status, lastUpdatedAt) with unique key `(customerId, domain)` in `customer/shared/Customer.Shared/Models/CustomerIdentityProvisioningDomainState.cs` — stored in `Customer.IdentityProvisioningDomainStates` JSON collection
- [x] T008 Configure `CustomerIdentityProvisioningState` and `CustomerIdentityProvisioningDomainState` table mappings and relationships in the Customer.Shared `DbContext` in `customer/shared/Customer.Shared/Database/CustomerDbContext.cs` — configured on `Customer` entity in `CustomerConfiguration`
- [x] T009 Add EF Core migration for `CustomerIdentityProvisioningState` and `CustomerIdentityProvisioningDomainState` tables in `customer/shared/Customer.Shared/Database/Migrations/` — migration adds columns to `Customer` table only (`AddCustomerIdentityProvisioningToCustomer`)
- [x] T010 Add `ICustomerReadinessRepository` interface and `CustomerReadinessRepository` implementation (GetByCustomerId, Upsert aggregate, UpsertDomainState); emit structured logs for new aggregate creation and domain state upsert (database integration boundary — Constitution VI) in `customer/shared/Customer.Shared/Repositories/CustomerReadinessRepository.cs`
- [x] T011 [P] Add `IRequiredCustomerReadinessDomainService` interface and `RequiredCustomerReadinessDomainService` implementation returning the required domain set (Booking, Organisation, Team, Marketplace, Location) in `customer/shared/Customer.Shared/Services/RequiredCustomerReadinessDomainService.cs`
- [x] T012 Add `ICustomerReadinessDerivationService` interface and `CustomerReadinessDerivationService` implementation (derives Active vs Activating, sets activatedAt on first Active transition, guards against regression) in `customer/shared/Customer.Shared/Services/CustomerReadinessDerivationService.cs`
- [x] T013 [P] Register `ICustomerReadinessRepository`, `IRequiredCustomerReadinessDomainService`, and `ICustomerReadinessDerivationService` in Customer.Shared dependency injection extensions in `customer/shared/Customer.Shared/CustomerSharedServiceCollectionExtensions.cs`
- [x] T014 [P] Add unit tests for `CustomerReadinessDerivationService` covering: partial required domains → Activating, all required domains → Active, duplicate Active report → no regression, first Active transition sets activatedAt in `customer/shared/Customer.Shared.UnitTests/Services/CustomerReadinessDerivationServiceShould.cs`

**Checkpoint**: Customer readiness persistence, repository, required-domain service, and derivation service are ready — user story implementation can now begin

---

## Phase 3: User Story 1 — Gate Access from Central Readiness (Priority: P1) 🎯 MVP

**Goal**: Replace the existing backend readiness fan-out with a single customer-domain readiness lookup so that federated access is gated on the central customer readiness state, with no runtime calls to participating domains in the hot path.

**Independent Test**: Create a customer readiness state with only some required domains provisioned → verify access is blocked. Provision all required domains → verify access is allowed. Confirm no call is made to booking, organisation, team, marketplace, or location services during the access check.

### Tests for User Story 1

- [x] T015 [P] [US1] Add unit tests for `CustomerReadinessAccessService` covering: missing aggregate → blocked, partial domains → blocked, all required domains provisioned → allowed in `customer/apis/Customer.Api.UnitTests/Services/CustomerReadinessAccessServiceShould.cs`
- [x] T016 [P] [US1] Add unit test verifying the access check does not inject or call any participating-domain service (booking, organisation, team, marketplace, location, core, slack, msteams) in `customer/apis/Customer.Api.UnitTests/Services/CustomerReadinessAccessServiceShould.cs` (same class as T015)

### Implementation for User Story 1

- [x] T017 [US1] Implement `ICustomerReadinessAccessService` and `CustomerReadinessAccessService` (reads `ICustomerReadinessRepository`, uses `IRequiredCustomerReadinessDomainService`, returns allowed or blocked with reason) in `customer/apis/Customer.Api/Services/CustomerReadinessAccessService.cs`
- [x] T018 [US1] Identify the existing backend readiness/auth fan-out hot-path location (middleware, GraphQL auth hook, or federated auth check in `shared/Enterprise.Shared/Security/` or `customer/apis/Customer.Api/`) and replace the multi-domain fan-out call with `ICustomerReadinessAccessService`
- [x] T019 [P] [US1] Register `ICustomerReadinessAccessService` in `customer/apis/Customer.Api/` dependency injection configuration
- [x] T020 [P] [US1] Add structured logging in `CustomerReadinessAccessService` for: access allowed (customerId, status), access blocked (customerId, missing domains), missing aggregate (customerId); include correlation context (e.g. correlationId) as a required structured property on all log entries

**Checkpoint**: Normal authenticated/federated access is gated on the central customer readiness state; hot path makes zero calls to participating domains

---

## Phase 4: User Story 2 — Participating Domains Report Durable Provisioning (Priority: P2)

> **⚠️ Spec gap**: `contracts/customer-readiness-contract.md` and the proto value file (T002) currently list only five participating domains (Booking, Organisation, Team, Marketplace, Location). The confirmed full list is **eight** — also add Core, Slack, and MsTeams to the `Domain` enum in T002 and update the contracts doc before implementing any tasks in this phase.

**Goal**: Each non-customer participating domain publishes `CustomerIdentityProvisioned` to `customer_readiness` after its local customer identity or auth projection is durably provisioned, not merely on source event receipt. Replay is safe and idempotent.

**Independent Test**: Replay a customer source event through a participating domain processor. Verify that after local provisioning succeeds the domain publishes `CustomerIdentityProvisioned`. Verify that replaying again does not create duplicate local state and publishes the readiness report again safely. Verify no readiness event is published when the domain cannot map itself to the Domain enum.

### Tests for User Story 2

- [x] T021 [P] [US2] Add unit tests for `Booking.Processors` `CustomerSubscriber` readiness extension: publishes after local provisioning succeeds, skips publish for unmappable domain, idempotent replay publishes again in `booking/processors/Booking.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T022 [P] [US2] Add unit tests for `Organization.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `organization/processors/Organization.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T023 [P] [US2] Add unit tests for `Team.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `team/processors/Team.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T024 [P] [US2] Add unit tests for `Marketplace.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `marketplace/processors/Marketplace.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T025 [P] [US2] Add unit tests for `Location.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `location/processors/Location.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T025a [P] [US2] Add unit tests for `Core.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `core/processors/Core.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T025b [P] [US2] Add unit tests for `Slack.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `slack/processors/Slack.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`
- [x] T025c [P] [US2] Add unit tests for `MsTeams.Processors` `CustomerSubscriber` readiness extension: same scenarios as T021 in `msteams/processors/MsTeams.Processors.UnitTests/Subscribers/CustomerSubscriberShould.cs`

### Implementation for User Story 2

- [x] T026 [US2] Add `ICustomerReadinessPublisher` interface and `CustomerReadinessPublisher` (Kafka outbox producer for `customer_readiness` topic using `CustomerReadinessMetadata`) in `shared/Api.Shared.Clients/Events/Skedular/CustomerReadiness/V1/CustomerReadinessPublisher.cs`
- [x] T027 [P] [US2] Register `ICustomerReadinessPublisher` in `booking/processors/Booking.Processors/` dependency injection / program configuration
- [x] T028 [P] [US2] Register `ICustomerReadinessPublisher` in `organization/processors/Organization.Processors/` dependency injection / program configuration
- [x] T029 [P] [US2] Register `ICustomerReadinessPublisher` in `team/processors/Team.Processors/` dependency injection / program configuration
- [x] T030 [P] [US2] Register `ICustomerReadinessPublisher` in `marketplace/processors/Marketplace.Processors/` dependency injection / program configuration
- [x] T031 [P] [US2] Register `ICustomerReadinessPublisher` in `location/processors/Location.Processors/` dependency injection / program configuration
- [x] T031a [P] [US2] Register `ICustomerReadinessPublisher` in `core/processors/Core.Processors/` dependency injection / program configuration
- [x] T031b [P] [US2] Register `ICustomerReadinessPublisher` in `slack/processors/Slack.Processors/` dependency injection / program configuration
- [x] T031c [P] [US2] Register `ICustomerReadinessPublisher` in `msteams/processors/MsTeams.Processors/` dependency injection / program configuration
- [x] T032 [US2] Extend `Booking.Processors` `CustomerSubscriber` to inject `ICustomerReadinessPublisher` and publish `CustomerIdentityProvisioned` (Domain.Booking) after durable local customer provisioning and cache invalidation in `booking/processors/Booking.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T033 [P] [US2] Extend `Organization.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Organization) after durable local provisioning in `organization/processors/Organization.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T034 [P] [US2] Extend `Team.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Team) after durable local provisioning in `team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T035 [P] [US2] Extend `Marketplace.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Marketplace) after durable local provisioning in `marketplace/processors/Marketplace.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T036 [P] [US2] Extend `Location.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Location) after durable local provisioning in `location/processors/Location.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T036a [P] [US2] Extend `Core.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Core) after durable local provisioning in `core/processors/Core.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T036b [P] [US2] Extend `Slack.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.Slack) after durable local provisioning in `slack/processors/Slack.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T036c [P] [US2] Extend `MsTeams.Processors` `CustomerSubscriber` to publish `CustomerIdentityProvisioned` (Domain.MsTeams) after durable local provisioning in `msteams/processors/MsTeams.Processors/Subscribers/CustomerSubscriber.cs`
- [x] T037 [P] [US2] Add structured logging in all eight `CustomerSubscriber` extensions for: local provisioning decision, skipped publish (unmappable domain), publish success, publish failure; include correlation context (e.g. correlationId) as a required structured property

**Checkpoint**: All eight participating domains publish `CustomerIdentityProvisioned` after durable provisioning; replay is safe and idempotent

---

## Phase 5: User Story 3 — Customer Domain Tracks Per-Domain Progress (Priority: P3)

**Goal**: The customer domain consumes `customer_readiness` events, records per-domain readiness as a collection keyed by domain enum (no per-domain columns), derives overall status from the central required-domain set, and transitions to Active only when every required domain has reported. Duplicate events and future unknown event types are handled safely.

**Independent Test**: Deliver readiness reports in any order (including duplicates and out-of-order). Verify only reported domains are marked Provisioned. Verify overall status is Active only after all required domains report. Verify active customers do not regress on duplicate success. Deliver an unknown event type — verify known event processing continues.

### Tests for User Story 3

- [x] T038 [P] [US3] Add unit tests for `CustomerReadinessEventSubscriber` covering: single domain report → Activating, all required domains → Active with activatedAt set, duplicate active report → no regression, unknown event type → no failure, missing domain → Pending, processing `CustomerIdentityProvisioned` never sets status to `Failed` or `ActionRequired` (FR-019) in `customer/processors/Customer.Processors.UnitTests/Subscribers/CustomerReadinessEventSubscriberShould.cs`

### Implementation for User Story 3

- [x] T040 [US3] Add `CustomerReadinessEventSubscriber` class skeleton with Kafka subscription to `customer_readiness` topic and `ICustomerReadinessRepository`, `ICustomerReadinessDerivationService` injection in `customer/processors/Customer.Processors/Subscribers/CustomerReadinessEventSubscriber.cs`
- [x] T041 [US3] Implement `CustomerIdentityProvisioned` handler in `CustomerReadinessEventSubscriber`: upsert `CustomerIdentityProvisioningDomainState`, call derivation service, upsert `CustomerIdentityProvisioningState` with updated overall status and activatedAt in `customer/processors/Customer.Processors/Subscribers/CustomerReadinessEventSubscriber.cs`
- [x] T042 [US3] Implement unknown future readiness event type safety in `CustomerReadinessEventSubscriber`: log and skip unknown types without failing known event processing in `customer/processors/Customer.Processors/Subscribers/CustomerReadinessEventSubscriber.cs`
- [x] T043 [P] [US3] Register `CustomerReadinessEventSubscriber` in `customer/processors/Customer.Processors/Program.cs` Kafka subscriber configuration
- [x] T044 [P] [US3] Add structured logging in `CustomerReadinessEventSubscriber` for: consumption start/completion (customerId, domain), per-domain state change, Activating→Active transition (customerId, activatedAt), duplicate/replay outcome (customerId, domain), unknown event type (type discriminator), and failure paths; include correlation context (e.g. correlationId) as a required structured property

**Checkpoint**: Customer domain durably records per-domain readiness progress, derives correct overall status, and handles duplicates and unknown events safely

---

## Phase 6: User Story 4 — Backfill Historical Customer Provisioning (Priority: P4)

**Goal**: Operators can manually trigger customer synchronisation/backfill by republishing historical customer source events. Participating domains reprocess them idempotently, republish readiness, and the customer domain updates central readiness. Safe for active, partially provisioned, and missing customers.

**Independent Test**: Replay historical customer source events for active, partially provisioned, and customers with missing readiness state. Verify resulting readiness state is correct. Verify active customers are not disrupted. Verify duplicate reports during backfill do not cause duplicate transitions.

### Tests for User Story 4

- [x] T045 [P] [US4] Add unit tests for the backfill/synchronisation endpoint (operator trigger dispatches republish, logging, error paths) in `customer/apis/Customer.Api.UnitTests/Services/CustomerSynchronisationServiceShould.cs`

### Implementation for User Story 4

- [x] T047 [US4] Locate the existing customer workaround republish path (`customer/apis/Customer.Api/Controllers/` or `customer/apis/Customer.Api/Services/`) and extend or reuse it to support operator-triggered synchronisation/backfill for a single customer or all customers; if no suitable path exists, create one in `customer/apis/Customer.Api/Controllers/CustomerWorkaroundController.cs` following the existing controller pattern
- [x] T048 [P] [US4] Add structured logging in the backfill endpoint for: sync start (operator context, target scope), per-customer republish dispatch, completion (count, duration), and failure paths; include correlation context (e.g. correlationId) as a required structured property

**Checkpoint**: Operator-triggered backfill safely produces central readiness state through the same participating-domain provisioning path as production events

---

## Final Phase: Polish & Cross-Cutting Verification

**Purpose**: End-to-end verification, acceptance checklist validation, and regeneration confirmation.

- [x] T049 [P] Run `api-definitions/events/generate.sh` and verify all affected C# projects compile clean (`dotnet build` across `shared/Api.Shared.Clients/`)
- [x] T050 [P] Run unit test suite for all affected projects per quickstart.md
- [x] T052 Validate all items in the quickstart.md acceptance checklist

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion — **BLOCKS all user stories**
- **US1 (Phase 3)**: Depends on Foundational (Phase 2) completion — independently testable by seeding readiness state
- **US2 (Phase 4)**: Depends on Phase 1 (event contract) and Phase 2 (repository for publisher registration); parallisable with US1 and US3
- **US3 (Phase 5)**: Depends on Phase 1 (event contract) and Phase 2 (persistence/derivation services); parallisable with US1 and US2
- **US4 (Phase 6)**: Depends on US2 and US3 being functionally complete (phases 4 and 5)
- **Polish (Final Phase)**: Depends on all user stories complete

### User Story Dependencies

- **US1 (P1)**: Can start after Phase 2 — no dependency on US2 or US3 for independent testing
- **US2 (P2)**: Can start after Phase 1 and Phase 2 — no dependency on US1 or US3
- **US3 (P3)**: Can start after Phase 1 and Phase 2 — no dependency on US1 or US2
- **US4 (P4)**: Depends on US2 and US3 being complete

### Within Each User Story Phase

- Tests are written before implementation where marked
- Models/entities before services
- Services before endpoint/subscriber
- Core implementation before integration wiring
- Structured logging added alongside implementation (not as a final afterthought)

### Parallel Opportunities Per Phase

**Phase 1**: T001 and T002 can run in parallel → T003 → T004 → T005 runs in parallel with Phase 2 setup

**Phase 2**: T006 and T007 in parallel → T008 (needs T006 + T007) → T009 (needs T008); T010 in parallel with T011 → T012 (needs T011); T013 and T014 in parallel after T010-T012

**Phase 3 (US1)**: T015 and T016 (tests) in parallel before T017-T020; T019 and T020 in parallel

**Phase 4 (US2)**: T021–T025 (tests) in parallel before T026; T027–T031 (registrations) in parallel; T033–T036 (subscriber extensions) in parallel after T032; T037 in parallel with T032-T036

**Phase 5 (US3)**: T038–T039 (tests) before T040; T041 and T042 in parallel after T040; T043 and T044 in parallel

**Phase 6 (US4)**: T045–T046 (tests) before T047; T048 in parallel with T047

**Final Phase**: T049–T050 in parallel → T052

---

## Parallel Example: User Story 2 (Phase 4)

If two developers are available for Phase 4:

**Developer A** (Booking + shared publisher):

```
T026 → T032 → T037 (shared publisher, Booking subscriber extension, logging task)
```

**Developer B** (Organisation, Team, Marketplace, Location, Core, Slack, MsTeams in parallel):

```
[T022, T023, T024, T025, T025a, T025b, T025c in parallel]          (unit tests first — seven domains)
Then [T028, T029, T030, T031, T031a, T031b, T031c in parallel]     (register publisher — seven domains)
Then [T033, T034, T035, T036, T036a, T036b, T036c in parallel]     (extend subscriber — seven domains)
```

---

## Summary

| Phase            | Story            | Tasks     | Key Deliverable                                                                         |
| ---------------- | ---------------- | --------- | --------------------------------------------------------------------------------------- |
| 1 — Setup        | —                | T001–T005 | `customer_readiness` protobuf contract + metadata companion                             |
| 2 — Foundational | —                | T006–T014 | Customer readiness persistence, repository, required-domain service, derivation service |
| 3 — US1 (P1)     | Gate access      | T015–T020 | Hot-path fan-out removed; single customer-domain readiness lookup                       |
| 4 — US2 (P2)     | Domain reporting | T021–T037 | All eight participating domains publish after durable provisioning                      |
| 5 — US3 (P3)     | Track progress   | T038–T044 | Customer processor subscriber; per-domain state; idempotent activation                  |
| 6 — US4 (P4)     | Backfill         | T045–T048 | Operator-triggered republish backfill flow                                              |
| Final            | Polish           | T049–T052 | Verified compilation, full unit test suite, acceptance checklist                        |

**Total tasks**: 61  
**Suggested MVP scope**: Phases 1, 2, and 3 (US1) — 21 tasks deliver the foundation and the access gate, independently testable by seeding readiness state directly
