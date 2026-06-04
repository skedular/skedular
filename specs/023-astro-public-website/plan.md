# Implementation Plan: Astro Public Website

**Branch**: `023-astro-public-website` | **Date**: 2026-06-04 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/023-astro-public-website/spec.md`

## Summary

Add the first source-controlled public Skedular website as a new static Astro app under `src/web/apps/public-web`. The app will deliver one minimal home page that accurately represents Skedular at a high level, integrates with the existing pnpm/Turborepo web workspace, and remains deployable as static output to Cloudflare Pages/Workers static assets or Vercel without adding a server runtime.

## Technical Context

**Language/Version**: TypeScript 6.0.3, Astro static site, Node.js 22 via `src/web` workspace  
**Primary Dependencies**: Astro, TypeScript, pnpm 11.5.1 workspace, Turborepo 2.9.x, Vitest, Testing Library DOM utilities, axe accessibility testing; app-local formatting/lint tooling consistent with current web apps
**Storage**: N/A - static website source files only  
**Testing**: Vitest with Testing Library DOM utilities for the rendered Astro home page, axe accessibility assertions, build-diagnostics success/failure tests, Astro diagnostics (`astro check`), Astro production build, app-local lint/format scripts, local preview and hot-reload validation, root `src/web` Turbo build/lint/format and workspace version-sync verification
**Target Platform**: Static web output for Cloudflare Pages/Workers static assets first; Vercel static Astro project as fallback  
**Project Type**: Static public marketing website app inside the existing web monorepo  
**Performance Goals**: One-page static output; first page should load in under 2 seconds on standard broadband from the deployed Cloudflare URL when available; avoid unnecessary client JavaScript
**Constraints**: One page only; no placeholder pages; no full WordPress migration; no SSR/server runtime; no product auth/Relay/API dependency; `PUBLIC_SKEDULAR_SIGNUP_URL` is required at build time and has no fallback; American English copy; compatible with JavaScript disabled
**Scale/Scope**: One new app (`public-web`), one route (`/`), README, workspace scripts, static deployment documentation

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** - This feature does not touch `api-definitions/`, generated GraphQL, Relay artifacts, OpenAPI clients, protobufs, or backend generated surfaces. No generator is required.
- [x] **II. Domain Boundaries** - This feature is a standalone public static web app and does not cross backend domain ownership, persistence, service, event, or workflow boundaries.
- [x] **III. Testing** - Static frontend work includes Vitest, Testing Library DOM utilities, axe accessibility checks, build-diagnostics success/failure tests, Astro diagnostics, build, lint/format, local preview, hot-reload validation, and manual accessibility/content review. Testing Library DOM utilities are the Astro-appropriate equivalent of React Testing Library because this page intentionally has no React runtime.
- [x] **IV. Frontend** - This feature includes web changes. It intentionally does not use Next.js App Router, Relay, or MUI typography wrappers because the stakeholder requires Astro and the page is static. No generated artifacts are hand-edited. All user-facing copy must use American spelling and grammar.
- [x] **V. Pattern Consistency** - This introduces a justified new frontend pattern: Astro for a static public marketing website. The existing Next.js/Relay/MUI pattern remains required for authenticated product apps; Astro is limited to `public-web` because it avoids unnecessary product runtime and server complexity for static marketing content.
- [x] **VI. Logging** - No runtime business workflow is added. Observability scope is build/static diagnostics: emit and test structured page-count/output-size metadata, verify successful and failure output remains visible, document output/deployment settings, avoid sensitive environment leakage, and document where future analytics can be integrated.

## Project Structure

### Documentation (this feature)

```text
specs/023-astro-public-website/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-website-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/web/
├── package.json                 # add required public-web convenience scripts
├── pnpm-workspace.yaml          # existing apps/* pattern discovers public-web
├── turbo.json                   # existing task names cover package scripts
└── apps/
    ├── README.md                # update topology to include public-web
    └── public-web/
        ├── README.md
        ├── astro.config.mjs
        ├── package.json
        ├── vitest.config.ts
        ├── scripts/
        │   └── report-build.mjs
        ├── public/
        ├── src/
            ├── env.d.ts
            ├── pages/
            │   └── index.astro
            └── styles/
                └── global.css
        └── tests/
            ├── build-diagnostics.test.ts
            └── home-page.test.ts
```

**Structure Decision**: Implement as one new Astro app at `src/web/apps/public-web`, discovered by the existing `apps/*` pnpm workspace. Keep source changes scoped to the new app plus minimal shared web workspace registration/documentation. Do not modify existing product apps or generated artifacts.

## Complexity Tracking

| Violation                                                       | Why Needed                                                                                                                               | Simpler Alternative Rejected Because                                                                                                                                         |
| --------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| Astro app instead of the current Next.js/Relay frontend pattern | The stakeholder explicitly requires Astro, and the public website is a static marketing surface rather than an authenticated product app | Reusing Next.js would violate the request and add unnecessary product-runtime patterns; static HTML outside the workspace would fail monorepo script/deployment requirements |

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical context items are resolved; no `NEEDS CLARIFICATION` items remain.

## Phase 1: Design & Contracts

Design artifacts:

- [data-model.md](./data-model.md) - app, home page, deployment, and build diagnostics entities.
- [contracts/public-website-contract.md](./contracts/public-website-contract.md) - UI/content/deployment contract for the static public website.
- [quickstart.md](./quickstart.md) - local development, validation, and deployment notes.

## Post-Design Constitution Check

- [x] **I. Contract-First** - Design artifacts confirm no API contracts, schema files, generated Relay artifacts, or OpenAPI clients are touched.
- [x] **II. Domain Boundaries** - Design remains limited to a static public web app and workspace registration.
- [x] **III. Testing** - Quickstart and tasks define Vitest/Testing Library DOM home-page tests, axe accessibility assertions, build-diagnostics success/failure tests, Astro check/build/preview, lint/format, hot-reload validation, and manual content/accessibility review.
- [x] **IV. Frontend** - Astro exception is documented and constrained to `public-web`; no Relay/generated artifacts are involved; American English remains required.
- [x] **V. Pattern Consistency** - Complexity tracking documents the new-framework exception and why simpler alternatives were rejected.
- [x] **VI. Logging** - Static diagnostics, success/failure log verification, and future analytics extension points are documented; no runtime structured logging is required for v1 because there is no server or business workflow.
