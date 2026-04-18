# Tasks: Split UI into Three Products

**Input**: Design documents from `/specs/002-split-ui-products/`
**Prerequisites**: plan.md (required), spec.md (required), research.md, data-model.md, contracts/, quickstart.md
**Tests**: Include validation and smoke checks requested by the feature specification
**Organization**: Tasks are grouped by user story so each story is independently implementable and testable

## Format: `[ID] [P?] [Story?] Description with file path`

---

## Phase 1: Setup (Project Initialization)

**Purpose**: Create missing app roots and baseline metadata for new products.

- [x] T001 Create project roots `web/apps/webapp-teams/` and `web/apps/webapp-spaces/`
- [x] T002 [P] Create health project roots `web/apps/webapp-teams-help/` and `web/apps/webapp-spaces-help/`
- [x] T003 [P] Create app docs folders `web/apps/webapp-teams/docs/` and `web/apps/webapp-spaces/docs/`
- [x] T004 [P] Create health docs folders `web/apps/webapp-teams-help/docs/` and `web/apps/webapp-spaces-help/docs/`
- [x] T005 [P] Add project overview in `web/apps/webapp-teams/README.md`
- [x] T006 [P] Add project overview in `web/apps/webapp-spaces/README.md`
- [x] T007 [P] Add health overview in `web/apps/webapp-teams-help/README.md`
- [x] T008 [P] Add health overview in `web/apps/webapp-spaces-help/README.md`
- [x] T009 [P] Add feature ops notes in `specs/002-split-ui-products/contracts/README.md`
- [x] T010 Add feature implementation notes in `specs/002-split-ui-products/quickstart.md`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared, blocking prerequisites before any story-specific scaffolding.

**⚠️ CRITICAL**: No user story work should start before this phase completes.

- [x] T011 Create design-system version policy script in `scripts/verify-ui-package-versions.sh`
- [x] T012 [P] Create product structure parity script in `scripts/validate-three-products.sh`
- [x] T013 [P] Create terraform workspace parity script in `scripts/validate-workspace-layout.sh`
- [x] T014 [P] Define logging checklist in `specs/002-split-ui-products/docs/logging-checks.md`
- [x] T015 [P] Define deployment-time measurement checklist in `specs/002-split-ui-products/docs/deployment-sla-checks.md`
- [x] T016 [P] Define shared environment variable contract in `specs/002-split-ui-products/docs/env-contract.md`
- [x] T017 [P] Document repo-level workflow strategy in `specs/002-split-ui-products/docs/workflow-strategy.md`
- [x] T018 [P] Document S3 backend key strategy in `specs/002-split-ui-products/docs/backend-key-strategy.md`
- [x] T019 Run version policy script against `web/apps/webapp/package.json`
- [x] T020 Run structure parity script against `web/apps/webapp/`
- [x] T021 Run workspace parity script against `web/apps/webapp/infrastructure/workspaces/`
- [x] T022 Record foundational baseline report in `specs/002-split-ui-products/docs/foundation-baseline.md`

**Checkpoint**: Foundation complete; user stories can proceed.

---

## Phase 3: User Story 1 - Scaffold teams web app Project Structure (Priority: P1)

**Goal**: Create `webapp-teams` by mirroring current webapp structure and infrastructure with isolated state keys.

**Independent Test**: `web/apps/webapp-teams` installs, builds, and all workspaces validate with backend disabled.

### Implementation for User Story 1

- [x] T023 [P] [US1] Copy app config files to `web/apps/webapp-teams/` (`package.json`, `tsconfig.json`, `next.config.js`, `eslint.config.*`)
- [x] T024 [P] [US1] Copy source tree from `web/apps/webapp/src/` to `web/apps/webapp-teams/src/`
- [x] T025 [P] [US1] Copy static assets from `web/apps/webapp/public/` to `web/apps/webapp-teams/public/`
- [x] T026 [P] [US1] Copy infra modules from `web/apps/webapp/infrastructure/modules/` to `web/apps/webapp-teams/infrastructure/modules/`
- [x] T027 [P] [US1] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/staging/` to `web/apps/webapp-teams/infrastructure/workspaces/staging/`
- [x] T028 [P] [US1] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/common_resources/` to `web/apps/webapp-teams/infrastructure/workspaces/common_resources/`
- [x] T029 [P] [US1] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/production/` to `web/apps/webapp-teams/infrastructure/workspaces/production/`
- [x] T030 [US1] Update app identity fields in `web/apps/webapp-teams/package.json`
- [x] T031 [US1] Update app identity env values in `web/apps/webapp-teams/.env.example`
- [x] T032 [US1] Update service name logging tags in `web/apps/webapp-teams/src/` startup/logger files
- [x] T033 [US1] Update backend key in `web/apps/webapp-teams/infrastructure/workspaces/staging/backend_webapp.tf`
- [x] T034 [US1] Update backend key in `web/apps/webapp-teams/infrastructure/workspaces/common_resources/terraform.tf`
- [x] T035 [US1] Update backend key in `web/apps/webapp-teams/infrastructure/workspaces/production/backend_webapp.tf`
- [x] T036 [US1] Align workspace locals in `web/apps/webapp-teams/infrastructure/workspaces/staging/locals.tf`
- [x] T037 [US1] Align workspace locals in `web/apps/webapp-teams/infrastructure/workspaces/common_resources/locals.tf`
- [x] T038 [US1] Align workspace locals in `web/apps/webapp-teams/infrastructure/workspaces/production/locals.tf`
- [x] T039 [US1] Validate package install with `pnpm --filter webapp-teams install` using `web/apps/webapp-teams/package.json`
- [x] T040 [US1] Validate build with `pnpm --filter webapp-teams build` using `web/apps/webapp-teams/package.json`
- [x] T041 [US1] Validate lint with `pnpm --filter webapp-teams lint` using `web/apps/webapp-teams/package.json`
- [x] T042 [US1] Validate terraform in `web/apps/webapp-teams/infrastructure/workspaces/staging/`
- [x] T043 [US1] Validate terraform in `web/apps/webapp-teams/infrastructure/workspaces/common_resources/` and `web/apps/webapp-teams/infrastructure/workspaces/production/`

**Checkpoint**: `webapp-teams` is scaffolded and independently verifiable.

---

## Phase 4: User Story 2 - Scaffold spaces web app Project Structure (Priority: P1)

**Goal**: Create `webapp-spaces` by mirroring current webapp structure and infrastructure with isolated state keys.

**Independent Test**: `web/apps/webapp-spaces` installs, builds, and all workspaces validate with backend disabled.

### Implementation for User Story 2

- [x] T044 [P] [US2] Copy app config files to `web/apps/webapp-spaces/` (`package.json`, `tsconfig.json`, `next.config.js`, `eslint.config.*`)
- [x] T045 [P] [US2] Copy source tree from `web/apps/webapp/src/` to `web/apps/webapp-spaces/src/`
- [x] T046 [P] [US2] Copy static assets from `web/apps/webapp/public/` to `web/apps/webapp-spaces/public/`
- [x] T047 [P] [US2] Copy infra modules from `web/apps/webapp/infrastructure/modules/` to `web/apps/webapp-spaces/infrastructure/modules/`
- [x] T048 [P] [US2] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/staging/` to `web/apps/webapp-spaces/infrastructure/workspaces/staging/`
- [x] T049 [P] [US2] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/common_resources/` to `web/apps/webapp-spaces/infrastructure/workspaces/common_resources/`
- [x] T050 [P] [US2] Copy workspace files from `web/apps/webapp/infrastructure/workspaces/production/` to `web/apps/webapp-spaces/infrastructure/workspaces/production/`
- [x] T051 [US2] Update app identity fields in `web/apps/webapp-spaces/package.json`
- [x] T052 [US2] Update app identity env values in `web/apps/webapp-spaces/.env.example`
- [x] T053 [US2] Update service name logging tags in `web/apps/webapp-spaces/src/` startup/logger files
- [x] T054 [US2] Update backend key in `web/apps/webapp-spaces/infrastructure/workspaces/staging/backend_webapp.tf`
- [x] T055 [US2] Update backend key in `web/apps/webapp-spaces/infrastructure/workspaces/common_resources/terraform.tf`
- [x] T056 [US2] Update backend key in `web/apps/webapp-spaces/infrastructure/workspaces/production/backend_webapp.tf`
- [x] T057 [US2] Align workspace locals in `web/apps/webapp-spaces/infrastructure/workspaces/staging/locals.tf`
- [x] T058 [US2] Align workspace locals in `web/apps/webapp-spaces/infrastructure/workspaces/common_resources/locals.tf`
- [x] T059 [US2] Align workspace locals in `web/apps/webapp-spaces/infrastructure/workspaces/production/locals.tf`
- [x] T060 [US2] Validate package install with `pnpm --filter webapp-spaces install` using `web/apps/webapp-spaces/package.json`
- [x] T061 [US2] Validate build with `pnpm --filter webapp-spaces build` using `web/apps/webapp-spaces/package.json`
- [x] T062 [US2] Validate lint with `pnpm --filter webapp-spaces lint` using `web/apps/webapp-spaces/package.json`
- [x] T063 [US2] Validate terraform in `web/apps/webapp-spaces/infrastructure/workspaces/staging/`
- [x] T064 [US2] Validate terraform in `web/apps/webapp-spaces/infrastructure/workspaces/common_resources/` and `web/apps/webapp-spaces/infrastructure/workspaces/production/`

**Checkpoint**: `webapp-spaces` is scaffolded and independently verifiable.

---

## Phase 5: User Story 5 - Configure Shared Design System Integration (Priority: P1)

**Goal**: Enforce identical shared UI package usage across existing, private, and spaces apps.

**Independent Test**: Version check script passes and each app renders using shared UI components without dependency drift.

### Implementation for User Story 5

- [x] T065 [US5] Verify shared UI dependency key in `web/apps/webapp/package.json`
- [x] T066 [P] [US5] Verify shared UI dependency key in `web/apps/webapp-teams/package.json`
- [x] T067 [P] [US5] Verify shared UI dependency key in `web/apps/webapp-spaces/package.json`
- [x] T068 [US5] Pin shared UI version in `web/apps/webapp/package.json`, `web/apps/webapp-teams/package.json`, and `web/apps/webapp-spaces/package.json`
- [x] T069 [P] [US5] Verify shared UI imports in `web/apps/webapp/src/`
- [x] T070 [P] [US5] Verify shared UI imports in `web/apps/webapp-teams/src/`
- [x] T071 [P] [US5] Verify shared UI imports in `web/apps/webapp-spaces/src/`
- [x] T072 [US5] Verify typography wrapper usage in `web/apps/webapp/src/components/`
- [x] T073 [P] [US5] Verify typography wrapper usage in `web/apps/webapp-teams/src/components/`
- [x] T074 [P] [US5] Verify typography wrapper usage in `web/apps/webapp-spaces/src/components/`
- [x] T075 [US5] Run UI version sync check using `scripts/verify-ui-package-versions.sh`
- [x] T076 [US5] Document UI dependency policy in `specs/002-split-ui-products/docs/design-system-integration.md`

**Checkpoint**: Shared UI integration is consistent across all three main apps.

---

## Phase 6: User Story 3 - Create Health Project for teams web app (Priority: P2)

**Goal**: Create `webapp-teams-help` by mirroring `webapphelp` structure and wiring it to teams app identity.

**Independent Test**: Health app builds and its infra workspaces validate independently.

### Implementation for User Story 3

- [x] T077 [P] [US3] Copy health config files to `web/apps/webapp-teams-help/` from `web/apps/webapphelp/`
- [x] T078 [P] [US3] Copy health source tree from `web/apps/webapphelp/src/` to `web/apps/webapp-teams-help/src/`
- [x] T079 [P] [US3] Copy health assets from `web/apps/webapphelp/public/` to `web/apps/webapp-teams-help/public/`
- [x] T080 [P] [US3] Copy health infra modules from `web/apps/webapphelp/infrastructure/modules/` to `web/apps/webapp-teams-help/infrastructure/modules/`
- [x] T081 [P] [US3] Copy health workspace trees from `web/apps/webapphelp/infrastructure/workspaces/` to `web/apps/webapp-teams-help/infrastructure/workspaces/`
- [x] T082 [US3] Update package identity in `web/apps/webapp-teams-help/package.json`
- [x] T083 [US3] Verify env identity scope (source template has no `web/apps/webapphelp/.env.example`)
- [x] T084 [US3] Update backend/workspace identity in `web/apps/webapp-teams-help/infrastructure/workspaces/`
- [x] T085 [US3] Verify log service identifier scope in `web/apps/webapp-teams-help/src/` (no dedicated startup logger file in source template)
- [x] T086 [US3] Validate build with `pnpm --filter webapp-teams-help build` using `web/apps/webapp-teams-help/package.json`
- [x] T087 [US3] Validate terraform in `web/apps/webapp-teams-help/infrastructure/workspaces/staging/`
- [x] T088 [US3] Validate terraform in `web/apps/webapp-teams-help/infrastructure/workspaces/common_resources/` and `web/apps/webapp-teams-help/infrastructure/workspaces/production/`

**Checkpoint**: `webapp-teams-help` is scaffolded and independently verifiable.

---

## Phase 7: User Story 4 - Create Health Project for spaces web app (Priority: P2)

**Goal**: Create `webapp-spaces-help` by mirroring `webapphelp` structure and wiring it to spaces app identity.

**Independent Test**: Health app builds and its infra workspaces validate independently.

### Implementation for User Story 4

- [x] T089 [P] [US4] Copy health config files to `web/apps/webapp-spaces-help/` from `web/apps/webapphelp/`
- [x] T090 [P] [US4] Copy health source tree from `web/apps/webapphelp/src/` to `web/apps/webapp-spaces-help/src/`
- [x] T091 [P] [US4] Copy health assets from `web/apps/webapphelp/public/` to `web/apps/webapp-spaces-help/public/`
- [x] T092 [P] [US4] Copy health infra modules from `web/apps/webapphelp/infrastructure/modules/` to `web/apps/webapp-spaces-help/infrastructure/modules/`
- [x] T093 [P] [US4] Copy health workspace trees from `web/apps/webapphelp/infrastructure/workspaces/` to `web/apps/webapp-spaces-help/infrastructure/workspaces/`
- [x] T094 [US4] Update package identity in `web/apps/webapp-spaces-help/package.json`
- [x] T095 [US4] Verify env identity scope (source template has no `web/apps/webapphelp/.env.example`)
- [x] T096 [US4] Update backend/workspace identity in `web/apps/webapp-spaces-help/infrastructure/workspaces/`
- [x] T097 [US4] Verify log service identifier scope in `web/apps/webapp-spaces-help/src/` (no dedicated startup logger file in source template)
- [x] T098 [US4] Validate build with `pnpm --filter webapp-spaces-help build` using `web/apps/webapp-spaces-help/package.json`
- [x] T099 [US4] Validate terraform in `web/apps/webapp-spaces-help/infrastructure/workspaces/staging/`
- [x] T100 [US4] Validate terraform in `web/apps/webapp-spaces-help/infrastructure/workspaces/common_resources/` and `web/apps/webapp-spaces-help/infrastructure/workspaces/production/`

**Checkpoint**: `webapp-spaces-help` is scaffolded and independently verifiable.

---

## Phase 8: Polish & Cross-Cutting Concerns

**Purpose**: Finalise CI wiring, observability checks, documentation, and release readiness.

- [x] T101 [P] Add teams app pipeline entry in `.github/workflows/webapp-teams.yml`
- [x] T102 [P] Add spaces app pipeline entry in `.github/workflows/webapp-spaces.yml`
- [x] T103 [P] Add teams health pipeline entry in `.github/workflows/webapp-teams-help.yml`
- [x] T104 [P] Add spaces health pipeline entry in `.github/workflows/webapp-spaces-help.yml`
- [x] T105 [P] Align shared workflow usage in `.github/workflows/web-shared.yml`
- [x] T106 [P] Add secrets/vars matrix documentation in `specs/002-split-ui-products/docs/github-actions-secrets.md`
- [x] T107 [P] Add observability runbook in `specs/002-split-ui-products/docs/observability.md`
- [x] T108 [P] Add deployment runbook in `specs/002-split-ui-products/docs/deployment-runbook.md`
- [x] T109 [P] Add product topology guide in `web/apps/README.md`
- [x] T110 [P] Run quickstart validation steps in `specs/002-split-ui-products/quickstart.md`
- [x] T111 [P] Measure and document deployment duration in `specs/002-split-ui-products/docs/deployment-sla-results.md`
- [x] T112 Finalise completion report in `specs/002-split-ui-products/COMPLETION.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: Start immediately.
- **Phase 2 (Foundational)**: Depends on Phase 1 and blocks all user stories.
- **Phase 3 (US1)**: Depends on Phase 2.
- **Phase 4 (US2)**: Depends on Phase 2; can run in parallel with US1.
- **Phase 5 (US5)**: Depends on US1 and US2.
- **Phase 6 (US3)**: Depends on US1.
- **Phase 7 (US4)**: Depends on US2.
- **Phase 8 (Polish)**: Depends on completion of target user stories.

### User Story Dependency Graph

- **US1** -> enables **US3**
- **US2** -> enables **US4**
- **US1 + US2** -> enables **US5**
- **US3 + US4 + US5** -> enable **Polish** completion

### Within Each User Story

- Copy/configure files first.
- Identity/backend/logging updates second.
- Install/build/lint/terraform validation last.

---

## Parallel Execution Examples

### User Story 1 (US1)

- Run in parallel: T023, T024, T025, T026, T027, T028, T029
- Then run sequentially: T030 -> T031 -> T032 -> T033 -> T034 -> T035 -> T036 -> T037 -> T039 -> T040 -> T041 -> T042 -> T043

### User Story 2 (US2)

- Run in parallel: T044, T045, T046, T047, T048, T049, T050
- Then run sequentially: T051 -> T052 -> T053 -> T054 -> T055 -> T056 -> T057 -> T058 -> T059 -> T060 -> T061 -> T062 -> T063 -> T064

### User Story 5 (US5)

- Run in parallel: T066, T067, T069, T070, T071, T073, T074
- Then run sequentially: T065 -> T068 -> T072 -> T075 -> T076

### User Story 3 (US3)

- Run in parallel: T077, T078, T079, T080, T081
- Then run sequentially: T082 -> T083 -> T084 -> T085 -> T086 -> T087 -> T088

### User Story 4 (US4)

- Run in parallel: T089, T090, T091, T092, T093
- Then run sequentially: T094 -> T095 -> T096 -> T097 -> T098 -> T099 -> T100

---

## Implementation Strategy

### MVP First (P1 stories only)

1. Complete Phase 1 and Phase 2.
2. Complete US1 (Phase 3).
3. Complete US2 (Phase 4).
4. Complete US5 (Phase 5).
5. Validate P1 independent tests and deployability.

### Incremental Delivery

1. Ship P1 scaffold (`webapp-teams`, `webapp-spaces`, UI version sync).
2. Ship P2 teams health (`webapp-teams-help`).
3. Ship P2 spaces health (`webapp-spaces-help`).
4. Finish cross-cutting polish and performance/logging evidence.

### Format Validation

- All tasks use checkbox format with Task ID.
- `[US#]` labels appear only in user-story phases.
- `[P]` markers identify parallel-safe tasks.
- Every task includes at least one concrete file path.
