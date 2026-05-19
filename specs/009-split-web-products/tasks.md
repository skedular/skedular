# Tasks: Split Web Products

**Input**: Design documents from `/specs/009-split-web-products/`
**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/](./contracts/), [quickstart.md](./quickstart.md)

**Tests**: Tests are required by the plan for foundation and every migration slice using Vitest + React Testing Library, plus lint/build/Relay checks where applicable.

**Organization**: Tasks are grouped by foundation first, then reviewable user-story phases. Do not start a later migration phase until the previous phase checkpoint has been manually reviewed.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files and has no dependency on an incomplete task
- **[Story]**: Maps to user stories from [spec.md](./spec.md)
- Every task includes an exact file path

## Phase 1: Setup (Planning and Guard Rails)

**Purpose**: Prepare review artefacts and migration guard rails before touching app behaviour.

- [ ] T001 Create the ownership inventory document from `contracts/ownership-inventory-contract.md` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T002 Create the slice review log from `contracts/migration-slice-contract.md` in `specs/009-split-web-products/migration-slices.md`
- [ ] T003 Create the route retirement register from `contracts/route-retirement-contract.md` in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T004 Document the foundation review checklist and local inspection URLs in `specs/009-split-web-products/foundation-review.md`
- [ ] T005 [P] Add the split-web-products verification command list to `web/apps/webapp/docs/split-web-products.md`
- [ ] T006 [P] Add the split-web-products verification command list to `web/apps/webapp-spaces/docs/split-web-products.md`
- [ ] T007 [P] Add the split-web-products verification command list to `web/apps/webapp-teams/docs/split-web-products.md`

---

## Phase 2: Foundational (Blocks All Migration Slices)

**Purpose**: Make the three apps usable, buildable, testable, and ready for one-slice-at-a-time migration.

**Critical**: No app-owned journey migration starts until this phase is implemented, verified, and manually reviewed.

- [ ] T008 Add `@skedular/shared` as a workspace dependency for Spaces in `web/apps/webapp-spaces/package.json`
- [ ] T009 Add `@skedular/shared` as a workspace dependency for Teams in `web/apps/webapp-teams/package.json`
- [ ] T010 Define app identity, app type, organisation type, and customer entry type helpers in `web/packages/shared/src/app-products/app-products.ts`
- [ ] T011 Export app product helpers from `web/packages/shared/src/app-products/index.ts`
- [ ] T012 Export app product helpers from `web/packages/shared/src/index.ts`
- [ ] T013 Define neutral app shell navigation types and view model helpers in `web/packages/shared/src/app-shell/app-shell-model.ts`
- [ ] T014 Export app shell model helpers from `web/packages/shared/src/app-shell/index.ts`
- [ ] T015 Export app shell model helpers from `web/packages/shared/src/index.ts`
- [ ] T016 [P] Create reusable shell layout visual primitives in `web/packages/ui/src/app-shell/app-shell-layout.tsx`
- [ ] T017 [P] Create reusable organisation empty-state visual primitive in `web/packages/ui/src/app-shell/organisation-empty-state.tsx`
- [ ] T018 [P] Create reusable app review banner visual primitive in `web/packages/ui/src/app-shell/app-review-banner.tsx`
- [ ] T019 Export app shell visual primitives from `web/packages/ui/src/app-shell/index.ts`
- [ ] T020 Export app shell visual primitives from `web/packages/ui/src/index.ts`
- [ ] T021 Add app-shell model unit tests for app identity and organisation type helpers in `web/packages/shared/src/app-shell/__tests__/app-shell-model.test.ts`
- [ ] T022 Add app-products unit tests for Teams/Spaces/WebApp ownership rules in `web/packages/shared/src/app-products/__tests__/app-products.test.ts`
- [ ] T023 Add app shell visual primitive render tests in `web/packages/ui/src/app-shell/__tests__/app-shell-layout.test.tsx`
- [ ] T024 Update WebApp root layout to use shared providers and app identity in `web/apps/webapp/src/app/layout.tsx`
- [ ] T025 Update Spaces root layout to use shared providers and app identity in `web/apps/webapp-spaces/src/app/layout.tsx`
- [ ] T026 Update Teams root layout to use shared providers and app identity in `web/apps/webapp-teams/src/app/layout.tsx`
- [ ] T027 Replace the empty Spaces home page with a reviewable foundation shell in `web/apps/webapp-spaces/src/app/page.tsx`
- [ ] T028 Replace the empty Teams home page with a reviewable foundation shell in `web/apps/webapp-teams/src/app/page.tsx`
- [ ] T029 Add WebApp foundation shell review affordances without changing customer routing in `web/apps/webapp/src/app/page.tsx`
- [ ] T030 Add Spaces foundation render test in `web/apps/webapp-spaces/src/app/page.test.tsx`
- [ ] T031 Add Teams foundation render test in `web/apps/webapp-teams/src/app/page.test.tsx`
- [ ] T032 Add WebApp foundation render test in `web/apps/webapp/src/app/page.test.tsx`
- [ ] T033 Add app selection diagnostics helper in `web/packages/shared/src/app-shell/app-selection-logger.ts`
- [ ] T034 Add app selection diagnostics tests in `web/packages/shared/src/app-shell/__tests__/app-selection-logger.test.ts`
- [ ] T035 Run foundation verification commands from `quickstart.md` and record results in `specs/009-split-web-products/foundation-review.md`
- [ ] T036 Stop for manual foundation review and record approval or blockers in `specs/009-split-web-products/foundation-review.md`

**Checkpoint**: Foundation ready. The user must be able to run and inspect WebApp, WebApp Spaces, and WebApp Teams before migration slices begin.

---

## Phase 3: User Story 1 - Classify Existing Product Journeys (Priority: P1)

**Goal**: Produce a reviewable ownership inventory before moving any real feature journey.

**Independent Test**: A reviewer can open `ownership-inventory.md` and see every targeted route/module classified with one owner, route risk, Relay impact, and verification scope.

### Tests for User Story 1

- [ ] T037 [P] [US1] Add inventory schema validation tests in `web/packages/shared/src/app-migration/__tests__/ownership-inventory.test.ts`
- [ ] T038 [P] [US1] Add route retirement schema validation tests in `web/packages/shared/src/app-migration/__tests__/route-retirement.test.ts`

### Implementation for User Story 1

- [ ] T039 [US1] Create ownership inventory schema helpers in `web/packages/shared/src/app-migration/ownership-inventory.ts`
- [ ] T040 [US1] Create route retirement schema helpers in `web/packages/shared/src/app-migration/route-retirement.ts`
- [ ] T041 [US1] Export migration schema helpers from `web/packages/shared/src/app-migration/index.ts`
- [ ] T042 [US1] Export migration schema helpers from `web/packages/shared/src/index.ts`
- [ ] T043 [US1] Inventory WebApp route ownership for `web/apps/webapp/src/app` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T044 [US1] Inventory WebApp root page ownership for `web/apps/webapp/src/rootPages` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T045 [US1] Inventory app component ownership for `web/apps/webapp/src/components` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T046 [US1] Inventory shared package candidates for `web/packages/ui/src` and `web/packages/shared/src` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T047 [US1] Inventory Relay and OpenAPI generated artefact impact for `web/apps/webapp/src/queries` and `web/apps/webapp/src/clients` in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T048 [US1] Inventory backend-originated return URL risks for WebApp routes in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T049 [US1] Propose migration slice order from the inventory in `specs/009-split-web-products/migration-slices.md`
- [ ] T050 [US1] Stop for manual ownership review and record approval or ownership corrections in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: Ownership is reviewed before code movement.

---

## Phase 4: User Story 2 - Move App-Owned Code to Target Apps (Priority: P1)

**Goal**: Establish the repeatable migration mechanics using one small reviewed slice before moving larger app journeys.

**Independent Test**: One small slice can be moved to its owner, verified in the target app, and marked accepted in `migration-slices.md`.

### Tests for User Story 2

- [ ] T051 [P] [US2] Add migration slice lifecycle tests in `web/packages/shared/src/app-migration/__tests__/migration-slice.test.ts`
- [ ] T052 [P] [US2] Add transition path validation tests in `web/packages/shared/src/app-migration/__tests__/transition-path.test.ts`

### Implementation for User Story 2

- [ ] T053 [US2] Create migration slice lifecycle helpers in `web/packages/shared/src/app-migration/migration-slice.ts`
- [ ] T054 [US2] Create transition path helpers in `web/packages/shared/src/app-migration/transition-path.ts`
- [ ] T055 [US2] Export migration slice helpers from `web/packages/shared/src/app-migration/index.ts`
- [ ] T056 [US2] Select the first low-risk pilot slice from `ownership-inventory.md` and document scope in `specs/009-split-web-products/migration-slices.md`
- [ ] T057 [US2] Move the pilot slice app-owned files to the target app path identified in `specs/009-split-web-products/migration-slices.md`
- [ ] T058 [US2] Move pilot slice neutral UI/runtime helpers to `web/packages/ui/src` or `web/packages/shared/src` as classified in `ownership-inventory.md`
- [ ] T059 [US2] Update imports for the pilot slice in the source and target paths documented in `specs/009-split-web-products/migration-slices.md`
- [ ] T060 [US2] Apply keep/redirect/block/delete/transition route action for the pilot slice in the old WebApp path documented in `route-retirement-register.md`
- [ ] T061 [US2] Run the pilot slice lint/test/build checks and record results in `specs/009-split-web-products/migration-slices.md`
- [ ] T062 [US2] Stop for manual pilot-slice review and record acceptance or blockers in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: The migration loop has been proven on one small slice.

---

## Phase 5: User Story 5 - Extract Enterprise and Private Organisation Experiences to WebApp Teams (Priority: P1)

**Goal**: Make WebApp Teams the private organisation app with private organisation selection only and no marketplace concepts.

**Independent Test**: A signed-in Teams user can see only private organisations or an empty state, and Teams routes do not expose marketplace organisation or marketplace product concepts.

### Tests for User Story 5

- [ ] T063 [P] [US5] Add Teams organisation filtering tests in `web/apps/webapp-teams/src/app/organization-selection/organization-selection.test.tsx`
- [ ] T064 [P] [US5] Add Teams no-marketplace-concepts route tests in `web/apps/webapp-teams/src/app/page.test.tsx`
- [ ] T065 [P] [US5] Add Teams empty organisation state tests in `web/apps/webapp-teams/src/app/organization-selection/empty-state.test.tsx`

### Implementation for User Story 5

- [ ] T066 [US5] Create Teams organisation selection route in `web/apps/webapp-teams/src/app/organization-selection/page.tsx`
- [ ] T067 [US5] Create Teams organisation filtering helper in `web/apps/webapp-teams/src/app/organization-selection/private-organization-filter.ts`
- [ ] T068 [US5] Create Teams organisation empty state in `web/apps/webapp-teams/src/app/organization-selection/empty-state.tsx`
- [ ] T069 [US5] Wire Teams home page to private organisation selection in `web/apps/webapp-teams/src/app/page.tsx`
- [ ] T070 [US5] Add Teams route diagnostics for selected app and organisation filter result in `web/apps/webapp-teams/src/app/organization-selection/organization-selection-logger.ts`
- [ ] T071 [US5] Move the first reviewed private organisation journey from `web/apps/webapp/src` into `web/apps/webapp-teams/src` according to `migration-slices.md`
- [ ] T072 [US5] Update Teams imports for moved private organisation code in `web/apps/webapp-teams/src`
- [ ] T073 [US5] Remove, redirect, block, or document the old WebApp path for the moved Teams journey in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T074 [US5] Run Teams slice verification and record commands/results in `specs/009-split-web-products/migration-slices.md`
- [ ] T075 [US5] Stop for manual WebApp Teams review and record acceptance or blockers in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: Teams foundation and first private-organisation slice are reviewed before Spaces work starts.

---

## Phase 6: User Story 4 - Extract Marketplace Organisation Experiences to WebApp Spaces (Priority: P1)

**Goal**: Make WebApp Spaces the marketplace/co-working operator app with marketplace/co-working organisation selection only.

**Independent Test**: A signed-in Spaces user can see only marketplace/co-working organisations or an empty state, and Spaces routes do not expose private organisation/team-only journeys.

### Tests for User Story 4

- [ ] T076 [P] [US4] Add Spaces organisation filtering tests in `web/apps/webapp-spaces/src/app/organization-selection/organization-selection.test.tsx`
- [ ] T077 [P] [US4] Add Spaces no-private-journeys route tests in `web/apps/webapp-spaces/src/app/page.test.tsx`
- [ ] T078 [P] [US4] Add Spaces empty organisation state tests in `web/apps/webapp-spaces/src/app/organization-selection/empty-state.test.tsx`

### Implementation for User Story 4

- [ ] T079 [US4] Create Spaces organisation selection route in `web/apps/webapp-spaces/src/app/organization-selection/page.tsx`
- [ ] T080 [US4] Create Spaces organisation filtering helper in `web/apps/webapp-spaces/src/app/organization-selection/marketplace-organization-filter.ts`
- [ ] T081 [US4] Create Spaces organisation empty state in `web/apps/webapp-spaces/src/app/organization-selection/empty-state.tsx`
- [ ] T082 [US4] Wire Spaces home page to marketplace/co-working organisation selection in `web/apps/webapp-spaces/src/app/page.tsx`
- [ ] T083 [US4] Add Spaces route diagnostics for selected app and organisation filter result in `web/apps/webapp-spaces/src/app/organization-selection/organization-selection-logger.ts`
- [ ] T084 [US4] Move the first reviewed marketplace organisation journey from `web/apps/webapp/src` into `web/apps/webapp-spaces/src` according to `migration-slices.md`
- [ ] T085 [US4] Update Spaces imports for moved marketplace organisation code in `web/apps/webapp-spaces/src`
- [ ] T086 [US4] Remove, redirect, block, or document the old WebApp path for the moved Spaces journey in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T087 [US4] Run Spaces slice verification and record commands/results in `specs/009-split-web-products/migration-slices.md`
- [ ] T088 [US4] Stop for manual WebApp Spaces review and record acceptance or blockers in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: Spaces foundation and first marketplace-organisation slice are reviewed before WebApp customer-facing slices start.

---

## Phase 7: User Story 3 - Extract Customer-Facing Experiences to WebApp (Priority: P1)

**Goal**: Keep WebApp customer-facing with root public discovery, co-working subdomain experiences, and private organisation customer-facing subdomain experiences.

**Independent Test**: A reviewer can inspect WebApp root, co-working subdomain path, and private organisation subdomain path and see only the intended customer-facing scope.

### Tests for User Story 3

- [ ] T089 [P] [US3] Add WebApp root discovery tests in `web/apps/webapp/src/app/page.test.tsx`
- [ ] T090 [P] [US3] Add co-working subdomain customer-facing tests in `web/apps/webapp/src/app/customer-facing-subdomain/co-working-subdomain.test.tsx`
- [ ] T091 [P] [US3] Add private organisation subdomain customer-facing tests in `web/apps/webapp/src/app/customer-facing-subdomain/private-organization-subdomain.test.tsx`

### Implementation for User Story 3

- [ ] T092 [US3] Define customer-facing entry point model in `web/apps/webapp/src/app/customer-facing-subdomain/customer-facing-entry-point.ts`
- [ ] T093 [US3] Create customer-facing subdomain resolver in `web/apps/webapp/src/app/customer-facing-subdomain/customer-facing-subdomain-resolver.ts`
- [ ] T094 [US3] Create co-working customer-facing subdomain shell in `web/apps/webapp/src/app/customer-facing-subdomain/co-working-subdomain.tsx`
- [ ] T095 [US3] Create private organisation customer-facing subdomain shell in `web/apps/webapp/src/app/customer-facing-subdomain/private-organization-subdomain.tsx`
- [ ] T096 [US3] Keep WebApp root URL focused on public marketplace discovery in `web/apps/webapp/src/app/page.tsx`
- [ ] T097 [US3] Remove private organisation admin and marketplace operator navigation from WebApp customer-facing surfaces in `web/apps/webapp/src/components/navigationMenu`
- [ ] T098 [US3] Add WebApp customer-facing route diagnostics in `web/apps/webapp/src/app/customer-facing-subdomain/customer-facing-logger.ts`
- [ ] T099 [US3] Audit WebApp root and subdomain return URL risks in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T100 [US3] Run WebApp customer-facing verification and record commands/results in `specs/009-split-web-products/migration-slices.md`
- [ ] T101 [US3] Stop for manual WebApp customer-facing review and record acceptance or blockers in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: WebApp customer-facing scope is reviewed before broader shared extraction and cleanup.

---

## Phase 8: User Story 6 - Preserve and Share Common Product Foundations (Priority: P2)

**Goal**: Move only genuinely neutral UI/runtime foundations into shared packages without moving app-specific rules into shared code.

**Independent Test**: At least one repeated neutral UI/runtime foundation is consumed by two apps from `@skedular/ui` or `@skedular/shared`, while app-specific copy/rules stay in the owning app.

### Tests for User Story 6

- [ ] T102 [P] [US6] Add shared UI consumer tests for extracted neutral components in `web/packages/ui/src/app-shell/__tests__/shared-consumption.test.tsx`
- [ ] T103 [P] [US6] Add shared runtime consumer tests for extracted neutral helpers in `web/packages/shared/src/app-shell/__tests__/shared-consumption.test.ts`
- [ ] T104 [P] [US6] Add no-app-rules-in-shared guard tests in `web/packages/shared/src/app-migration/__tests__/shared-boundary.test.ts`

### Implementation for User Story 6

- [ ] T105 [US6] Move first reviewed neutral visual component from an app into `web/packages/ui/src/app-shell`
- [ ] T106 [US6] Move first reviewed neutral hook or utility from an app into `web/packages/shared/src/app-shell`
- [ ] T107 [US6] Update WebApp imports for shared foundations in `web/apps/webapp/src`
- [ ] T108 [US6] Update Spaces imports for shared foundations in `web/apps/webapp-spaces/src`
- [ ] T109 [US6] Update Teams imports for shared foundations in `web/apps/webapp-teams/src`
- [ ] T110 [US6] Remove duplicated neutral implementations from app paths documented in `ownership-inventory.md`
- [ ] T111 [US6] Record shared extraction decisions and rejected broad feature-module sharing in `specs/009-split-web-products/ownership-inventory.md`
- [ ] T112 [US6] Run shared package and affected app verification and record commands/results in `specs/009-split-web-products/migration-slices.md`
- [ ] T113 [US6] Stop for manual shared-foundation review and record acceptance or blockers in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: Shared foundations are reviewed before final transition cleanup.

---

## Phase 9: User Story 7 - Maintain Transition Safety (Priority: P2)

**Goal**: Keep route retirement, backend-originated return URL handling, verification, and manual acceptance safe for every completed slice.

**Independent Test**: Every retired/deleted route has a recorded return URL audit, transition decision, verification result, and review status.

### Tests for User Story 7

- [ ] T114 [P] [US7] Add route retirement register validation tests in `web/packages/shared/src/app-migration/__tests__/route-retirement-register.test.ts`
- [ ] T115 [P] [US7] Add backend-originated return URL audit helper tests in `web/packages/shared/src/app-migration/__tests__/return-url-audit.test.ts`

### Implementation for User Story 7

- [ ] T116 [US7] Create backend-originated return URL audit helper in `web/packages/shared/src/app-migration/return-url-audit.ts`
- [ ] T117 [US7] Export return URL audit helper from `web/packages/shared/src/app-migration/index.ts`
- [ ] T118 [US7] Complete route retirement entries for all completed slices in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T119 [US7] Document unresolved return URL blockers as transition paths in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T120 [US7] Verify no route deletion is recorded without a return URL audit in `specs/009-split-web-products/route-retirement-register.md`
- [ ] T121 [US7] Verify each completed slice has lint/test/build/manual review results in `specs/009-split-web-products/migration-slices.md`
- [ ] T122 [US7] Run full quickstart verification and record final results in `specs/009-split-web-products/migration-slices.md`

**Checkpoint**: Transition safety artefacts are complete for all implemented slices.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Final consistency checks after selected slices are accepted.

- [ ] T123 [P] Update implementation notes and any changed manual URLs in `specs/009-split-web-products/quickstart.md`
- [ ] T124 [P] Update app ownership guidance in `web/apps/webapp/docs/split-web-products.md`
- [ ] T125 [P] Update app ownership guidance in `web/apps/webapp-spaces/docs/split-web-products.md`
- [ ] T126 [P] Update app ownership guidance in `web/apps/webapp-teams/docs/split-web-products.md`
- [ ] T127 Run `pnpm webapp#lint` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T128 Run `pnpm webapp#test` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T129 Run `pnpm webapp#build` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T130 Run `pnpm webapp-spaces#lint` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T131 Run `pnpm webapp-spaces#test` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T132 Run `pnpm webapp-spaces#build` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T133 Run `pnpm webapp-teams#lint` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T134 Run `pnpm webapp-teams#test` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T135 Run `pnpm webapp-teams#build` and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T136 Run affected Relay generation commands from each app's `package.json` when GraphQL operations moved and record result in `specs/009-split-web-products/migration-slices.md`
- [ ] T137 Perform final British English copy review for changed app files in `web/apps/webapp/src`, `web/apps/webapp-spaces/src`, and `web/apps/webapp-teams/src`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 Setup**: No dependencies.
- **Phase 2 Foundational**: Depends on Phase 1; blocks every migration story.
- **Phase 3 US1 Ownership Inventory**: Depends on foundation approval.
- **Phase 4 US2 Pilot Migration Loop**: Depends on US1 ownership review.
- **Phase 5 US5 Teams**: Depends on accepted pilot slice.
- **Phase 6 US4 Spaces**: Depends on accepted Teams slice.
- **Phase 7 US3 WebApp Customer-Facing**: Depends on accepted Spaces slice.
- **Phase 8 US6 Shared Foundations**: Depends on accepted initial app slices.
- **Phase 9 US7 Transition Safety**: Runs after each slice but must be completed before final cleanup.
- **Final Phase**: Depends on selected slices being accepted.

### User Story Dependencies

- **US1**: First after foundation; creates the inventory and migration order.
- **US2**: Proves the migration mechanics with one low-risk slice.
- **US5**: First app-specific migration checkpoint; Teams private organisation only.
- **US4**: Second app-specific migration checkpoint; Spaces marketplace/co-working only.
- **US3**: Third app-specific migration checkpoint; WebApp customer-facing only.
- **US6**: Shared extraction after app boundaries are proven.
- **US7**: Transition safety applied throughout, completed after selected slices.

### Manual Review Gates

- Complete T036 before starting T037.
- Complete T050 before starting T051.
- Complete T062 before starting T063.
- Complete T075 before starting T076.
- Complete T088 before starting T089.
- Complete T101 before starting T102.
- Complete T113 before final transition cleanup.

---

## Parallel Opportunities

- T005-T007 can run in parallel after T004.
- T016-T018 can run in parallel after T010-T015.
- T021-T023 can run in parallel after shared app shell files exist.
- T037-T038 can run in parallel.
- T051-T052 can run in parallel.
- T063-T065 can run in parallel.
- T076-T078 can run in parallel.
- T089-T091 can run in parallel.
- T102-T104 can run in parallel.
- T114-T115 can run in parallel.
- T123-T126 can run in parallel.

## Parallel Example: Foundation

```text
Task: "Create reusable shell layout visual primitives in web/packages/ui/src/app-shell/app-shell-layout.tsx"
Task: "Create reusable organisation empty-state visual primitive in web/packages/ui/src/app-shell/organisation-empty-state.tsx"
Task: "Create reusable app review banner visual primitive in web/packages/ui/src/app-shell/app-review-banner.tsx"
```

## Parallel Example: WebApp Teams Slice

```text
Task: "Add Teams organisation filtering tests in web/apps/webapp-teams/src/app/organization-selection/organization-selection.test.tsx"
Task: "Add Teams no-marketplace-concepts route tests in web/apps/webapp-teams/src/app/page.test.tsx"
Task: "Add Teams empty organisation state tests in web/apps/webapp-teams/src/app/organization-selection/empty-state.test.tsx"
```

## Implementation Strategy

### MVP First

1. Complete Phase 1 setup.
2. Complete Phase 2 foundation.
3. Stop for manual review at T036.
4. Do not move any real journey until the foundation is accepted.

### Incremental Delivery

1. Build the app foundation.
2. Build ownership inventory.
3. Prove one low-risk pilot migration.
4. Move Teams slice and stop for review.
5. Move Spaces slice and stop for review.
6. Move WebApp customer-facing slice and stop for review.
7. Extract shared foundations only after app-owned boundaries are proven.
8. Complete transition safety and final verification.

### Safety Rules

- Do not change backend services, backend APIs, backend contracts, backend data ownership, or `api-definitions/`.
- Do not delete a WebApp route until `route-retirement-register.md` shows backend-originated return URL usage is safe.
- Do not move marketplace organisation or marketplace product concepts into WebApp Teams.
- Do not move app-specific rules into `@skedular/ui` or `@skedular/shared`.
- Do not proceed past a manual checkpoint until the user has reviewed the app or slice.
