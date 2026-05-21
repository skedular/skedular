# Tasks: Organization Patch Updates

**Input**: Design documents from `specs/010-organization-patch-updates/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/organization-patch-update.graphql.md, quickstart.md

**Tests**: Required by the repository constitution and plan. Write story tests before implementation where practical.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files and has no dependency on incomplete tasks
- **[Story]**: Which user story the task belongs to
- Every task includes exact file paths

## Phase 1: Setup

**Purpose**: Confirm current organisation update shape and create the shared files needed by all stories.

- [x] T001 Inspect existing organisation update field mappings for all editable organisation setup fields in `organization/apis/Organization.Api/Mappers/GraphQlMapper.cs` and `organization/shared/Organization.Shared/Models/Organization.cs`
- [x] T002 Inspect existing organisation entity concurrency behaviour in `organization/shared/Organization.Shared/Database/Entities/Organization.cs` and repository save flow in `organization/shared/Organization.Shared/Repositories/OrganizationRepository.cs`
- [x] T003 Create GraphQL field-masked update input and enum classes in `organization/apis/Organization.Api/GraphQL/Organization/UpdateOrganizationInput.cs`
- [x] T004 Reuse existing `OrganizationPayload` for the field-masked update response in `organization/apis/Organization.Api/GraphQL/Organization/RootMutation.cs`

---

## Phase 2: Foundational

**Purpose**: Add service-level patch contracts and logging seams that block all user stories.

- [x] T005 Add organisation patch request/result model types in `organization/apis/Organization.Api/Services/OrganizationPatchModels.cs`
- [x] T006 Extend `IOrganizationService` with `UpdatePatchAsync` in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T007 Add mapping from `UpdateOrganizationInput` to the patch request model in `organization/apis/Organization.Api/Mappers/GraphQlMapper.cs`
- [x] T008 Add structured logging dependencies and event names for organisation patch workflows in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T009 Add unit test fixture helpers for organisation patch service tests in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/OrganizationPatchTestHelpers.cs`

**Checkpoint**: Patch request/result types, mapping seam, service interface, and logging seam are ready for story implementation.

---

## Phase 3: User Story 1 - Save a single organisation field inline (Priority: P1)

**Goal**: A caller can use the GraphQL field-masked update contract to update one allowlisted field while omitted allowlisted fields remain unchanged.

**Independent Test**: Patch only name or only description through GraphQL and verify the selected value changes while the omitted value is preserved.

### Tests for User Story 1

- [x] T010 [P] [US1] Add service unit tests for single-field and full setup patch in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T010a [P] [US1] Add mapper unit tests for applying all editable organisation setup fields in `organization/apis/Organization.Api.UnitTests/Mappers/OrganizationPatchMapperTests/ApplyToShould.cs`
- [x] T011 [P] [US1] Add GraphQL integration tests for `updateOrganization` single-field field-masked updates in `organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationMutationTests/UpdateOrganizationShould.cs`
- [x] T012 [P] [US1] Add mapping unit tests for `UpdateOrganizationInput.fieldsToUpdate` enum handling in `organization/apis/Organization.Api.UnitTests/Mappers/GraphQlMapperTests/MapToShould.cs`

### Implementation for User Story 1

- [x] T013 [US1] Implement `updateOrganization(input: UpdateOrganizationInput!)` resolver with field-mask semantics in `organization/apis/Organization.Api/GraphQL/Organization/RootMutation.cs`
- [x] T014 [US1] Implement all editable organisation setup fields allowlist application in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T015 [US1] Return existing `OrganizationPayload` with the latest organisation details in `organization/apis/Organization.Api/GraphQL/Organization/RootMutation.cs`
- [x] T016 [US1] Add structured logs for patch start and applied completion in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T017 [US1] Run `scripts/generate-graphql.sh` from the repository root and commit regenerated GraphQL outputs under `organization/apis/Organization.Api/schema.graphqls`, `organization/domain/Organization.Domain.IntegrationTests/schema.graphql`, and related generated GraphQL artifacts

**Checkpoint**: User Story 1 is independently functional as the MVP.

---

## Phase 4: User Story 2 - Preserve omitted values from partial callers (Priority: P2)

**Goal**: Invalid, no-op, disallowed, and concurrent partial update cases preserve omitted values and never apply partial invalid changes.

**Independent Test**: Submit partial requests for unknown fields, invalid values, no-op values, and simulated concurrency conflicts; verify omitted organisation values remain unchanged.

### Tests for User Story 2

- [x] T018 [US2] Add unit tests for rejecting non-allowlisted `fieldsToUpdate` values in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T019 [US2] Add unit tests for invalid selected values applying no changes in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T020 [US2] Add unit tests for valid no-op patches returning the current organisation in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T021 [US2] Add unit tests for entity concurrency retry preserving omitted fields in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T022 [P] [US2] Add GraphQL integration tests for invalid field selection, no-op, and concurrency retry behaviour in `organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationMutationTests/UpdateOrganizationPatchShould.cs`

### Implementation for User Story 2

- [x] T023 [US2] Implement whole-patch validation before persistence in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T024 [US2] Implement no-op-compatible patch handling while still returning the latest organisation details in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T025 [US2] Implement entity concurrency catch, latest-organisation reload, and selected-field retry in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T026 [US2] Add structured logs for completion, disallowed field, validation failure, authorisation failure, concurrency retry, and persistence failure in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T027 [US2] Add or reuse repository-layer assertion helpers for persisted organisation state in `organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationMutationTests/UpdateOrganizationShould.cs`

**Checkpoint**: User Stories 1 and 2 both work independently without full-object overwrite regressions.

---

## Phase 5: User Story 3 - Reuse a consistent patch contract across organisation updates (Priority: P3)

**Goal**: The organisation field-mask contract is documented, old full-replacement paths are removed, and migrated GraphQL/gRPC organisation update callers use one public `Update*` contract per update surface.

**Independent Test**: Review the contract, confirm web callers use field-masked `Update*` GraphQL mutations, confirm removed `*Patch` aliases are not exposed, and confirm migrated gRPC billing/tag/zone updates require field masks.

### Tests for User Story 3

- [x] T028 [P] [US3] Add regression tests proving removed `*Patch` GraphQL aliases are not exposed and setup updates use `updateOrganization` with `fieldsToUpdate`
- [x] T029 [P] [US3] Add tests proving migrated gRPC billing/tag/zone update inputs preserve omitted fields in organisation gRPC integration coverage
- [x] T030 [P] [US3] Add logging tests asserting patch runtime log level and content intent in `organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs`
- [x] T030a [P] [US3] Add web UI unit tests for setup-form patch saves, inline saving indicators, no success toast, and failure toast behaviour in `web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx`
- [x] T030b [P] [US3] Add web UI unit tests for setup-form patch saves, inline saving indicators, no success toast, and failure toast behaviour in `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx`
- [x] T030c [P] [US3] Add web UI unit tests for setup-form patch saves, inline saving indicators, no success toast, and failure toast behaviour in `web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-admin-setup-section.test.tsx`
- [x] T030d [P] [US3] Add follow-up regression coverage for each migrated specialised organisation field-masked update mutation.
- [x] T030e [P] [US3] Add backend unit tests for the SSO settings field-masked update path in `organization/apis/Organization.Api.UnitTests/Services/OrganizationSsoServiceTests/UpdatePatchAsyncShould.cs`
- [x] T030f [P] [US3] Add web UI unit tests proving SSO settings saves use `updateOrganizationSsoSettings` with `fieldsToUpdate` in `web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx`, `web/apps/webapp-teams/src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx`, and `web/apps/webapp-spaces/src/components/organization/organizationAdmin/organization-admin-sso-section.test.tsx`

### Implementation for User Story 3

- [x] T031 [US3] Document the reusable field-masked update pattern in `organization/docs/architecture/organization-domain-architecture.md`
- [x] T032 [US3] Update `specs/010-organization-patch-updates/contracts/organization-patch-update.graphql.md` with final implemented mutation, field enum, and payload names
- [x] T033 [US3] Update `specs/010-organization-patch-updates/quickstart.md` with final verification commands and any generated artifact notes
- [x] T033a [US3] Migrate specialised organisation GraphQL update surfaces to field-masked updates across backend schema, services, Relay artefacts, and affected web apps.
- [x] T033b [US3] Migrate organisation SSO settings to field-masked `updateOrganizationSsoSettings` usage across backend schema, service, Relay artefacts, and all three web apps.
- [x] T033c [US3] Migrate organisation gRPC billing details, tag, custom tag, product tag, and zone update endpoints to field-masked update inputs in `api-definitions/grpc/skedular/organization/`.
- [x] T033d [US3] Remove dead public full-replacement organisation update methods and stale replacement mappers where migrated callers no longer use them.
- [x] T033e [US3] Rename temporary public GraphQL and gRPC `*Patch` update contracts back to single `Update*` names while retaining `fieldsToUpdate` patch semantics.

**Checkpoint**: Patch behaviour is documented as an extendable organisation-domain pattern and existing update surfaces remain compatible.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Regeneration, verification, and cleanup across all stories.

- [x] T034 Run `dotnet test organization/apis/Organization.Api.UnitTests/Organization.Api.UnitTests.csproj --no-restore` from the repository root
- [x] T035 Run `dotnet test organization/domain/Organization.Domain.IntegrationTests/Organization.Domain.IntegrationTests.csproj --no-restore --filter FullyQualifiedName~Organization` from the repository root
- [x] T036 Regenerate Relay artefacts for `webapp`, `webapp-teams`, and `webapp-spaces` after backend GraphQL schema regeneration
- [x] T037 Review generated GraphQL and Relay artifacts to ensure no generated file was hand-edited in `organization/apis/Organization.Api/schema.graphqls`, `organization/domain/Organization.Domain.IntegrationTests/schema.graphql`, and `web/apps/webapp/src/queries/__generated__/`
- [x] T038 Review user/operator-facing validation messages for British spelling in `organization/apis/Organization.Api/Services/OrganizationService.cs`
- [x] T039 Update `specs/010-organization-patch-updates/tasks.md` completion state after implementation verification
- [x] T040 Run focused builds, organisation patch unit tests, focused web lint, and `git diff --check` after contract-name cleanup

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies
- **Foundational (Phase 2)**: Depends on Setup completion and blocks user stories
- **User Story 1 (Phase 3)**: Depends on Foundational; MVP
- **User Story 2 (Phase 4)**: Depends on Foundational and may build on the `UpdatePatchAsync` implementation from US1
- **User Story 3 (Phase 5)**: Depends on the final contract behaviour from US1/US2
- **Polish**: Depends on all desired user stories

### User Story Dependencies

- **US1**: Independent MVP after Foundational
- **US2**: Can start after Foundational, but implementation is simpler after US1 service/resolver scaffolding exists
- **US3**: Should run after US1 and US2 so docs and regression checks match final behaviour

### Within Each User Story

- Tests before implementation where practical
- Input/model work before service work
- Service behaviour before resolver/payload integration
- Contract generation after GraphQL code changes
- Logging assertions after runtime logging is implemented

---

## Parallel Execution Examples

### User Story 1

```text
Task: T010 Add service unit tests in organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs
Task: T011 Add GraphQL integration tests in organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationMutationTests/UpdateOrganizationPatchShould.cs
Task: T012 Add mapping unit tests in organization/apis/Organization.Api.UnitTests/Mappers/GraphQlMapperTests/MapToShould.cs
```

### User Story 2

```text
Task: T018 Add allowlist rejection unit tests in organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs
Task: T019 Add invalid value unit tests in organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs
Task: T022 Add GraphQL negative-flow integration tests in organization/domain/Organization.Domain.IntegrationTests/Api/GraphQL/OrganizationMutationTests/UpdateOrganizationPatchShould.cs
```

### User Story 3

```text
Task: T028 Add regression tests proving removed GraphQL `*Patch` aliases stay absent and setup updates use `updateOrganization` with `fieldsToUpdate`
Task: T030 Add logging tests in organization/apis/Organization.Api.UnitTests/Services/OrganizationServiceTests/UpdatePatchAsyncShould.cs
Task: T031 Update architecture documentation in organization/docs/architecture/organization-domain-architecture.md
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 and Phase 2.
2. Complete User Story 1.
3. Run US1 unit and integration tests.
4. Regenerate GraphQL outputs.
5. Stop and validate that one selected field updates while omitted fields remain unchanged.

### Incremental Delivery

1. Deliver US1 as the first usable field-masked update mutation.
2. Add US2 to harden omitted-field preservation, no-op, invalid, and concurrency retry cases.
3. Add US3 to document the reusable pattern and confirm existing update surfaces remain compatible.
4. Run polish verification and regeneration checks.

### Parallel Team Strategy

1. One developer handles Setup and Foundational service/input seams.
2. Once Foundational is complete, separate developers can write US1 service tests, US1 GraphQL integration tests, and US2 negative-flow tests in parallel.
3. Documentation and patch-only regression tests can proceed after the mutation contract stabilises.

---

## Notes

- `fieldsToUpdate` is the explicit enum-list update mask.
- Do not add `expectedVersion`; concurrency remains entity-layer-owned.
- Keep normal public gRPC `Update*` names while using explicit field masks on migrated organisation update endpoints.
- Do not hand-edit generated GraphQL schema or Relay artifacts.
- Integration tests must not query `DbContext` directly; use repository/query-layer assertions.
