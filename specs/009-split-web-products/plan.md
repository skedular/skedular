# Implementation Plan: Split Web Products

**Branch**: `009-split-web-products` | **Date**: 2026-05-19 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/009-split-web-products/spec.md`

## Summary

Split the current mixed web application into three reviewable web app products without changing backend services, API contracts, or backend ownership. The implementation starts with a foundation phase that makes `webapp`, `webapp-spaces`, and `webapp-teams` run, build, test, and share the correct package foundations. After that, each migrated journey is delivered as an isolated slice: classify ownership, move app-owned code into the owning app, move neutral code into `@skedular/ui` or `@skedular/shared`, verify route retirement and backend-originated return URLs, then pause for manual review before the next slice.

## Technical Context

**Language/Version**: TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router  
**Primary Dependencies**: Relay 20.1.1, MUI 9, `@skedular/ui`, `@skedular/shared`, Vitest, React Testing Library, Turbo, pnpm 11.1.2  
**Storage**: N/A for this feature; backend persistence remains unchanged  
**Testing**: Vitest + React Testing Library per app/package, Next build checks, ESLint, Relay compiler where GraphQL surfaces move  
**Target Platform**: Web monorepo apps under `web/apps/*` deployed independently  
**Project Type**: Frontend-only web application migration across three Next.js apps  
**Performance Goals**: Each migrated primary journey remains usable without visible regressions; no completed slice should add avoidable full-page reloads or duplicate shared runtime bundles beyond existing app boundaries  
**Constraints**: No backend service/API/contract ownership changes; no hand-edited generated Relay/OpenAPI artefacts; British English user-facing copy; typography wrappers from `@skedular/ui`; neutral shared runtime code in `@skedular/shared`; app-specific rules remain in owning app  
**Scale/Scope**: Three app products (`webapp`, `webapp-spaces`, `webapp-teams`), shared packages (`web/packages/ui`, `web/packages/shared`), migration delivered as foundation plus one reviewable journey slice at a time

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature does not change `api-definitions/`, backend contracts, generated OpenAPI contracts, or backend GraphQL schema. If any web GraphQL operation moves or changes, Relay artefacts must be regenerated with the existing web Relay workflow rather than hand-edited.
- [x] **II. Domain Boundaries** — Backend domains and data ownership remain unchanged. The migration consumes existing public frontend-facing backend surfaces and does not access backend internals or databases.
- [x] **III. Testing** — Web UI changes require Vitest and React Testing Library coverage for moved app foundations and each migrated slice. Build/lint/Relay checks are required for affected apps. No persistence/integration backend tests are required unless a later slice proves otherwise.
- [x] **IV. Frontend** — The plan uses Next.js App Router, Relay colocation, generated artefact discipline, `@skedular/ui` typography wrappers, `@skedular/shared` for neutral runtime modules, and British English user-facing copy.
- [x] **V. Pattern Consistency** — The plan follows the constitution's existing three-app package model. No new framework is introduced. Any temporary adapter or duplicate route must be documented with owner, reason, and removal condition.
- [x] **VI. Logging** — The migration must preserve existing frontend diagnostics and add app/route-selection diagnostics where route retirement, app selection, or backend-originated return URL handling changes.

## Project Structure

### Documentation (this feature)

```text
specs/009-split-web-products/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── migration-slice-contract.md
│   ├── ownership-inventory-contract.md
│   └── route-retirement-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
web/
├── apps/
│   ├── webapp/                 # Customer-facing public discovery and customer organisation/subdomain surfaces
│   │   └── src/
│   ├── webapp-spaces/          # Marketplace/co-working organisation operator surfaces
│   │   └── src/
│   └── webapp-teams/           # Private organisation and team surfaces
│       └── src/
└── packages/
    ├── ui/                     # Neutral visual primitives, typography, theme, layout foundations
    │   └── src/
    └── shared/                 # Neutral hooks, providers, utilities, runtime helpers
        └── src/
```

**Structure Decision**: Use the existing three-app layout and central packages. Foundation work must make all three apps usable and independently verifiable before any large journey migration. Each subsequent migration slice moves only one app-owned journey or tightly related journey group, and must leave the affected apps buildable before user review.

## Phase 0: Research

See [research.md](./research.md). Main decisions:

- Use foundation-first delivery before feature movement.
- Use one migration slice at a time, grouped by reviewable user journey.
- Keep shared code narrow: `@skedular/ui` for visual primitives and `@skedular/shared` for neutral runtime helpers.
- Route retirement is allowed per completed slice only after backend-originated return URL usage is audited.
- Organisation selection is app-filtered while membership remains platform-wide.

## Phase 1: Design

See [data-model.md](./data-model.md) and [contracts/](./contracts/). The design artefacts define:

- ownership inventory fields used to classify routes, modules, and journeys
- migration slice lifecycle and review checkpoints
- route retirement and backend-originated return URL gate
- app-specific organisation filtering expectations
- no-backend-change boundary

## Phase 2: Execution Strategy

This feature must be implemented in reviewable stages.

### Stage 1 - Foundation

Goal: all three apps run, build, lint, and test with a shared shell and package boundary.

Deliverables:

- `webapp`, `webapp-spaces`, and `webapp-teams` each have a usable baseline shell.
- Each app imports shared visual primitives from `@skedular/ui`.
- Each app imports neutral providers/hooks/utilities from `@skedular/shared` where needed.
- Each app has an app identity, basic route shell, organisation-selection placeholder, empty states, and diagnostics suitable for later slices.
- No mixed feature migration is attempted until foundation verification passes.

Review checkpoint:

- User can run and inspect all three apps.
- User confirms the foundation is acceptable before journey slices begin.

### Stage 2 - Ownership Inventory

Goal: classify current `web/apps/webapp/src` journeys and modules before moving them.

Deliverables:

- Inventory covers routes, root pages, components, hooks, providers, utilities, Relay operations, and backend-originated return URL references.
- Each item is classified as WebApp, WebApp Spaces, WebApp Teams, `@skedular/ui`, `@skedular/shared`, or temporary transition.
- Items with backend-originated return URL risk are flagged before route deletion.

Review checkpoint:

- User can review the proposed migration order and challenge ownership before code moves.

### Stage 3 - Slice Migration Loop

Each slice follows the same lifecycle:

1. Select one journey or tightly related journey group.
2. Confirm ownership and backend return URL usage.
3. Move app-owned pages/components/hooks into the target app.
4. Move neutral visual/runtime foundations into `@skedular/ui` or `@skedular/shared`.
5. Regenerate Relay artefacts if GraphQL operations move or change.
6. Retire, redirect, block, or document old WebApp routes according to the route-retirement gate.
7. Add or update focused tests.
8. Run app-specific verification.
9. Stop for user review before the next slice.

Recommended first migration order after foundation:

1. WebApp Teams organisation-selection foundation: private organisations only, no marketplace product concepts.
2. WebApp Spaces organisation-selection foundation: marketplace/co-working organisations only.
3. WebApp public root discovery shell: customer-facing root URL remains public marketplace discovery.
4. WebApp co-working subdomain customer-facing shell.
5. WebApp private organisation subdomain customer-facing shell.
6. Marketplace operator slices into WebApp Spaces, one journey at a time.
7. Private organisation/team slices into WebApp Teams, one journey at a time.
8. Remaining shared/neutral component extraction and stale route cleanup.

### Verification Per Slice

Minimum checks for every completed slice:

- affected app lint
- affected app tests
- affected app build
- relevant Relay generation/check if GraphQL changed
- manual route check in the target app
- stale route check in the old app
- backend-originated return URL audit for any retired/deleted route

## Post-Design Constitution Check

- [x] **I. Contract-First** — Design avoids backend contract changes. Relay regeneration is explicitly required only when web GraphQL operations move or change.
- [x] **II. Domain Boundaries** — Design keeps backend domains untouched and treats backend-originated return URLs as frontend URL dependencies to audit, not backend ownership changes.
- [x] **III. Testing** — Design requires per-slice app/package tests and build/lint checks.
- [x] **IV. Frontend** — Design uses existing app/package boundaries and constitution-approved shared packages.
- [x] **V. Pattern Consistency** — Design preserves current monorepo patterns and documents temporary transition paths.
- [x] **VI. Logging** — Design requires diagnostics for app selection, route retirement, verification failures, and return URL handling.

## Complexity Tracking

No constitution violations require justification.
