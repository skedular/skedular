# Tasks: Modularize Webapp Products

**Input**: Design documents from `specs/004-modularize-webapp-products/`
**Prerequisites**: plan.md ✅ spec.md ✅ research.md ✅ data-model.md ✅ quickstart.md ✅

**Tech stack**: TypeScript 6, React 19, Next.js 16, Relay, MUI v9, pnpm workspaces, Turborepo, Vitest
**Key constraint**: `@skedular/ui` must never import from `@skedular/shared`; `@skedular/shared` may import from `@skedular/ui`.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel with other [P]-marked tasks in the same phase
- **[Story]**: User story label — US1, US2, US3, US4

---

## Phase 1: Setup

**Purpose**: Scaffold the new `@skedular/shared` package so all subsequent extraction tasks have a destination.

- [x] T001 Scaffold `web/packages/shared/package.json` with name `@skedular/shared`, `"workspace:*"` dependencies on `@skedular/ui`, and `peerDependencies` for React 19, MUI v9, Next.js 16 — matching the pattern in `web/packages/ui/package.json`
- [x] T002 Create `web/packages/shared/tsconfig.json` extending the root web TypeScript config, matching the pattern in `web/packages/ui/`
- [x] T003 Create `web/packages/shared/src/index.ts` as an empty barrel export file to satisfy the package entry point
- [x] T004 Create `web/packages/shared/eslint.config.mjs` matching `web/packages/ui/eslint.config.mjs`

**Checkpoint**: `pnpm install` from `web/` resolves `@skedular/shared` as a workspace package

---

## Phase 2: Foundational

**Purpose**: Expand `@skedular/ui` with theme and typography directories. These are prerequisites because `@skedular/shared` providers depend on the theme, and webapp feature components depend on typography wrappers.

**⚠️ CRITICAL**: Phase 3–5 tasks depend on this phase completing first.

- [X] T005 Create `web/packages/ui/src/theme/` directory. Move `web/apps/webapp/src/libs/theme/theme-primitives.ts` into `web/packages/ui/src/theme/theme-primitives.ts` — update internal imports (none expected, file is self-contained)
- [X] T006 Move `web/apps/webapp/src/libs/theme/theme.ts` into `web/packages/ui/src/theme/theme.ts` — update import of `theme-primitives` from relative path to `./theme-primitives`
- [X] T007 Create `web/packages/ui/src/theme/index.ts` exporting all named exports from `theme-primitives.ts` and `theme.ts`
- [X] T008 Update `web/packages/ui/src/index.ts` to re-export everything from `./theme/index`
- [X] T009 [P] Update `web/apps/webapp/src/libs/theme/theme-primitives.ts` to re-export from `@skedular/ui` (keep file as thin re-export to avoid breaking internal `@/libs/theme` imports during transition; will be removed in US1)
- [X] T010 [P] Update `web/apps/webapp/src/libs/theme/theme.ts` to re-export from `@skedular/ui` (thin re-export bridge; will be removed in US1)
- [X] T011 [P] Add `"@skedular/shared": "workspace:*"` to `web/apps/webapp/package.json` dependencies
- [X] T012 Run `pnpm install` from `web/` and verify `@skedular/ui` TypeScript compilation succeeds: `pnpm turbo build --filter=@skedular/ui`

**Checkpoint**: `@skedular/ui` exports theme tokens and theme factory. `webapp` still builds via re-export bridges.

---

## Phase 3: User Story 1 — Establish Shared Module Boundaries (P1) 🎯 MVP

**Goal**: Extract all shared runtime modules from `webapp/src/libs/` into `@skedular/shared` so every product can import them from a single central package.

**Independent Test**: A developer can run `pnpm turbo build --filter=webapp` and all imports resolve from `@skedular/shared` for providers, hooks, and utilities — no `@/libs/providers/`, `@/libs/utils/`, `@/libs/mui/`, `@/libs/cookie-consent/`, or `@/libs/image-file-uploader/` imports remain in files that were extracted.

### Utilities — Canonical Example

- [X] T013 [US1] Create `web/packages/shared/src/utils/date-utils.ts` — move all date-related exports from `web/apps/webapp/src/libs/utils/index.ts` (dayjs setup, `convertCalendarDayToStartOfDay`, `localNow`, `now`, `isTodayDate`, `isTomorrowDate`, `isYesterdayDate`, `isInSameWeek`, `isInSameMonth`, and related helpers). Update internal imports to use `@skedular/ui` for theme-primitive colour constants where needed.
- [X] T014 [US1] Create `web/packages/shared/src/utils/name-utils.ts` — move `NameDetails` type and name-formatting helpers from `web/apps/webapp/src/libs/utils/index.ts`
- [X] T015 [US1] Create `web/packages/shared/src/utils/relay-utils.ts` — move `RelayErrorLike`, `ErrorWithGraphQlSource`, `secondaryColors`, and all Relay error-formatting helpers from `web/apps/webapp/src/libs/utils/index.ts`
- [X] T016 [US1] Create `web/packages/shared/src/utils/constants.ts` — move `isServer`, `keyboardSearchDebounceTimeout`, `keyboardTextFieldDebounceTimeout` from `web/apps/webapp/src/libs/utils/index.ts`
- [X] T017 [US1] Create `web/packages/shared/src/utils/index.ts` re-exporting all named exports from the four utils files above
- [X] T018 [US1] Delete `web/apps/webapp/src/libs/utils/index.ts` and update every `@/libs/utils` import in `web/apps/webapp/src/` to import from `@skedular/shared`

### Providers

- [X] T019 [US1] Move `web/apps/webapp/src/libs/providers/relay-provider.tsx` → `web/packages/shared/src/providers/relay-provider.tsx` — update internal imports
- [X] T020 [US1] Move `web/apps/webapp/src/libs/providers/theme-provider.tsx` → `web/packages/shared/src/providers/theme-provider.tsx` — update import of theme from `@skedular/ui`
- [X] T021 [US1] Move `web/apps/webapp/src/libs/providers/palette-mode-provider.tsx` → `web/packages/shared/src/providers/palette-mode-provider.tsx`
- [X] T022 [US1] Move `web/apps/webapp/src/libs/providers/date-picker-localization-provider.tsx` → `web/packages/shared/src/providers/date-picker-localization-provider.tsx`
- [X] T023 [US1] Move `web/apps/webapp/src/libs/providers/google-analytics-provider.tsx` → `web/packages/shared/src/providers/google-analytics-provider.tsx`
- [X] T024 [US1] Move `web/apps/webapp/src/libs/providers/logrocket-provider.tsx` → `web/packages/shared/src/providers/logrocket-provider.tsx`
- [X] T025 [US1] Move `web/apps/webapp/src/libs/providers/in-msteams-provider.tsx` → `web/packages/shared/src/providers/in-msteams-provider.tsx`
- [X] T026 [US1] Create `web/packages/shared/src/providers/index.ts` re-exporting all providers and their exported contexts

### Hooks

- [X] T027 [US1] Move `web/apps/webapp/src/libs/providers/known-params-hook.tsx` → `web/packages/shared/src/hooks/use-known-params.ts` — update internal imports
- [X] T029 [US1] Create `web/packages/shared/src/hooks/index.ts` re-exporting `use-known-params` — note: `integrated-platform-hook.tsx` (MS Teams platform-detection logic) is **deferred**; leave it in `webapp/src/libs/providers/` for now and do not include it in this extraction

### MUI Helpers

- [X] T030 [P] [US1] Move `web/apps/webapp/src/libs/mui/muix-license.tsx` → `web/packages/shared/src/mui/muix-license.tsx`
- [X] T031 [P] [US1] Move `web/apps/webapp/src/libs/mui/index.ts` exports (`defaultGridRowSelectionModelValue`) → `web/packages/shared/src/mui/index.ts`

### Cookie Consent & Image Upload

- [X] T032 [P] [US1] Move `web/apps/webapp/src/libs/cookie-consent/` (all three files) → `web/packages/shared/src/cookie-consent/` — update internal imports
- [X] T033 [P] [US1] Move `web/apps/webapp/src/libs/image-file-uploader/` (all three files) → `web/packages/shared/src/image-file-uploader/` — update internal imports

### Barrel and Webapp Wiring

- [X] T034 [US1] Update `web/packages/shared/src/index.ts` to re-export from `./providers`, `./hooks`, `./utils`, `./mui`, `./cookie-consent`, `./image-file-uploader`
- [X] T035 [US1] Delete `web/apps/webapp/src/libs/providers/` directory and update all `@/libs/providers/` imports in `webapp/src/` to use `@skedular/shared`
- [X] T036 [US1] Delete `web/apps/webapp/src/libs/mui/` directory and update all `@/libs/mui` imports in `webapp/src/` to use `@skedular/shared`
- [X] T037 [US1] Delete `web/apps/webapp/src/libs/cookie-consent/` directory and update all imports in `webapp/src/` to use `@skedular/shared`
- [X] T038 [US1] Delete `web/apps/webapp/src/libs/image-file-uploader/` directory and update all imports in `webapp/src/` to use `@skedular/shared`
- [X] T039 [US1] Remove the thin re-export bridge files from `web/apps/webapp/src/libs/theme/` (created in T009–T010) and update all remaining `@/libs/theme` imports in `webapp/src/` to use `@skedular/ui`
- [X] T040 [US1] Write unit tests for moved utilities in `web/packages/shared/src/utils/` — cover `date-utils.ts` (timezone helpers, comparison helpers), `name-utils.ts` (formatting), `relay-utils.ts` (error extraction). Use Vitest. Place tests at `web/packages/shared/src/utils/__tests__/`.
- [X] T041 [US1] Write unit tests for moved hooks in `web/packages/shared/src/hooks/` — cover `use-known-params` using React Testing Library + Vitest. Place tests at `web/packages/shared/src/hooks/__tests__/`.
- [X] T042 [US1] Verify `webapp` builds successfully: `pnpm turbo build --filter=webapp`
- [X] T043 [US1] Run unit tests for `webapp` and `@skedular/shared`: `pnpm turbo test --filter=webapp --filter=@skedular/shared` — fix any import path failures caused by the extraction

**Checkpoint**: All shared runtime modules live in `@skedular/shared`. Unit tests for moved modules pass. `webapp` imports exclusively from `@skedular/shared` for providers, hooks, utils, MUI helpers, cookie consent, and image upload. Build and tests pass.

---

## Phase 4: User Story 2 — Separate Product Ownership Clearly — DEFERRED

> **⏸ DEFERRED**: Individual product bootstrapping (`webapp-teams`, `webapp-spaces`) is out of scope for this feature. The shared module extraction in Phase 3 establishes the package boundary that future product builds will consume. When Teams or Spaces product implementation is prioritised, a dedicated feature should revisit this phase — including the `integrated-platform-hook.tsx` (MS Teams platform detection), per-product layout scaffolding, logging, and analytics wiring.
>
> The `in-msteams-provider.tsx` is extracted to `@skedular/shared` in Phase 3 (T025) as a provider component. Its usage and the hook that detects the Teams vs browser context are deferred.

_No tasks in this phase for the current feature._

---

## Phase 5: User Story 3 — Consolidate Shared Design Patterns (P2)

**Goal**: Move all typography wrappers and remaining generic layout helpers from `webapp/src/components/commons/` into `@skedular/ui` so all three products can use them without copying.

**Independent Test**: A developer can add a typography wrapper to `webapp` using `import { BodyIconTypography } from '@skedular/ui'` and confirm the component renders correctly; all existing `webapp` feature components continue to render correctly after the import path change.

### Typography Wrappers → `@skedular/ui`

- [X] T052 [US3] Create `web/packages/ui/src/typography/` directory. Move all `*-typography.tsx` files and `icon-typography.tsx` from `web/apps/webapp/src/components/commons/` → `web/packages/ui/src/typography/` — update each file to import `Typography` from `@mui/material/Typography` directly (these are the low-level primitive implementations where direct MUI import is allowed)
- [X] T053 [US3] Create `web/packages/ui/src/typography/index.ts` re-exporting all typography wrapper components
- [X] T054 [US3] Update `web/packages/ui/src/index.ts` to re-export from `./typography/index`

### Generic Commons → `@skedular/ui`

- [X] T055 [P] [US3] Move generic layout helpers from `web/apps/webapp/src/components/commons/` that have no product state or business logic into `web/packages/ui/src/commons/`: `appbar-with-stack-column.tsx`, `collection-toolbar.tsx`, `color-picker.tsx`, `credit-card.tsx`, `default-dialog-title.tsx`, `form-field-label.tsx`, `form-stack-column-base.tsx`, `form-stack-column.tsx`, `grid-container.tsx`, `helper-text.tsx`, `push-to-right.tsx`, `two-buttons-dialog-actions.tsx` — note: `stack-column.tsx` and `stack-row.tsx` are already in `web/packages/ui/src/` and require no move
- [X] T056 [P] [US3] Create `web/packages/ui/src/commons/index.ts` re-exporting all moved commons components
- [X] T057 [US3] Update `web/packages/ui/src/index.ts` to re-export from `./commons/index`

### webapp Import Updates

- [X] T058 [US3] Delete `web/apps/webapp/src/components/commons/` directory and update all `@/components/commons` imports in `webapp/src/` to use `@skedular/ui` — confirm no `@mui/material/Typography` direct imports have been introduced in feature components
- [X] T059 [US3] Write unit tests for typography wrappers in `web/packages/ui/src/typography/__tests__/` — verify each wrapper renders with the correct MUI variant prop using Vitest + React Testing Library; at minimum cover `BodyIconTypography`, `SmallIconTypography`, `CaptionIconTypography`, `ErrorTypography`
- [X] T060 [US3] Verify `@skedular/ui` TypeScript compilation: `pnpm turbo build --filter=@skedular/ui`
- [X] T061 [US3] Verify `webapp` builds: `pnpm turbo build --filter=webapp`
- [X] T062 [US3] Run webapp and `@skedular/ui` unit tests: `pnpm turbo test --filter=webapp --filter=@skedular/ui` — fix any test failures from commons path changes

**Checkpoint**: All typography wrappers and generic layout helpers live in `@skedular/ui`. Unit tests for moved typography wrappers pass. `webapp` imports from `@skedular/ui`. No `@/components/commons` paths remain.

---

## Phase 6: User Story 4 — Preserve Delivery Safety During Refactor (P2)

**Goal**: Verify the full build pipeline passes for all products and packages, fix any remaining broken test imports, and ensure AGENTS.md documentation reflects the new import conventions.

**Independent Test**: After modularising, a maintainer can run `pnpm turbo build` and `pnpm turbo test` from `web/` and all products and packages pass with zero errors.

### Full Build & Test Verification

- [X] T063 [US4] Run full monorepo build: `pnpm turbo build` from `web/` — identify and fix any remaining type errors or missing re-exports
- [X] T064 [US4] Run full test suite: `pnpm turbo test` from `web/` — fix any test failures caused by changed import paths (do not change test logic, only update import paths)
- [X] T065 [P] [US4] Run `pnpm turbo lint` from `web/` — fix any lint errors introduced by the refactor (unused imports, missing re-exports)

### Documentation Updates

- [X] T066 [P] [US4] Update `web/apps/webapp/AGENTS.md` — revise the Typography Rule to show `import { BodyIconTypography } from '@skedular/ui'` as the correct path; remove reference to `@/components/commons` as the source; add note that providers/utils/hooks import from `@skedular/shared`; document FR-013 auth ownership (sign-in, callback, account settings, notifications remain in `webapp`); note that Teams/Spaces product bootstrapping is deferred
- [X] T067 [P] [US4] Update `web/AGENTS.md` — add section describing the two workspace packages (`@skedular/ui` and `@skedular/shared`), their boundaries, and the decision tree from `quickstart.md`
- [X] T068 [P] [US4] Verify LOG-001/LOG-003/LOG-004 compliance: (1) confirm `GoogleAnalyticsProvider` and `LogrocketProvider` in `@skedular/shared` accept product-specific tag IDs as props rather than importing them internally; (2) confirm `@skedular/shared/src/index.ts` does not re-export any env var values or sensitive runtime config; (3) grep `web/packages/shared/src/` for any `process.env` reads — all product-specific env binding must remain in the product app

**Checkpoint**: Full pipeline (`pnpm turbo build && pnpm turbo test && pnpm turbo lint`) passes for all products and packages. AGENTS.md files accurately describe the modularized import conventions. LOG compliance verified.

---

## Phase 7: Polish & Cross-Cutting Concerns

- [X] T069 [P] Verify no direct `@mui/material/Typography` imports exist outside `web/packages/ui/src/typography/` using grep: `grep -r "from '@mui/material/Typography'" web/apps`
- [X] T070 [P] Verify no `@skedular/ui` imports exist inside `web/packages/shared/src/` that would create a circular dependency inversion (`@skedular/ui` importing back from `@skedular/shared`)
- [X] T071 Run `quickstart.md` slice checklist against the completed codebase to confirm all ownership map entries in `data-model.md` match the final file locations
- [X] T072 [P] Update `.github/copilot-instructions.md` — revise the Web Typography Rule to reference `@skedular/ui` as the import source (replacing `@/components/commons`); add a note that shared providers, hooks, and utilities import from `@skedular/shared`; this aligns the repo-level agent instructions with the updated constitution §IV

---

## Dependencies & Execution Order

### Phase Dependencies

- **Phase 1 (Setup)**: No dependencies — start immediately
- **Phase 2 (Foundational)**: Depends on Phase 1
- **Phase 3 (US1)**: Depends on Phase 2
- **Phase 4 (US2)**: DEFERRED — no tasks in this feature
- **Phase 5 (US3)**: Depends on Phase 2 — can run in parallel with Phase 3 (only depends on `@skedular/ui` theme being ready)
- **Phase 6 (US4)**: Depends on Phases 3 and 5 both being complete
- **Phase 7 (Polish)**: Depends on Phase 6

### User Story Dependencies

- **US1 (P1)**: Depends on Foundational (Phase 2) only
- **US2 (P1)**: DEFERRED — product bootstrapping for Teams/Spaces is out of scope for this feature
- **US3 (P2)**: Depends on Foundational (Phase 2) — can proceed in parallel with US1
- **US4 (P2)**: Depends on US1 + US3 both complete

### Within Each Phase

- Tasks marked `[P]` within a phase can run in parallel
- Sequential tasks within a phase must be completed in listed order
- Each phase ends with a build verification — do not proceed to the next phase until the checkpoint passes

### Parallel Opportunities

**Phase 2**: T005–T008 (theme to @skedular/ui) must sequence; T009–T011 can run in parallel after T008  
**Phase 3**: T013–T017 (utils) → T018 (delete + update imports); T019–T026 (providers) can run in parallel with T027/T029 (hook) and T030–T033 (MUI/cookie/image) once utils are extracted  
**Phase 4**: DEFERRED  
**Phase 5**: T052–T054 sequences; T055–T057 can run in parallel with T052–T054

---

## Parallel Example: User Story 1 (US1)

Given two engineers available after Phase 2 completes:

```
Engineer A                              Engineer B
─────────────────────────────────       ─────────────────────────────────
T013 date-utils extraction              T030 MUI helpers extraction
T014 name-utils extraction              T031 MUI index
T015 relay-utils extraction             T032 cookie-consent extraction
T016 constants extraction               T033 image-file-uploader extraction
T017 utils/index.ts barrel
T018 delete webapp/libs/utils + update imports
T019–T026 providers (sequential)        T027/T029 hook (after T025)
T034 shared/src/index.ts barrel
T035–T039 webapp cleanup (sequential)
T040 unit tests — utils                 T041 unit tests — hook
T042 build verification
T043 build + test verification (all)
```

---

## Implementation Strategy

**MVP Scope**: Phase 1 + Phase 2 + Phase 3 (US1) — once complete, `webapp` works correctly with `@skedular/shared` as the source of truth for all shared runtime modules. Teams and Spaces can immediately declare the dependency and begin real product work.

**Incremental Delivery**: Each phase leaves all affected products buildable. Never delete source files before the destination package is confirmed to compile and the consuming app's imports are updated.

**Slice Rule** (from `quickstart.md`): A slice is not complete until the source location is deleted, all consumers updated, and the build + test suite passes.
