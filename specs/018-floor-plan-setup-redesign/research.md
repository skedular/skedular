# Research: Floor Plan Setup Page Redesign

**Feature**: `018-floor-plan-setup-redesign`
**Date**: 2026-05-30

## Summary

All unknowns from the Technical Context have been resolved. This document records each research
question, the decision made, the rationale, and alternatives that were evaluated.

---

## Decision 1 — Modern Layout Pattern to Replace `AppBarWithStackColumn`

**Question**: Which `@skedular/ui` pattern should replace `AppBarWithStackColumn` to match the
modern page style used by other location management pages?

**Decision**: Use the same pattern as `add-resource-dialog.tsx` in page-presentation mode:

- `Box` with responsive CSS Grid: `maxWidth: 1320`, `mx: 'auto'`, `px: { xs: 2, md: 3 }`, `py: 3`
- `PageHeaderPanel` — page-level title with optional description and eyebrow label
- One or more `SettingsSectionCard` blocks — each logical section of the form
- `EditorActionBar` — sticky action bar for Save / Cancel / Dismiss actions (replaces inline `StackRow` button rows)

**Rationale**: This pattern is already established in the codebase (Add Resource, Edit Resource,
and organisation settings pages all use it). It produces a centered, light-background layout
with no secondary app bar. Reusing the existing pattern satisfies the constitution's
"pattern consistency" gate and requires zero new shared layout primitives.

**Alternatives considered**:

- `AppBarWithStackColumn` restyled — rejected: the dark secondary toolbar is load-bearing in the
  current component; restyling it to match the page background would still leave structural
  asymmetry and is harder to test than a full replacement
- Full-width canvas with centered form above — rejected: user confirmed (Q3) that the canvas
  should remain inside `SettingsSectionCard`, contained within the centered column

---

## Decision 2 — Canvas Presentation

**Question**: How should the floor plan image canvas (where resources are positioned by dragging)
be presented in the new layout?

**Decision**: Wrap the canvas `Box` inside a `SettingsSectionCard` titled "Floor Plan Layout"
(or "Floor Plan"). The `SettingsSectionCard` sits in the same centered column as the form fields
above it.

**Rationale**: User confirmed in Q3. Keeps the canvas visually contained and consistent with
the card treatment on the rest of the page. The canvas's intrinsic `width`/`height` from the
floor plan image drives its visual size; the card wrapper gives it a clean border and shadow.

**Alternatives considered**:

- Canvas full-width below the centered column — rejected per Q3 answer
- Canvas in its own `PageHeaderPanel` — rejected per Q3 answer; `PageHeaderPanel` is for
  top-of-page title sections, not interactive content areas

---

## Decision 3 — App-Local Add/Edit Implementations

**Question**: The three webapps each have copies of `add-floor-plan.tsx` and
`edit-floor-plan.tsx`. Should the full components be consolidated?

**Decision**: Keep the full `AddFloorPlan` and `EditFloorPlan` implementations app-local in
webapp, webapp-teams, and webapp-spaces. Apply the same redesign and behavioural fixes to all
three copies.

**Rationale**: The implementation direction changed after review. Keeping the full components
app-local avoids adding domain-feature Relay components and shared Relay compiler plumbing to
`@skedular/shared`, while still allowing the visual redesign to ship consistently across all
three products.

**Alternatives considered**:

- Full shared Relay component in `@skedular/shared` — rejected after implementation review; it
  adds shared package and generation complexity that is not wanted for this feature
- Shared presentational component + per-app Relay wiring — rejected for now; it adds more files
  without materially reducing the risk in this small redesign

**Consistency note**: Since the components remain app-local, fixes must be applied to all three
copies together unless there is a product-specific reason to diverge.

---

## Decision 4 — App-Local Import Resolution

**Question**: The current floor plan components import from app-local paths (`@/components/icons`,
`@/components/loading`, `@/components/notification`, `@/components/relayError`,
`@/libs/image-file-uploader`, `@/clients/openapi/.../FileUploadResponse`). These cannot be used
from `@skedular/shared`. How should each be resolved?

**Decisions per dependency**:

| Current import                                                                    | Resolution                                                                         | Rationale                                                                                                      |
| --------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------- |
| `@/components/icons` — `DeskIcon`, `RoomIcon`, `ParkingIcon`, `OtherResourceIcon` | Import directly from `@mui/icons-material` in shared                               | The icons file is just re-exporting MUI icons; `@mui/material` is already a peer dep of `@skedular/shared`     |
| `@/components/loading` — `Loading`                                                | Replace with `<CircularProgress />` from `@mui/material`                           | `Loading` is a thin wrapper; using MUI directly removes the per-app dependency                                 |
| `@/components/notification` — `NotificationContent`, `errorNotificationOptions`   | Move to `src/web/packages/shared/src/notification/`                                | These are utilities (not feature components) used identically in all three apps; they belong in shared         |
| `@/components/relayError` — `RelayError`, `toRootError`                           | Move to `src/web/packages/shared/src/relay-error/`                                 | Relay error display is cross-product utility code; fits naturally alongside existing Relay utilities in shared |
| `@/libs/image-file-uploader` — `ImageFileUploader`                                | Import from `@skedular/shared` directly                                            | `ImageFileUploader` already lives in `@skedular/shared/src/image-file-uploader/`                               |
| `@/clients/openapi/.../FileUploadResponse`                                        | Define a local `ImageUploadResult` interface in `@skedular/shared/src/floor-plan/` | Only the `url` field is used; no need to depend on generated OpenAPI client type                               |

**Alternatives considered**:

- Inject app-local dependencies as render props/component props — rejected: adds prop-drilling
  ceremony for dependencies that are identical across all callers; no benefit
- Keep app-local imports via path aliases configured in shared — rejected: `@/` aliases are
  Next.js-specific and cannot work in a package consumed by three different apps

---

## Decision 5 — Relay Compiler Configuration

**Question**: Does this feature require a new Relay compiler configuration?

**Decision**: No new shared Relay compiler configuration is needed. The floor plan GraphQL
fragments remain in each app, and the existing app Relay compiler passes continue to own their
generated artefacts.

**Alternatives considered**:

- Add `src/web/packages/shared/relay.config.js` — rejected because the full floor plan components
  no longer move to `@skedular/shared`
- Move only fragments to shared — rejected because fragments should stay collocated with the
  app-local component implementations

---

## Decision 6 — Relay Fragment Key Ownership

**Question**: Where should generated Relay fragment key types live after the app-local direction?

**Decision**: Generated Relay types remain in each app's `src/queries/__generated__/` directory.
Each app-local floor plan component imports the generated types from its own app package.

**Rationale**: Types follow the app-local component and fragment definitions. No shared
`@skedular/shared/src/queries/__generated__/` output is needed for this feature.

---

## Decision 7 — Add Floor Plan Label Bug Fix

**Question**: The existing `AddFloorPlan` component has `<AppBarWithStackColumn label="Add Location">` —
the label incorrectly says "Add Location" instead of "Add Floor Plan". Should this be fixed?

**Decision**: Yes — fix the label to "Add Floor Plan" as part of the redesign. This is a trivial
correction with no risk.

**Rationale**: The label was clearly a copy-paste error. The redesign replaces the whole wrapper
so the fix is naturally included.

---

## Summary Table

| #   | Decision                                                                     | Status      |
| --- | ---------------------------------------------------------------------------- | ----------- |
| 1   | Modern layout: `PageHeaderPanel` + `SettingsSectionCard` + `EditorActionBar` | ✅ Resolved |
| 2   | Canvas inside `SettingsSectionCard` within centered column                   | ✅ Resolved |
| 3   | Keep Add/Edit Floor Plan implementations app-local and aligned               | ✅ Resolved |
| 4   | App-local imports remain app-local; shared utilities only where appropriate  | ✅ Resolved |
| 5   | No shared Relay compiler config required                                     | ✅ Resolved |
| 6   | Generated Relay types stay app-local                                         | ✅ Resolved |
| 7   | Fix "Add Location" label bug to "Add Floor Plan"                             | ✅ Resolved |
