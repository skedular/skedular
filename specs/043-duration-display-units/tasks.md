# Tasks: Persisted Duration Display Units

**Input**: Design documents from /specs/043-duration-display-units/
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/, quickstart.md

**Organization**: Tasks are grouped by user story. The repository-wide audit is foundational and must be refreshed before implementation is closed.

## Phase 1: Setup

**Purpose**: Confirm the source-of-truth files and generated-artifact workflow.

- [X] T001 Review specs/043-duration-display-units/research.md against current repository paths and record the starting audit baseline
- [X] T002 [P] Identify affected GraphQL source definitions and generated surfaces under src/marketplace/apis/Marketplace.Api, api-definitions/graphql, and src/web
- [X] T003 [P] Identify affected event source definitions and generated surfaces under api-definitions/events/skedular and src/shared/Api.Shared.Clients

## Phase 2: Foundational

**Purpose**: Establish the shared model, enum mapping, audit inventory, and regression boundary before story work.

- [X] T004 BLOCKING GATE: Run a repository-wide targeted search for persisted editable minute/hour concepts; update specs/043-duration-display-units/research.md with every occurrence, owner, persistence boundary, editor, classification, exact test paths, and implementation decision before any user-story implementation begins
- [X] T005 Define the owning display-unit model/constants and explicit persisted-string-to-model mappings in src/shared/Api.Shared.Services/Models, including HOURS fallback and unknown-value behavior
- [X] T006 [P] Add shared model serialization/deserialization coverage for MINUTES, HOURS, missing metadata, and unsupported metadata in the exact owning model test path recorded by T004
- [X] T007 [P] Add shared duration-input behavior tests for default unit, initial unit, controlled unit changes, canonical-minute preservation, and existing conversion/rounding in src/web/packages/ui/src/duration-input.test.tsx
- [X] T008 Document the source-to-generated regeneration commands and affected surfaces in specs/043-duration-display-units/contracts/duration-display-unit-contract.md

## Phase 3: User Story 1 - Restore Marketplace Duration Preferences (Priority: P1) 🎯 MVP

**Goal**: Marketplace pricing editors persist and restore per-field display units without changing canonical minute values.

**Independent Test**: Load legacy pricing JSON without metadata, save 5 minutes with MINUTES, reload, save HOURS, reload, and verify display state plus unchanged canonical minutes for every Marketplace duration field.

### Tests for User Story 1

- [X] T009 [P] [US1] Add ProductPricing JSON compatibility tests for absent and present display-unit metadata in the exact Marketplace model test path recorded by T004
- [X] T010 [P] [US1] Verify Marketplace mapper behavior remains canonical-only because display-unit metadata is intentionally domain-local and excluded from cross-domain event mapping
- [X] T011 [P] [US1] Verify generated Marketplace and integration GraphQL contracts expose nullable duration display-unit fields and preserve omitted-input compatibility
- [X] T012 [P] [US1] Extend product editor tests for 5-minute MINUTES/HOURS save-and-reload behavior in src/web/apps/webapp-spaces/src/test/product-editor-form.test.tsx and src/web/apps/webapp-host/src/components/product/product-editor-shared.test.ts

### Implementation for User Story 1

- [X] T013 Add nullable per-field display-unit metadata to ProductPricing and ProductPricingCancellationRefundRule in src/shared/Api.Shared.Services/Models/ProductPricing.cs and owning model files without changing existing minute fields
- [X] T014 Verify that display-unit metadata remains in the owning domain/subgraph and is not added to cross-domain events; update an event source definition only if a concrete same-domain editor contract requires it
- [X] T015 Update Marketplace event mapping and JSON projection logic in src/marketplace/shared/Marketplace.Shared/Mappers/EventMapper.cs and related source mappers
- [X] T016 Update Marketplace GraphQL source definitions and input/output mappings in src/marketplace/apis/Marketplace.Api to expose nullable display-unit fields
- [X] T017 Regenerate Marketplace event, GraphQL schema, and affected generated client artifacts using api-definitions/events/generate.sh, scripts/generate-graphql.sh, and the required web generation commands
- [X] T018 Update Marketplace product pricing form state and submit/autosave mapping in src/web/apps/webapp-spaces/src/components/product/product-editor-shared.ts and src/web/apps/webapp-host/src/components/product/product-editor-shared.ts
- [X] T019 Verify Marketplace pricing validation, refund policy evaluation, lock-window behavior, and booking calculations continue to consume only canonical minute fields in src/booking/shared/Booking.Shared and src/marketplace/shared
- [X] T020 Run the User Story 1 independent test and relevant Marketplace backend/frontend tests; fix failures without changing existing conversion/rounding behavior

## Phase 4: User Story 2 - Consistent Cross-Domain Editor Conversion (Priority: P2)

**Goal**: Every identified in-scope editor uses the shared display-unit contract and submits canonical minutes plus optional metadata.

**Independent Test**: In each editor listed in research.md, switch units, submit without changing the visible value, and verify the same canonical minute value and selected unit are sent.

### Tests for User Story 2

- [X] T021 [P] [US2] Add shared DurationInput tests for all controlled and uncontrolled unit-change paths in src/web/packages/ui/src/duration-input.test.tsx
- [X] T022 [P] [US2] Add Spaces editor tests for minimum/maximum duration, card/bank lock windows, and cancellation timing in src/web/apps/webapp-spaces/src/test/product-editor-form.test.tsx
- [X] T023 [P] [US2] Add Host unified listing and legacy location pricing tests in src/web/apps/webapp-host/src/components/unified-listing-form and src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing
- [X] T024 [P] [US2] Verify generated Relay clients and owning editor contracts for every audit-confirmed occurrence

### Implementation for User Story 2

- [X] T025 Extend the shared duration input API and implementation in src/web/packages/ui/src/duration-input.tsx to carry canonical minutes, selected/initial units, controlled changes, and existing conversion/rounding
- [X] T026 Update Spaces product editor forms in src/web/apps/webapp-spaces/src/components/product/product-editor-form.tsx and related add/edit files to carry value plus display unit per field
- [X] T027 Update Host product editors in src/web/apps/webapp-host/src/components/product/product-editor-form.tsx and related add/edit files to carry value plus display unit per field
- [X] T028 Update Host unified listing pricing in src/web/apps/webapp-host/src/components/unified-listing-form/pricing-section.tsx, HostListingProductSettings.tsx, and useHostListingCoordinator.ts
- [X] T029 Update the Host legacy location pricing editor in src/web/apps/webapp-host/src/rootPages/organizations/organization/locations/location/pricing/page.tsx
- [X] T030 Implement the same metadata and editor behavior for every additional persisted editable occurrence added to research.md by T004
- [X] T031 Regenerate affected Relay artifacts and GraphQL clients using the repository web generation commands; do not hand-edit generated files
- [X] T032 Run all identified editor independent tests and confirm mutation success updates Relay state without window.location.reload()

## Phase 5: User Story 3 - Preserve Non-Editor Duration Semantics (Priority: P3)

**Goal**: Operational, calculated, and read-only duration consumers remain unchanged and explicitly covered by the audit.

**Independent Test**: Run calculations with identical canonical minute inputs and compare behavior before/after display metadata changes.

### Tests for User Story 3

- [X] T033 [P] [US3] Add or extend unit tests for Marketplace refund policy evaluation using canonical MinutesBefore in src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceRefundPolicyServiceTests
- [X] T034 [P] [US3] Add or extend unit tests for Marketplace lock-window and booking duration calculations in src/booking/shared/Booking.Shared.UnitTests/Services/MarketplaceBookingServiceTests
- [X] T035 [P] [US3] Run the complete Booking.Shared regression suite and confirm operational duration consumers remain unchanged

### Implementation for User Story 3

- [X] T036 Confirm and document exclusion of operational/internal and read-only occurrences in specs/043-duration-display-units/research.md
- [X] T037 Verify actionable structured error logging at the Marketplace product persistence/contract boundary with focused ProductService logging tests; invalid display-unit values fail fast through the shared converter tests without routine unit-change logging
- [X] T038 Review customer-facing and operator-facing documentation paths and record why no public documentation update is required for editor-only metadata in specs/043-duration-display-units/quickstart.md

## Phase 6: Polish & Cross-Cutting Concerns

- [X] T039 [P] Rerun the repository-wide audit and reconcile specs/043-duration-display-units/research.md with the final source tree
- [X] T040 [P] Run required generation commands and verify no generated GraphQL, event, OpenAPI, or Relay artifacts are stale
- [X] T041 [P] Run focused backend unit/integration tests, web tests, linting, formatting, and git diff checks described by specs/043-duration-display-units/quickstart.md
- [X] T042 Run the complete quickstart validation and confirm all success criteria in specs/043-duration-display-units/spec.md
- [X] T043 [REGRESSION] Include all per-field display-unit values in Spaces and Host add/edit mutation payloads and add autosave tests that verify the persisted mutation mapping.

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1) precedes Foundational (Phase 2).
- Foundational (Phase 2) blocks all user stories because the audit and shared contract define scope and behavior.
- User Story 1 is the MVP and can proceed after Phase 2.
- User Story 2 depends on the shared model and contract from Phase 2 and integrates with US1 contract fields.
- User Story 3 can begin after Phase 2, but final regression validation depends on US1 and US2 changes.
- Polish depends on all selected stories.

### User Story Dependencies

- **US1 (P1)**: Depends on Phase 2; MVP.
- **US2 (P2)**: Depends on Phase 2 and consumes the Marketplace contract established by US1.
- **US3 (P3)**: Depends on Phase 2; final regression coverage should run after US1/US2.

### Parallel Opportunities

- T002, T003, T006, and T007 can run in parallel after setup.
- T009–T012 can run in parallel before US1 implementation.
- T021–T024 can run in parallel before US2 implementation.
- T033–T035 can run in parallel.
- T039–T041 can run in parallel after implementation.

## Implementation Strategy

### MVP First

1. Complete T001–T008.
2. Complete US1 (T009–T020).
3. Run the US1 independent test and stop for validation/demo.

### Incremental Delivery

1. Add US2 editor coverage and all audit-confirmed editable occurrences.
2. Add US3 calculation and exclusion regressions.
3. Complete final audit, regeneration, and quickstart validation.

### Completion Rule

The feature is not complete until the final audit is reconciled with research.md, every persisted user-editable minute/hour occurrence is implemented or explicitly tracked, and excluded operational/read-only occurrences have regression coverage or documented evidence.
