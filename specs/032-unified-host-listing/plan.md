# Implementation Plan: Unified Host Listing Experience

**Branch**: `032-unified-host-listing` | **Date**: 2026-07-08 | **Spec**: `/specs/032-unified-host-listing/spec.md`
**Input**: Feature specification from `/specs/032-unified-host-listing/spec.md`

## Summary

Skedular Host will expose a unified listing experience where hosts manage location and pricing/rules setup through location-first screens. The first screen is a grouped card entry page that hides Product terminology and routes users into focused edit pages. Frontend coordinates existing Location and Product GraphQL operations; no new backend aggregation service/API is introduced unless a proven blocker appears.

## Technical Context

**Language/Version**: TypeScript 6.0.3 (frontend), C# .NET 10 (backend unchanged for this slice)
**Primary Dependencies**: Next.js 16 App Router, React 19, Relay, MUI 9, `@skedular/ui`, `@skedular/shared`
**Storage**: PostgreSQL via existing domain services (no new persistence for Phase 1)
**Testing**: Vitest + React Testing Library (route redirects, page flows, coordinator state)
**Target Platform**: Web (Skedular Host app)
**Project Type**: Frontend web app refactor/composition
**Performance Goals**: Keep locations page regression within SC-003 limit (+500ms max)
**Constraints**:

- Use existing Location/Product GraphQL APIs
- No new backend orchestration API/service unless concrete blocker
- Keep Product concept hidden from host UX
- Preserve existing backend validation rules

**Scale/Scope**: Host listing create/edit/list flows and legacy product-route cleanup

## Constitution Check

_GATE: Must pass before implementation._

- [x] **I. Contract-First** — No `api-definitions/` changes in this slice; no generated surface regenerated.
- [x] **II. Domain Boundaries** — Frontend orchestration uses public GraphQL APIs only; no cross-domain DB/internal access.
- [x] **III. Testing** — Frontend tests planned for redirect behavior, grouped card entry flow, and pending-to-ready product state handling.
- [x] **IV. Frontend** — Relay usage preserved; no hand-edited generated artifacts; user-facing copy remains American English.
- [x] **V. Pattern Consistency** — Uses existing Host card-group navigation pattern and focused edit pages, not a new tab-first interaction model.
- [ ] **VI. Logging** — Frontend flow logging/tracing tasks remain to be completed in tasks (LOG-001..LOG-004 coverage gap still open).

## Project Structure

### Documentation (this feature)

```text
specs/032-unified-host-listing/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
└── tasks.md
```

### Source Code (repository root)

```text
src/web/apps/webapp-host/src/
├── app/
│   ├── locations/
│   └── products/
├── components/
│   ├── navigationMenu/
│   └── unified-listing-form/
└── queries/
```

**Structure Decision**: Implement in `webapp-host` by composing existing location/product editor behavior behind location-centric grouped cards and route redirects.

## Phase Plan

1. Setup and scaffold consolidation
2. Frontend foundational query/coordinator/pending-shell infrastructure
3. Legacy host product route and navigation cleanup
4. Create/edit/list flow convergence onto location-centric grouped-card UX
5. Validation and quickstart verification

## Known Open Items

- Explicit logging tasks (LOG-001..LOG-004) are not implemented yet.
- Booking confirmation explicit task coverage still needs to be added/aligned.
- Performance/migration validation tasks for SC-003 and SC-005 remain open.
