# Tasks: App Switcher

**Input**: Design documents from `/specs/019-app-switcher/`
**Prerequisites**: plan.md, spec.md, research.md, data-model.md, contracts/app-switcher-contract.md, quickstart.md

**Tests**: Included because the project constitution requires Vitest and React Testing Library for web UI changes, and logging behavior must be verified.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- Web workspace: `src/web/`
- Product apps: `src/web/apps/webapp`, `src/web/apps/webapp-teams`, `src/web/apps/webapp-spaces`
- Shared runtime package: `src/web/packages/shared`
- Shared UI package: `src/web/packages/ui`

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the existing shared-package and product-app seams before implementation begins.

- [x] T001 Inspect current app shell exports and tests in `src/web/packages/shared/src/app-shell/index.ts`, `src/web/packages/ui/src/app-shell/index.ts`, and `src/web/packages/ui/src/app-shell/__tests__/app-shell-layout.test.tsx`
- [x] T002 [P] Inspect existing product app environment typings in `src/web/apps/webapp/src/types/environment.d.ts`, `src/web/apps/webapp-teams/src/types/environment.d.ts`, and `src/web/apps/webapp-spaces/src/types/environment.d.ts`
- [x] T003 [P] Inspect current root page and shell wiring in `src/web/apps/webapp/src/app/page.tsx`, `src/web/apps/webapp-teams/src/app/page.tsx`, and `src/web/apps/webapp-spaces/src/app/page.tsx`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core app identity, configuration, validation, and logging primitives that all user stories depend on.

**Critical**: No user story work can begin until this phase is complete.

- [x] T004 Update canonical product app display names to Skedular, Skedular Teams, and Skedular Spaces in `src/web/packages/shared/src/app-products/app-products.ts`
- [x] T005 [P] Add tests for canonical product app display names in `src/web/packages/shared/src/app-products/__tests__/app-products.test.ts`
- [x] T006 Define app switcher configuration, destination availability, destination, and model types in `src/web/packages/shared/src/app-shell/app-switcher-model.ts`
- [x] T007 Implement app switcher URL normalization and destination availability derivation in `src/web/packages/shared/src/app-shell/app-switcher-model.ts`
- [x] T008 [P] Export app switcher model APIs from `src/web/packages/shared/src/app-shell/index.ts`
- [x] T009 [P] Add app switcher configuration and selection log event helpers in `src/web/packages/shared/src/app-shell/app-switcher-logger.ts`
- [x] T010 [P] Export app switcher logging APIs from `src/web/packages/shared/src/app-shell/index.ts`
- [x] T011 [P] Add model tests for valid URLs, missing URLs, invalid URLs, current app handling, and destination ordering in `src/web/packages/shared/src/app-shell/__tests__/app-switcher-model.test.ts`
- [x] T012 [P] Add logging tests for configuration and selection events without sensitive URL payloads in `src/web/packages/shared/src/app-shell/__tests__/app-switcher-logger.test.ts`

**Checkpoint**: Shared app switcher model and logging contracts are ready for UI and product-app wiring.

---

## Phase 3: User Story 1 - Switch Between Skedular Apps (Priority: P1) MVP

**Goal**: Users can open a switcher in each Skedular app and navigate to another configured app's base URL.

**Independent Test**: Open each product app with all destination URLs configured, choose another app from the switcher, and confirm navigation uses the configured base URL without preserving current page, organization, tenant, or workflow context.

### Tests for User Story 1

- [x] T013 [P] [US1] Add UI tests for rendering active app destination links and invoking destination selection in `src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx`
- [x] T014 [P] [US1] Add AppShellLayout tests proving the shared shell header does not reserve product-switching UI in `src/web/packages/ui/src/app-shell/__tests__/app-shell-layout.test.tsx`
- [x] T015 [P] [US1] Add product wiring tests for Skedular destination configuration in `src/web/apps/webapp/src/app/app-switcher-config.test.ts`
- [x] T016 [P] [US1] Add product wiring tests for Skedular Teams destination configuration in `src/web/apps/webapp-teams/src/app/app-switcher-config.test.ts`
- [x] T017 [P] [US1] Add product wiring tests for Skedular Spaces destination configuration in `src/web/apps/webapp-spaces/src/app/app-switcher-config.test.ts`

### Implementation for User Story 1

- [x] T018 [US1] Implement reusable AppSwitcher component with accessible links and selection callback in `src/web/packages/ui/src/app-shell/app-switcher.tsx`
- [x] T019 [US1] Export AppSwitcher and its public props from `src/web/packages/ui/src/app-shell/index.ts`
- [x] T020 [US1] Keep AppShellLayout free of app switcher wiring so switching remains a secondary navigation/menu shortcut in `src/web/packages/ui/src/app-shell/app-shell-layout.tsx`
- [x] T021 [US1] Add configured app URL environment typings for Skedular in `src/web/apps/webapp/src/types/environment.d.ts`
- [x] T022 [US1] Add configured app URL environment typings for Skedular Teams in `src/web/apps/webapp-teams/src/types/environment.d.ts`
- [x] T023 [US1] Add configured app URL environment typings for Skedular Spaces in `src/web/apps/webapp-spaces/src/types/environment.d.ts`
- [x] T024 [US1] Implement Skedular app switcher configuration builder in `src/web/apps/webapp/src/app/app-switcher-config.ts`
- [x] T025 [US1] Implement Skedular Teams app switcher configuration builder in `src/web/apps/webapp-teams/src/app/app-switcher-config.ts`
- [x] T026 [US1] Implement Skedular Spaces app switcher configuration builder in `src/web/apps/webapp-spaces/src/app/app-switcher-config.ts`
- [x] T027 [US1] Wire app switcher configuration into authenticated Skedular left navigation in `src/web/apps/webapp/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [x] T028 [US1] Wire app switcher configuration into authenticated Skedular Teams left navigation in `src/web/apps/webapp-teams/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [x] T029 [US1] Wire app switcher configuration into authenticated Skedular Spaces left navigation in `src/web/apps/webapp-spaces/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [x] T030 [US1] Emit structured app switcher selection logs before navigation in `src/web/packages/ui/src/app-shell/app-switcher.tsx`

**Checkpoint**: User Story 1 is functional and testable independently in all three apps.

---

## Phase 4: User Story 2 - See Current App Context (Priority: P2)

**Goal**: Users can clearly identify the current Skedular app and distinguish it from available destination apps.

**Independent Test**: Open the switcher in each app and confirm the current app is identified while the other configured apps remain switch targets with canonical names.

### Tests for User Story 2

- [x] T031 [P] [US2] Add model tests for current app destination state in `src/web/packages/shared/src/app-shell/__tests__/app-switcher-model.test.ts`
- [x] T032 [P] [US2] Add UI tests for current app labeling and non-link current item behavior in `src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx`

### Implementation for User Story 2

- [x] T033 [US2] Update AppSwitcher to visually identify the current app without rendering it as an active switch link in `src/web/packages/ui/src/app-shell/app-switcher.tsx`
- [x] T034 [US2] Ensure authenticated app navigation passes current app identity to AppSwitcher from the shared model in each product navigation menu
- [x] T035 [US2] Update product app root and customer-facing tests to confirm the switcher is not promoted into root pages or customer-facing subdomains in `src/web/apps/webapp/src/app/page.test.tsx`, `src/web/apps/webapp-teams/src/app/page.test.tsx`, `src/web/apps/webapp-spaces/src/app/page.test.tsx`, and `src/web/apps/webapp/src/app/customer-facing-subdomain/private-organization-subdomain.test.tsx`

**Checkpoint**: User Stories 1 and 2 both work independently.

---

## Phase 5: User Story 3 - Handle Unavailable App Destinations (Priority: P3)

**Goal**: Missing or invalid destination URLs do not produce broken switch targets, while valid configured destinations still render even when destination access is unknown.

**Independent Test**: Run an app with one missing URL, one invalid URL, and one valid URL; confirm only the valid destination is active and malformed configuration is logged without blocking the shell.

### Tests for User Story 3

- [x] T036 [P] [US3] Add model tests for missing, malformed, unsupported, and partially configured destination URLs in `src/web/packages/shared/src/app-shell/__tests__/app-switcher-model.test.ts`
- [x] T037 [P] [US3] Add UI tests for hiding inactive destinations and omitting the switcher when no active targets exist in `src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx`
- [x] T038 [P] [US3] Add logging tests for malformed destination warning events in `src/web/packages/shared/src/app-shell/__tests__/app-switcher-logger.test.ts`

### Implementation for User Story 3

- [x] T039 [US3] Ensure app switcher model excludes missing-url and invalid-url destinations from active switch targets in `src/web/packages/shared/src/app-shell/app-switcher-model.ts`
- [x] T040 [US3] Ensure AppSwitcher hides inactive destinations and returns no unusable controls when no switch targets exist in `src/web/packages/ui/src/app-shell/app-switcher.tsx`
- [x] T041 [US3] Emit structured configuration logs for missing and invalid destination decisions in `src/web/packages/shared/src/app-shell/app-switcher-logger.ts`
- [x] T042 [US3] Wire configuration logging from product app configuration builders in `src/web/apps/webapp/src/app/app-switcher-config.ts`, `src/web/apps/webapp-teams/src/app/app-switcher-config.ts`, and `src/web/apps/webapp-spaces/src/app/app-switcher-config.ts`

**Checkpoint**: All user stories are independently functional.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Verification, accessibility, responsive behavior, and documentation cleanup across all stories.

- [x] T043 [P] Add responsive and keyboard accessibility assertions for the switcher in `src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx`
- [x] T044 [P] Add quickstart configuration examples for local app URLs in `specs/019-app-switcher/quickstart.md`
- [x] T045 Run shared package tests with `pnpm --dir src/web --filter @skedular/shared test`
- [x] T046 Run shared UI tests with `pnpm --dir src/web --filter @skedular/ui test`
- [x] T047 Run product app tests with `pnpm --dir src/web --filter webapp test`, `pnpm --dir src/web --filter webapp-teams test`, and `pnpm --dir src/web --filter webapp-spaces test`
- [x] T048 Run product app lint checks with `pnpm --dir src/web --filter webapp lint`, `pnpm --dir src/web --filter webapp-teams lint`, and `pnpm --dir src/web --filter webapp-spaces lint`
- [x] T049 Verify no generated Relay/OpenAPI artifacts were hand-edited by checking `src/web/apps/webapp/src/queries/__generated__`, `src/web/apps/webapp-teams/src/queries/__generated__`, `src/web/apps/webapp-spaces/src/queries/__generated__`, `src/web/apps/webapp/src/clients/openapi`, `src/web/apps/webapp-teams/src/clients/openapi`, and `src/web/apps/webapp-spaces/src/clients/openapi`
- [x] T050 [US1] Fix app switcher discoverability by rendering it near the top of authenticated product navigation in expanded and collapsed states in `src/web/packages/ui/src/app-shell/app-switcher.tsx`, `src/web/apps/webapp/src/components/navigationMenu/left-side-navigation-menu-content.tsx`, `src/web/apps/webapp-teams/src/components/navigationMenu/left-side-navigation-menu-content.tsx`, and `src/web/apps/webapp-spaces/src/components/navigationMenu/left-side-navigation-menu-content.tsx`
- [x] T051 [US1] Fix client-side app switcher configuration defaults so Next.js inlines public destination URL environment values in `src/web/apps/webapp/src/app/app-switcher-config.ts`, `src/web/apps/webapp-teams/src/app/app-switcher-config.ts`, and `src/web/apps/webapp-spaces/src/app/app-switcher-config.ts`
- [x] T052 [US1] Move app switcher access from left navigation into authenticated profile menus in `src/web/packages/ui/src/app-shell/app-switcher.tsx`, `src/web/apps/webapp/src/components/appBar/app-bar.tsx`, `src/web/apps/webapp-teams/src/components/appBar/app-bar.tsx`, `src/web/apps/webapp-spaces/src/components/appBar/app-bar.tsx`, and matching no-organization app bars
- [x] T053 [US1] Configure deployed Vercel app switcher URL environment variables for Skedular, Skedular Teams, and Skedular Spaces in each product app Terraform workspace

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup completion and blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational and is the MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational; can be implemented after or alongside US1 once shared model/UI seams exist.
- **User Story 3 (Phase 5)**: Depends on Foundational; can be implemented after or alongside US1 once shared model/UI seams exist.
- **Polish (Phase 6)**: Depends on selected user stories being complete.

### User Story Dependencies

- **User Story 1 (P1)**: No dependency on US2 or US3 after Foundational.
- **User Story 2 (P2)**: No dependency on US3; uses the same shared model created for US1.
- **User Story 3 (P3)**: No dependency on US2; uses the same shared model created for US1.

### Within Each User Story

- Tests should be written first and fail before implementation.
- Shared model behavior should precede shared UI behavior.
- Shared UI should precede product-app wiring.
- Logging assertions should be added with the behavior they verify.

### Parallel Opportunities

- T002 and T003 can run in parallel after T001 starts.
- T005, T008, T009, T010, T011, and T012 can run in parallel where file ownership does not overlap.
- T013 through T017 can run in parallel after Foundational completion.
- T021 through T026 can run in parallel by product app.
- T031 and T032 can run in parallel for US2.
- T036 through T038 can run in parallel for US3.
- T043 and T044 can run in parallel during Polish.

---

## Parallel Example: User Story 1

```bash
Task: "Add UI tests for rendering active app destination links and invoking destination selection in src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx"
Task: "Add product wiring tests for Skedular destination configuration in src/web/apps/webapp/src/app/app-switcher-config.test.ts"
Task: "Add product wiring tests for Skedular Teams destination configuration in src/web/apps/webapp-teams/src/app/app-switcher-config.test.ts"
Task: "Add product wiring tests for Skedular Spaces destination configuration in src/web/apps/webapp-spaces/src/app/app-switcher-config.test.ts"
```

## Parallel Example: User Story 2

```bash
Task: "Add model tests for current app destination state in src/web/packages/shared/src/app-shell/__tests__/app-switcher-model.test.ts"
Task: "Add UI tests for current app labeling and non-link current item behavior in src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx"
```

## Parallel Example: User Story 3

```bash
Task: "Add model tests for missing, malformed, unsupported, and partially configured destination URLs in src/web/packages/shared/src/app-shell/__tests__/app-switcher-model.test.ts"
Task: "Add UI tests for hiding inactive destinations and omitting the switcher when no active targets exist in src/web/packages/ui/src/app-shell/__tests__/app-switcher.test.tsx"
Task: "Add logging tests for malformed destination warning events in src/web/packages/shared/src/app-shell/__tests__/app-switcher-logger.test.ts"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational shared model and logging primitives.
3. Complete Phase 3: User Story 1.
4. Stop and validate app switching in all three apps.

### Incremental Delivery

1. Add User Story 1 for base cross-app switching.
2. Add User Story 2 for clearer current-app context.
3. Add User Story 3 for missing and invalid configuration resilience.
4. Run Phase 6 verification before review.

### Parallel Team Strategy

1. One developer owns `@skedular/shared` model/logging work.
2. One developer owns `@skedular/ui` switcher and authenticated navigation integration.
3. One developer wires and tests the three product apps after the shared contracts settle.

## Notes

- Keep user-facing copy in American English.
- Do not hand-edit generated Relay or OpenAPI artifacts.
- Do not add backend contracts or generated code unless a later implementation discovery changes scope and the plan is updated first.
- Keep app-switcher URLs as configured base URLs; do not append current page, organization, tenant, or workflow context.
