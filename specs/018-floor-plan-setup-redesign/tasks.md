# Tasks: Floor Plan Setup Page Redesign

**Input**: Design documents from `specs/018-floor-plan-setup-redesign/`
**Implementation Direction**: Keep `AddFloorPlan` and `EditFloorPlan` app-local in webapp,
webapp-teams, and webapp-spaces. Do not move the full components or Relay fragments into
`@skedular/shared`.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel when files do not overlap
- **[Story]**: Which user story this task belongs to (US1-US4)

---

## Phase 1: Design Alignment

**Purpose**: Replace the old secondary dark app-bar layout with the standard centred settings
page layout in each app-local implementation.

- [x] T001 [US1] Update `src/web/apps/webapp/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx` to use the modern centred page layout with `PageHeaderPanel`, `SettingsSectionCard`, and `EditorActionBar`
- [x] T002 [US2] Update `src/web/apps/webapp/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx` to use the same modern centred page layout and preserve edit auto-save behaviour
- [x] T003 [P] [US3] Apply the same Add Floor Plan layout update in `src/web/apps/webapp-teams/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`
- [x] T004 [P] [US3] Apply the same Edit Floor Plan layout update in `src/web/apps/webapp-teams/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`
- [x] T005 [P] [US4] Apply the same Add Floor Plan layout update in `src/web/apps/webapp-spaces/src/components/floorPlan/addFloorPlan/add-floor-plan.tsx`
- [x] T006 [P] [US4] Apply the same Edit Floor Plan layout update in `src/web/apps/webapp-spaces/src/components/floorPlan/editFloorPlan/edit-floor-plan.tsx`

---

## Phase 2: Behaviour Preservation

**Purpose**: Ensure redesign changes do not alter floor-plan create/update semantics.

- [x] T007 [US1] Ensure Add Floor Plan only calls `onAdded` and `onReloadRequired` after a successful GraphQL mutation completion in webapp
- [x] T008 [US3] Ensure Add Floor Plan only calls `onAdded` and `onReloadRequired` after a successful GraphQL mutation completion in webapp-teams
- [x] T009 [US4] Ensure Add Floor Plan only calls `onAdded` and `onReloadRequired` after a successful GraphQL mutation completion in webapp-spaces
- [x] T010 [US2] Preserve Edit Floor Plan debounce/auto-save, image upload, and resource-position mutation semantics in webapp
- [x] T011 [US3] Preserve Edit Floor Plan debounce/auto-save, image upload, and resource-position mutation semantics in webapp-teams
- [x] T012 [US4] Preserve Edit Floor Plan debounce/auto-save, image upload, and resource-position mutation semantics in webapp-spaces

---

## Phase 3: Shared Utilities Only

**Purpose**: Keep the full floor-plan setup components app-local while allowing small already
cross-app utilities to remain shared.

- [x] T013 [P] Keep `NewFloorplanButton` as a small shared control in `src/web/packages/shared/src/floor-plan/add-floor-plan/new-floorplan-button.tsx` if the three apps consume the same button behaviour
- [x] T014 [P] Keep shared notification and relay error utilities in `src/web/packages/shared/src/notification/` and `src/web/packages/shared/src/relay-error/` where they are used outside this feature
- [x] T015 Remove form-only dependencies from `src/web/packages/shared/package.json` when the full Add/Edit Floor Plan forms remain app-local

---

## Phase 4: Verification

- [x] T016 [US1] Run focused webapp tests or type checks covering Add/Edit Floor Plan pages
- [x] T017 [US3] Run focused webapp-teams tests or type checks covering Add/Edit Floor Plan pages
- [x] T018 [US4] Run focused webapp-spaces tests or type checks covering Add/Edit Floor Plan pages
- [x] T019 Manually verify Add Floor Plan and Edit Floor Plan in all three apps against the quickstart checklist
- [x] T020 Regenerate app Relay artefacts with `src/web/apps/webapp/scripts/generate.sh` if any app-local GraphQL fragment changed

---

## Dependencies

```
Phase 1 layout updates
  -> Phase 2 behaviour preservation
    -> Phase 3 shared utility cleanup
      -> Phase 4 verification
```

## Notes

- The earlier shared-component plan was intentionally superseded. Do not add
  `src/web/packages/shared/relay.config.js` for this feature.
- Do not mark verification tasks complete until the relevant command or manual review has actually run.
