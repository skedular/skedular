# Implementation Plan: Floor Plan Setup Page Redesign

**Branch**: `018-floor-plan-setup-redesign` | **Date**: 2026-05-30 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/018-floor-plan-setup-redesign/spec.md`

## Summary

Redesign the Add Floor Plan and Edit Floor Plan pages across all three webapps (webapp,
webapp-teams, webapp-spaces) to match the modern design used by other location management pages:
centered content panel, standard app background, no secondary dark app bar. The full Add/Edit
Floor Plan implementations remain app-local in each webapp, and the redesign must be applied
consistently to all three copies. The redesign covers both the outer chrome and the
canvas/resource-placement area.

## Technical Context

**Language/Version**: TypeScript 6, React 19, Next.js 16 App Router  
**Primary Dependencies**: `@skedular/ui`, `@skedular/shared`, Relay 21, MUI 9, `react-final-form`, `mui-rff`, `react-toastify`, `yup`  
**Storage**: N/A — frontend only; no persistence changes  
**Testing**: Vitest + React Testing Library  
**Target Platform**: Web browser (desktop-first; Teams-embedded for webapp-teams)  
**Project Type**: Web application — frontend-only redesign  
**Performance Goals**: Canvas drag interaction must remain smooth (no regression from current)  
**Constraints**: Must not break floor plan create, update, image upload, or resource positioning workflows  
**Scale/Scope**: 3 webapps × 2 pages (Add + Edit) = 6 app-local page/component instances kept aligned

## Constitution Check

_GATE: Re-checked after Phase 1 design. All gates pass._

- [x] **I. Contract-First** — No `api-definitions/` changes. Relay fragments remain app-local,
      so no shared Relay compiler config or shared generated Relay artefacts are required. If
      app-local GraphQL fragments change, regenerate through `src/web/apps/webapp/scripts/generate.sh`.

- [x] **II. Domain Boundaries** — Frontend only. No domain boundary concerns.

- [x] **III. Testing** — Frontend-only change. Vitest + React Testing Library coverage should focus
      on the app-local Add/Edit Floor Plan behaviour: render, validation, successful create
      navigation, failed create staying on page, edit auto-save, image upload, and resource positioning.
      No backend integration tests required.

- [x] **IV. Frontend** — ✅ Relay fragments remain collocated with the app-local components that own them.
      ✅ No hand-editing of generated Relay artefacts.
      ✅ Typography wrappers from `@skedular/ui` used throughout.
      ✅ User-facing copy: American spelling per workspace preference override.

- [x] **V. Pattern Consistency** — No shared domain-feature Relay component deviation is required;
      the existing app-local component ownership model remains intact.

- [x] **VI. Logging** — Frontend-only. No structured backend logging changes required.
      User-facing feedback uses existing toast notification pattern.

## Project Structure

### Documentation (this feature)

```text
specs/018-floor-plan-setup-redesign/
├── plan.md                    ← This file
├── research.md                ← Phase 0 output
├── data-model.md              ← Phase 1 output
├── quickstart.md              ← Phase 1 output
├── contracts/
│   └── component-contracts.md    ← Phase 1 output
└── tasks.md                   ← Phase 2 output (from /speckit.tasks)
```

### Source Code (repository root)

```text
src/web/apps/webapp/src/components/
├── floorPlan/
│   ├── addFloorPlan/
│   │   ├── add-floor-plan.tsx                    ← UPDATED app-local implementation
│   │   └── index.ts
│   └── editFloorPlan/
│       ├── edit-floor-plan.tsx                   ← UPDATED app-local implementation
│       └── index.ts

# webapp-teams and webapp-spaces: same app-local ownership pattern as webapp above
```

**Structure Decision**: Keep full floor-plan setup components and Relay fragments app-local.
Shared utilities may remain shared, but this feature does not introduce shared Add/Edit Floor Plan
components or a shared Relay compiler pass.

## Complexity Tracking

| Violation                                                                                                                             | Why Needed                                                                                                                                                                                                                                        | Simpler Alternative Rejected Because                                                                                                                                                                                                                     |
| ------------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
No complexity deviations are required after the implementation direction changed to keep the full Add/Edit Floor Plan components app-local.
