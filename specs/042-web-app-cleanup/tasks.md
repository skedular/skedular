# Tasks: Web App Component Cleanup

**Input**: Design documents from `/specs/042-web-app-cleanup/`
**Prerequisites**: [plan.md](plan.md), [spec.md](spec.md), [research.md](research.md), [data-model.md](data-model.md), [contracts/reachability-inventory.md](contracts/reachability-inventory.md), [quickstart.md](quickstart.md)

**Tests**: Existing per-app lint, unit/component tests, builds, relevant Playwright suites, and route inventory comparisons are required by the feature specification. No new product test framework is needed.

## Phase 1: Setup

**Purpose**: Establish the cleanup audit workspace and confirm the actual application boundaries.

- [X] T001 Confirm the four in-scope application roots and excluded paths in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T002 [P] Record each app’s existing lint, test, build, Playwright, and Relay commands in `specs/042-web-app-cleanup/quickstart.md`
- [X] T003 [P] Create the initial audit record structure for the four applications in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`

## Phase 2: Foundational

**Purpose**: Capture route roots and dependency-analysis rules before any deletion. This phase blocks cleanup work.

- [X] T004 Enumerate `src/web/apps/webapp/src/app`, `src/web/apps/webapp/src/rootPages`, middleware/proxy, API routes, and custom-domain entry points in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T005 [P] Enumerate `src/web/apps/webapp-spaces/src/app`, `src/web/apps/webapp-spaces/src/rootPages`, middleware/proxy, API routes, and custom-domain entry points in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T006 [P] Enumerate `src/web/apps/webapp-teams/src/app`, `src/web/apps/webapp-teams/src/rootPages`, middleware/proxy, API routes, and custom-domain entry points in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T007 [P] Enumerate `src/web/apps/webapp-host/src/app`, `src/web/apps/webapp-host/src/rootPages`, middleware/proxy, API routes, and custom-domain entry points in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T008 Document route-file and route-level-test protection, shared/UI exclusions, generated-file exclusions, and unresolved-candidate retention in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T009 [P] Capture the pre-cleanup route inventory for `src/web/apps/webapp` in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T010 [P] Capture the pre-cleanup route inventory for `src/web/apps/webapp-spaces` in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T011 [P] Capture the pre-cleanup route inventory for `src/web/apps/webapp-teams` in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T012 [P] Capture the pre-cleanup route inventory for `src/web/apps/webapp-host` in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`

**Checkpoint**: All four route surfaces and deletion rules are recorded before candidate cleanup begins.

## Phase 3: User Story 1 - Establish the Four-App Inventory (Priority: P1) 🎯 MVP

**Goal**: Produce a complete, evidence-backed inventory of application-owned components, support files, and tests.

**Independent Test**: Review `specs/042-web-app-cleanup/contracts/reachability-inventory.md` and verify that every in-scope candidate has a classification, evidence, consumers, and deletion eligibility decision.

- [X] T013 [P] [US1] Inventory application-owned components and exports under `src/web/apps/webapp/src/components` and related source directories in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T014 [P] [US1] Inventory application-owned components and exports under `src/web/apps/webapp-spaces/src/components` and related source directories in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T015 [P] [US1] Inventory application-owned components and exports under `src/web/apps/webapp-teams/src/components` and related source directories in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T016 [P] [US1] Inventory application-owned components and exports under `src/web/apps/webapp-host/src/components` and related source directories in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T017 [US1] Trace static imports, aliases, barrels, dynamic imports, test setup, and workspace references for all four applications and record evidence in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T018 [US1] Classify every candidate as used, conditionally-used, unused, or unresolved and record retained consumers in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T019 [US1] Review the inventory for missing route roots, protected routes/tests, shared/UI files, generated artifacts, and speculative deletion decisions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`

**Checkpoint**: User Story 1 is complete when all four application inventories are reviewable and no candidate lacks evidence.

## Phase 4: User Story 2 - Remove Unreachable Application Code (Priority: P1)

**Goal**: Remove confirmed dead components, component-only tests, and orphaned app-owned dependency chains without touching protected routes.

**Independent Test**: Apply cleanup to one application from the approved inventory, then confirm no retained file imports removed code and the application’s existing checks pass.

- [X] T020 [US2] Approve the confirmed-unused candidate set and protected/ambiguous exclusions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T021 [P] [US2] Remove confirmed-unused application-owned components and component-only tests under `src/web/apps/webapp`, updating only app-owned imports/exports and recording deletions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T022 [P] [US2] Remove confirmed-unused application-owned components and component-only tests under `src/web/apps/webapp-spaces`, updating only app-owned imports/exports and recording deletions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T023 [P] [US2] Remove confirmed-unused application-owned components and component-only tests under `src/web/apps/webapp-teams`, updating only app-owned imports/exports and recording deletions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T024 [P] [US2] Remove confirmed-unused application-owned components and component-only tests under `src/web/apps/webapp-host`, updating only app-owned imports/exports and recording deletions in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T025 [US2] Follow deleted candidates’ dependency chains and remove orphaned app-owned helpers, hooks, styles, fixtures, configuration, and tests only when no retained consumer remains under `src/web/apps`
- [X] T026 [US2] Re-scan `src/web/apps/webapp`, `src/web/apps/webapp-spaces`, `src/web/apps/webapp-teams`, and `src/web/apps/webapp-host` for stale imports, exports, test references, and protected-file changes

**Checkpoint**: User Story 2 is complete when confirmed dead code is removed, ambiguous/shared code remains, and no unresolved application-owned reference points to a deleted file.

## Phase 5: User Story 3 - Prove the Cleanup Is Behavior-Preserving (Priority: P2)

**Goal**: Demonstrate that every retained route remains available and each application passes its existing quality checks.

**Independent Test**: Compare the protected route inventory before and after cleanup and run the validation commands for one application without cleanup-introduced failures.

- [X] T027 [P] [US3] Run lint, unit/component tests, and build for `src/web/apps/webapp` using its existing package scripts and record results in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T028 [P] [US3] Run lint, unit/component tests, and build for `src/web/apps/webapp-spaces` using its existing package scripts and record results in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T029 [P] [US3] Run lint, unit/component tests, and build for `src/web/apps/webapp-teams` using its existing package scripts and record results in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T030 [P] [US3] Run lint, unit/component tests, and build for `src/web/apps/webapp-host` using its existing package scripts and record results in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`
- [X] T031 [P] [US3] Run relevant Playwright smoke suites for `src/web/apps/webapp`, `src/web/apps/webapp-spaces`, `src/web/apps/webapp-teams`, and `src/web/apps/webapp-host` and record supported/unsupported results
- [X] T032 [US3] Compare the post-cleanup route inventory with the protected pre-cleanup inventory and document any discrepancy in `specs/042-web-app-cleanup/contracts/reachability-inventory.md`

**Checkpoint**: User Story 3 is complete when all four apps pass applicable validation, protected routes remain, and cleanup-introduced failures are resolved.

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Finalize evidence, documentation, and repository hygiene.

- [X] T033 [P] Update `specs/042-web-app-cleanup/quickstart.md` with the actual commands and environment notes discovered during validation
- [X] T034 [P] Update `specs/042-web-app-cleanup/contracts/reachability-inventory.md` with final deleted, retained-ambiguous, protected-route, and validation summaries
- [X] T035 Run `git diff --check` and verify no changes exist under shared/UI packages, generated artifacts, dependencies, `.next`, or unrelated applications
- [X] T036 Run the final quickstart validation from `specs/042-web-app-cleanup/quickstart.md` and confirm zero confirmed-unused candidates remain in the audited application scope

## Dependencies & Execution Order

### Phase Dependencies

- Setup (Phase 1) has no dependencies.
- Foundational (Phase 2) depends on Setup and blocks all user stories.
- User Story 1 depends on the completed route-root inventory in Phase 2.
- User Story 2 depends on the approved inventory from User Story 1.
- User Story 3 depends on the cleanup from User Story 2.
- Polish depends on the completed validation in User Story 3.

### User Story Dependencies

- **US1 (P1)**: Starts after Phase 2; MVP and prerequisite for deletion.
- **US2 (P1)**: Depends on US1’s approved classifications; can proceed app-by-app in parallel after approval.
- **US3 (P2)**: Depends on US2; per-app validation tasks can run in parallel.

## Parallel Opportunities

- T005–T007 and T009–T012 can run in parallel across applications.
- T013–T016 can run in parallel across applications.
- T021–T024 can run in parallel after T020, provided each task stays within its application directory.
- T027–T031 can run in parallel after cleanup and stale-reference scanning.
- T033–T035 can run in parallel after validation results are available.

## Implementation Strategy

### MVP First

1. Complete Setup and Foundational phases.
2. Complete User Story 1 and review the four-app inventory.
3. Stop for approval at the US1 checkpoint; this inventory is the MVP deliverable and supports safe incremental cleanup.

### Incremental Delivery

1. Clean and validate one application at a time under US2 and US3.
2. Keep route files, route-level tests, shared/UI files, and ambiguous candidates protected throughout.
3. Finish with the cross-app evidence and quickstart checks.

## Notes

- Every task follows the required checklist format and includes a concrete repository or artifact path.
- No new backend, API, persistence, or generated-contract tasks are required.
