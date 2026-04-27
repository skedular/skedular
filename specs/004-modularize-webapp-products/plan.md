# Implementation Plan: Modularize Webapp Products

**Branch**: `004-modularize-webapp-products` | **Date**: 2026-04-27 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `specs/004-modularize-webapp-products/spec.md`

## Summary

The three Skedular web products (`webapp`, `webapp-teams`, `webapp-spaces`) each have their own domain and are independently deployed Next.js applications. Currently `webapp` contains all shared infrastructure — providers, theme tokens, utilities, typography wrappers — alongside all Skedular-specific feature components. `webapp-teams` and `webapp-spaces` are stubs with placeholder pages.

The goal is to extract shared code into two workspace packages so Teams and Spaces can be built as real products without copying from `webapp`. The technical approach:

1. Expand `web/packages/ui` (`@skedular/ui`) with typography wrappers and theme tokens currently living in `webapp`.
2. Create `web/packages/shared` (`@skedular/shared`) — the single centralised package for all cross-product runtime modules: React providers, hooks, utilities (date, name, Relay error helpers — the canonical example), MUI helpers, cookie consent, and image upload.
3. Update `webapp` to import from both packages instead of owning the code locally.
4. Wire `webapp-teams` and `webapp-spaces` to consume both packages, providing the foundation for real product content.
5. Update all imports, documentation, and unit tests throughout.

## Technical Context

**Language/Version**: TypeScript 6, React 19
**Primary Dependencies**: Next.js 16 (App Router), Relay, MUI v9, pnpm workspaces, Turborepo
**Storage**: N/A — frontend only
**Testing**: Vitest + React Testing Library
**Target Platform**: Vercel — each product on its own domain
**Project Type**: pnpm monorepo with three Next.js apps and two workspace packages
**Performance Goals**: No regression in Vercel build times or bundle sizes
**Constraints**: No hand-editing Relay-generated artefacts; no direct `@mui/material/Typography` in feature components; British spelling in user-facing copy; `@skedular/ui` must not depend on `@skedular/shared`
**Scale/Scope**: ~50 component directories in `webapp`; ~11 files in `web/packages/ui`; 2 stub apps to bootstrap

## Constitution Check

_Re-checked after Phase 1 design. All gates pass._

- [x] **I. Contract-First** — This feature does not touch `api-definitions/` or change any backend contract surface. If any Relay fragment file moves location, `web/apps/webapp/scripts/generate.sh` must be re-run to keep generated artefacts aligned. No new API definitions are introduced.
- [x] **II. Domain Boundaries** — Purely frontend restructure. No cross-domain backend access. No persistence, Kafka, or Temporal involvement.
- [x] **III. Testing** — Unit tests required for all moved components, hooks, and utilities. Vitest + React Testing Library. No persistence boundaries, so no integration tests required. Tests for moved modules must be updated to import from new package paths.
- [x] **IV. Frontend** — This is a frontend-only change. Relay colocation is preserved throughout (fragments stay with their components). No hand-editing of generated artefacts. Typography wrappers move to `@skedular/ui` (their canonical home) — import paths in consumers update accordingly. British spelling enforced in all new or updated user-facing copy.
- [x] **V. Pattern Consistency** — **New pattern introduced**: multi-package web monorepo with `@skedular/ui` (visual primitives) and `@skedular/shared` (runtime providers/hooks). Justification: three separately deployed products require shared but product-isolated modules; a pnpm workspace package is the correct established solution for this monorepo architecture.
- [x] **VI. Logging** — Existing `libs/logging/index.ts` logger config stays per-product (product name in `app:` field differs per product). `@skedular/shared` does not own a logger — each product continues to configure its own. The existing LogRocket provider moves to `@skedular/shared` as a component; the product-specific LogRocket app ID remains per-product config. Observability continuity explicitly required by LOG-001 through LOG-004.

## Project Structure

### Documentation (this feature)

```text
specs/004-modularize-webapp-products/
├── plan.md          ← this file
├── research.md      ← Phase 0 decisions
├── data-model.md    ← Phase 1 ownership map
├── quickstart.md    ← Phase 1 developer guide
└── tasks.md         ← Phase 2 (/speckit.tasks)
```

### Source Code — Target Structure

```text
web/
  packages/
    ui/                          # @skedular/ui — EXPANDED
      src/
        ← existing layout primitives (unchanged)
        typography/              # NEW — moved from webapp/src/components/commons/
          body-icon-typography.tsx
          caption-icon-typography.tsx
          ... (all *-typography.tsx wrappers)
          index.ts
        theme/                   # NEW — moved from webapp/src/libs/theme/
          theme-primitives.ts
          theme.ts
          index.ts
        index.ts                 # updated to re-export typography/ and theme/

    shared/                  # @skedular/shared — NEW PACKAGE
      package.json
      tsconfig.json
      src/
        providers/               # moved from webapp/src/libs/providers/
          relay-provider.tsx
          theme-provider.tsx
          palette-mode-provider.tsx
          date-picker-localization-provider.tsx
          google-analytics-provider.tsx
          logrocket-provider.tsx
          in-msteams-provider.tsx
          index.ts
        hooks/                   # moved from webapp/src/libs/providers/
          use-known-params.ts
          use-integrated-platform.ts
          index.ts
        utils/                   # moved from webapp/src/libs/utils/
          (date helpers, name helpers, relay error helpers)
          index.ts
        mui/                     # moved from webapp/src/libs/mui/
          muix-license.tsx
          index.ts
        cookie-consent/          # moved from webapp/src/libs/cookie-consent/
        image-file-uploader/     # moved from webapp/src/libs/image-file-uploader/
        index.ts

  apps/
    webapp/                      # Core Skedular scheduler — UPDATED imports
      src/
        app/                     # Unchanged route tree (owns auth/account entry points)
        components/
          commons/               # REMOVED — moved to @skedular/ui
          ← all feature components unchanged (booking/, marketplace/, etc.)
        libs/
          analytics/             # Unchanged — product-specific GA/GTM config
          logging/               # Unchanged — product-specific logger (app: 'webapp')
          providers/             # REMOVED — moved to @skedular/shared
          theme/                 # REMOVED — moved to @skedular/ui
          utils/                 # REMOVED — moved to @skedular/shared
          mui/                   # REMOVED — moved to @skedular/shared
          cookie-consent/        # REMOVED — moved to @skedular/shared
          image-file-uploader/   # REMOVED — moved to @skedular/shared

    webapp-teams/                # Skedular for Teams — BOOTSTRAPPED
      src/
        app/
          layout.tsx             # Updated: uses @skedular/shared providers
          page.tsx               # Teams product landing page
        libs/
          logging/               # NEW — product logger (app: 'webapp-teams')
          analytics/             # NEW — Teams-specific GA/GTM config

    webapp-spaces/               # Skedular for Spaces — BOOTSTRAPPED
      src/
        app/
          layout.tsx             # Updated: uses @skedular/shared providers
          page.tsx               # Spaces product landing page
        libs/
          logging/               # NEW — product logger (app: 'webapp-spaces')
          analytics/             # NEW — Spaces-specific GA/GTM config
```

**Structure Decision**: pnpm workspace packages for shared code; product apps own only product-specific configuration, feature components, and route trees. `@skedular/ui` is the visual primitive layer; `@skedular/shared` is the runtime infrastructure layer. One-way dependency: `app-shared` may import from `ui`; `ui` never imports from `app-shared`.

## Complexity Tracking

| Violation                                  | Why Needed                                                               | Simpler Alternative Rejected Because                                                                  |
| ------------------------------------------ | ------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------- |
| New workspace package (`@skedular/shared`) | Three separately deployed products need shared React providers and hooks | Copying into each app creates three diverging copies; `@skedular/ui` must stay visual-only per FR-003 |
