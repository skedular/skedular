# Tasks: Bulk Resource Import

**Input**: Design documents from `specs/008-bulk-resource-import/`
**Prerequisites**: plan.md ✓ · spec.md ✓ · research.md ✓ · data-model.md ✓ · contracts/ ✓ · quickstart.md ✓

---

## Phase 1: Setup

**Purpose**: No new project initialisation required — this feature extends the existing `location` domain and `web/apps/webapp`. Setup tasks ensure shared infrastructure (DI registration, mapper wiring) is in place before user story work begins.

- [X] T001 Register `IBulkImportResourcesService` in the `location` API DI container in `location/apis/Location.Api/DependencyInjection.cs` (or equivalent registration file)
- [X] T002 [P] Add `GetActiveNamesByLocationIdAsync` to `IResourceRepository` interface in `location/shared/Location.Shared/Repositories/ResourceRepository.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core backend types and mapper wiring that MUST exist before any user story mutation or UI work can be implemented.

**⚠️ CRITICAL**: No user story work can begin until this phase is complete.

- [X] T003 [P] Create `BulkImportResourcesInput` HotChocolate input class in `location/apis/Location.Api/GraphQL/Resource/BulkImportResourcesInput.cs`
- [X] T004 [P] Create `BulkImportResourceRowInput` HotChocolate input class in `location/apis/Location.Api/GraphQL/Resource/BulkImportResourceRowInput.cs`
- [X] T005 [P] Create `BulkImportResourcesPayload` HotChocolate output class in `location/apis/Location.Api/GraphQL/Resource/BulkImportResourcesPayload.cs`
- [X] T006 [P] Create `BulkImportResourceRowResult` HotChocolate output class in `location/apis/Location.Api/GraphQL/Resource/BulkImportResourceRowResult.cs`
- [X] T007 Create service-layer models (`BulkImportResources`, `BulkImportResourceRow`, `BulkImportRowResult`) in `location/apis/Location.Api/Services/BulkImportResourcesService.cs` (or a dedicated models file alongside the service)
- [X] T008 Implement `GetActiveNamesByLocationIdAsync` on `ResourceRepository` in `location/shared/Location.Shared/Repositories/ResourceRepository.cs` — single read query returning all active resource names for a location
- [X] T009 Add `MapTo` overloads for `BulkImportResourcesInput → BulkImportResources` and `BulkImportRowResult → BulkImportResourceRowResult` to `location/apis/Location.Api/Mappers/GraphQlMapper.cs`

**Checkpoint**: All types compiled, mapper wiring complete — user story implementation can now begin.

---

## Phase 3: User Story 1 — Bulk Add Resources to a Location (Priority: P1) 🎯 MVP

**Goal**: `bulkImportResources` GraphQL mutation validates rows, auto-generates names, and persists all valid resources in a single transaction, returning per-row success/failure results.

**Independent Test**: Submit a 10-row batch with a valid base name and resource-type tag — verify 10 resources created with auto-generated `{baseName}-{n}` names. Submit a batch with one invalid row (quantity = 0) — verify partial success.

### Implementation

- [X] T010 [US1] Implement `IBulkImportResourcesService` interface and `BulkImportResourcesService` skeleton (constructor, DI dependencies mirroring `ResourceService`) in `location/apis/Location.Api/Services/BulkImportResourcesService.cs`
- [X] T011 [US1] Implement top-level input validation in `BulkImportResourcesService.ImportAsync`: reject empty `rows`, reject `sum(quantities) > 100`, reject any row with `quantity < 1` — collect per-row failures without aborting other rows
- [X] T012 [US1] Implement resource-type tag validation in `BulkImportResourcesService.ImportAsync`: fetch all tag IDs for the batch in one repository call, verify each row's `organizationResourceTypeTagId` exists and is a resource-type tag, record `failureReason` per invalid row
- [X] T013 [US1] Implement name-generation logic in `BulkImportResourcesService.ImportAsync`: call `GetActiveNamesByLocationIdAsync` once, maintain in-memory allocated-name set, apply always-append suffix strategy (`effectiveBaseName = baseName ?? resourceTypeName`, `nextSuffix = max(existing+allocated suffixes) + 1` starting from 1)
- [X] T014 [US1] Implement single-transaction bulk write in `BulkImportResourcesService.ImportAsync`: add all pre-validated resources via `IRepositoryFactory.ResourceRepository.Add(...)` inside one `IDbTransactionBuilder` transaction, publish `locationOutboxPublisher`, trigger `ITemporalOutboxService.StartComputeOrganizationLocationsAndProductsRelationships` once per location (not per resource)
- [X] T015 [US1] Add structured logging to `BulkImportResourcesService`: log batch received (locationId, rowCount, totalQuantity), log each per-row failure (rowIndex, reason — no tag content), log completion summary (created, rejected) in `location/apis/Location.Api/Services/BulkImportResourcesService.cs`
- [X] T016 [US1] Register `BulkImportResourcesAsync` mutation method on `RootMutation` in `location/apis/Location.Api/GraphQL/Resource/RootMutation.cs`
- [X] T017 [US1] Run `scripts/generate-graphql.sh` to regenerate `location/domain/Location.Domain.IntegrationTests/schema.graphql`, the composed gateway schema, and Relay artefacts — verify `bulkImportResources` appears in the schema output

### Unit Tests

- [X] T018 [P] [US1] Create `ImportShould` unit test class covering naming logic (base name present, base name empty, collision with existing names, within-batch collision) in `location/apis/Location.Api.UnitTests/Services/BulkImportResourcesServiceTests/ImportShould.cs`
- [X] T019 [P] [US1] Add unit test cases for input validation (empty rows, sum > 100, quantity < 1, missing resource-type tag, invalid tag IDs) and assert that all emitted log entries include a correlation/request identifier (LOG-004) in `location/apis/Location.Api.UnitTests/Services/BulkImportResourcesServiceTests/ImportShould.cs`
- [X] T020 [P] [US1] Add unit test case for partial success (mixed valid/invalid rows — valid rows persisted, invalid rows return failure reasons) in `location/apis/Location.Api.UnitTests/Services/BulkImportResourcesServiceTests/ImportShould.cs`

### Integration Tests

- [X] T021 [US1] Create `BulkImportResourcesShould` integration test class with happy-path test: submit valid 3-row batch, assert resources created via `IResourceRepository` in `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/Resource/BulkImportResourcesShould.cs`
- [X] T022 [US1] Add partial-success integration test: 1 valid row + 1 invalid row → valid created, invalid returns failure reason in `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/Resource/BulkImportResourcesShould.cs`
- [X] T023 [US1] Add empty-base-name integration test: no `baseName` provided → names derived from resource type name in `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/Resource/BulkImportResourcesShould.cs`
- [X] T024 [US1] Add name-collision integration test: pre-seed `Desk-1`, `Desk-3` → batch generates `Desk-4`, `Desk-5` (always-append, not gap-fill) in `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/Resource/BulkImportResourcesShould.cs`
- [X] T025 [US1] Add batch-size-limit integration test: submit batch with total quantity 101 → mutation returns error before processing any row in `location/domain/Location.Domain.IntegrationTests/Api/GraphQL/Resource/BulkImportResourcesShould.cs`

**Checkpoint**: `bulkImportResources` mutation fully functional and tested independently.

---

## Phase 4: User Story 2 — Compose Batch in the UI Before Submitting (Priority: P1)

**Goal**: A dynamic row-based form in the webapp allows the admin to add/remove/edit rows (type, base name, quantity, tags) before submitting the batch.

**Independent Test**: Add 3 rows, remove 1, edit the type on another, verify the final submission payload matches the composed state.

### Implementation

- [X] T026 [US2] Run `web/apps/webapp/scripts/generate.sh` to regenerate Relay TypeScript types from the updated schema (depends on T017) — verify `bulkImportResourcesMutation` type is generated in `web/apps/webapp/src/queries/__generated__/`
- [X] T027 [P] [US2] Create `BulkImportResourceRowForm` component (row: resource-type selector, base-name text field, quantity number field, custom-tags multi-select, zones multi-select, product-tags multi-select, remove button) in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-row.tsx` — use `SingleChoiceResourceType`, `MultipleChoicesCustomTags`, `MultipleChoicesZones`, `MultipleChoicesProductTags` Relay fragment components; typography via `@skedular/ui` wrappers
- [X] T028 [US2] Create `BulkImportResourcesDialog` component with dynamic row list, "Add row" button, submit/cancel actions, and `useMutation` wiring to `bulkImportResources` Relay mutation in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-dialog.tsx` — use `useQueryLoader`/`usePreloadedQuery` pattern matching `AddResourceDialogWithRelay`
- [X] T029 [US2] Create `index.ts` barrel export for the new component in `web/apps/webapp/src/components/resource/bulkImportResources/index.ts`
- [X] T030 [US2] Add "Bulk add" button to `organization-location-manage-resources-section.tsx` and wire `BulkImportResourcesDialog` open/close state in `web/apps/webapp/src/components/organization/organizationLocation/organization-location-manage-resources-section.tsx`
- [X] T031 [US2] Implement client-side validation: disable submit when row list is empty; ensure quantity field enforces min=1 in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-dialog.tsx`

**Checkpoint**: Admin can open the dialog, compose a batch, and submit it — results are received from the API.

---

## Phase 5: User Story 3 — Review Import Results (Priority: P2)

**Goal**: After submission, the admin sees a per-row results view with total created/failed counts, per-row failure reasons, and a "Retry failed rows" action.

**Independent Test**: Submit a batch with one deliberate error — verify results view shows 1 created, 1 failed with the correct reason, and the retry action pre-populates only the failed row.

### Implementation

- [X] T032 [US3] Add results view to `BulkImportResourcesDialog`: show total created count, total failed count, per-row result list (success or failure reason) after mutation completes in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-dialog.tsx`
- [X] T033 [US3] Implement "Retry failed rows" action: extract failed rows from the mutation response, pre-populate a new batch form with those rows' last values in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-dialog.tsx`
- [X] T034 [US3] Implement "Done" / dismiss action: close dialog and trigger `onReloadRequired` to refresh the location resource list in `web/apps/webapp/src/components/resource/bulkImportResources/bulk-import-resources-dialog.tsx`

**Checkpoint**: Full end-to-end flow complete — admin can import, review results, retry failures, and see newly created resources in the location list.

---

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T035 [P] Verify all user-facing strings in the new UI components use British spelling (e.g., "organised", "authorise") — fix any American-English copy in `web/apps/webapp/src/components/resource/bulkImportResources/`
- [X] T036 [P] Confirm no `@mui/material/Typography` is imported directly in any new component — all typography via `@skedular/ui` wrappers in `web/apps/webapp/src/components/resource/bulkImportResources/`
- [X] T037 [P] Confirm all Relay fragments are collocated with their consuming components in `web/apps/webapp/src/components/resource/bulkImportResources/` — no hand-edited `__generated__` files
- [X] T038 Add `BulkImportResourcesService` to the location API integration test Aspire host `WaitFor` wiring if any new dependency is introduced (check `location/domain/Location.Domain.IntegrationTests/AppHost.cs`)
- [X] T039 [P] Run full location unit test suite and fix any regressions: `dotnet test location/apis/Location.Api.UnitTests`
- [X] T040 [P] Run full location integration test suite and fix any regressions: `dotnet test location/domain/Location.Domain.IntegrationTests`

---

## Dependencies

```
Phase 1 (T001–T002)
  └── Phase 2 (T003–T009)
        ├── Phase 3: US1 backend (T010–T025)
        │     └── T017 generate-graphql.sh
        │           └── Phase 4: US2 UI (T026–T031) ← T026 depends on T017
        │                 └── Phase 5: US3 results (T032–T034)
        │                       └── Phase 6: polish (T035–T040)
        └── (T009 mapper wiring also gates Phase 3)
```

**Parallel opportunities within phases**:

- T003–T006 (all four GraphQL type files) — fully parallel
- T018–T020 (unit test cases) — parallel with each other, parallel with T021–T025
- T021–T025 (integration test cases) — parallel with each other after T010–T016
- T027 (row form component) — parallel with T028 (dialog skeleton), once T026 is done
- T035–T037, T039–T040 — fully parallel

---

## Implementation Strategy

**MVP scope** (Phases 1–3): The GraphQL mutation alone is a complete, independently verifiable MVP. An admin can call `bulkImportResources` directly from any GraphQL client and verify partial success, naming, and tag assignment without any UI.

**Incremental delivery**:

1. Phases 1–3 → working mutation, unit + integration tests passing
2. Phase 4 → dialog form in webapp; admin can use the UI to compose and submit
3. Phase 5 → results view and retry flow complete the end-to-end UX
4. Phase 6 → final polish and regression validation

**Total tasks**: 40
**Tasks per user story**: US1 = 16 (T010–T025), US2 = 6 (T026–T031), US3 = 3 (T032–T034)
**Parallel opportunities**: 20+ tasks can run in parallel within their phase

**Out-of-scope success criteria** (post-launch metrics, no implementation task required):
- SC-001: "Admin completes 50-resource import in under 3 minutes" — validated via manual QA/usability testing after release.
- SC-004: "90% task-completion rate in usability testing" — post-launch user research metric.
