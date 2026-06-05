# Implementation Plan: Public Website Content Integration

**Branch**: `025-public-website-content` | **Date**: 2026-06-05 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/025-public-website-content/spec.md`

## Summary

Expand the existing Astro `public-web` app from a single-page public site into a full content-led public website based on `src/web/apps/public-web/public-website-content-draft.md` and the current live public website inventory. The implementation will add product pages, pricing, resource/blog/support migration, feature and comparison pages, SEO/AI-discoverability metadata, structured-data support, route redirects, and review inventories. Booking/search actions remain outbound links to the separate application website, configured through three required public environment variables with no hardcoded staging or production destination domains.

## Technical Context

**Language/Version**: TypeScript 6.0.3; Astro 6.4.4; Node.js 22; pnpm 11.5.1  
**Primary Dependencies**: Astro static site generation, `@astrojs/check`, Vitest, Testing Library DOM, JSDOM, axe-core, Prettier with Astro plugin, Wrangler for Cloudflare Pages upload  
**Storage**: Static repository files only; no database, server session, or runtime persistence  
**Testing**: `pnpm --dir src/web/apps/public-web test`, `check`, `lint`, `build`; Vitest/JSDOM page tests; axe critical-accessibility checks; content-inventory review; redirect/link validation  
**Target Platform**: Static public website under `src/web/apps/public-web`, deployed through Cloudflare Pages direct upload; Vercel remains a fallback static host  
**Project Type**: Static Astro web application with content collections/pages and deployment configuration  
**Performance Goals**: Preserve static-first delivery; target draft goals of Lighthouse Performance 95+, Accessibility 95+, SEO 100, Best Practices 100, LCP under 1.5s, INP under 200ms, CLS under 0.05 when measured against staging when available or a local static preview as the fallback  
**Constraints**: No direct public-site booking or checkout; booking/search links forward to the separate app website; exactly three required public URL variables; no hardcoded staging/production destination domains; full current blog/support migration; all draft comparison pages published; American English; friendly professional human-written copy; complete source-draft coverage inventory  
**Scale/Scope**: Multi-page public website covering home, Teams, Spaces, pricing, resources/blog/support, feature pages, comparison pages, redirects, metadata, structured data, and review inventories from the 1,006-line draft

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature does not touch `api-definitions/`, generated GraphQL schemas, Relay artifacts, OpenAPI clients, protobufs, or backend contracts. No generator is required.
- [x] **II. Domain Boundaries** — This feature is public static website work only. It does not cross backend domain ownership, persistence, service, workflow, or event boundaries.
- [x] **III. Testing** — Static web changes require Astro type checks, app-local Vitest tests, accessibility checks, build diagnostics tests, link/redirect validation, content inventory review, and workspace-level web validation where practical. Backend unit/integration tests are not required because no backend code or persistence boundary is touched.
- [x] **IV. Frontend** — This feature includes web changes in the existing Astro public website, not the Next.js/Relay product apps. Relay colocation and typography-wrapper gates are not applicable. User-facing copy must use American spelling and grammar.
- [x] **V. Pattern Consistency** — Use the existing `src/web/apps/public-web` Astro static-site pattern established by `023-astro-public-website`. This is not a new framework introduction for the repo; it expands the existing public-web app. Continue existing build diagnostics, environment validation, and Cloudflare static deployment patterns.
- [x] **VI. Logging** — No runtime business workflow is added. Observability scope is build diagnostics, clear missing-environment failures, link/redirect validation output, and reviewable content/source inventories. Static public pages must not expose sensitive values.

## Project Structure

### Documentation (this feature)

```text
specs/025-public-website-content/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── public-website-contract.md
└── tasks.md                  # Created by /speckit-tasks, not this command
```

### Source Code (repository root)

```text
src/web/apps/public-web/
├── public-website-content-draft.md
├── .env.template
├── README.md
├── astro.config.mjs
├── package.json
├── scripts/
│   └── report-build.mjs
├── src/
│   ├── env.d.ts
│   ├── content/              # planned content data and markdown/MDX collections
│   ├── data/                 # planned navigation, URL, pricing, inventory, and redirect data
│   ├── layouts/              # planned shared page layouts
│   ├── pages/
│   │   ├── index.astro
│   │   ├── teams.astro
│   │   ├── spaces.astro
│   │   ├── pricing.astro
│   │   ├── resources/
│   │   ├── support/
│   │   ├── features/
│   │   └── compare/
│   └── styles/
│       └── global.css
├── tests/
│   ├── build-diagnostics.test.ts
│   ├── home-page.test.ts
│   └── public-site-content.test.ts   # planned route/link/content validation
└── infrastructure/workspaces/
    ├── common/
    ├── staging/
    └── production/
```

**Structure Decision**: Implement inside the existing `src/web/apps/public-web` Astro app. Add content/data/layout folders only as needed to keep the large public-site inventory maintainable. Keep planning artifacts under `specs/025-public-website-content/`. Do not add backend services, GraphQL operations, Relay artifacts, or shared product-app dependencies.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical context items are resolved; no `NEEDS CLARIFICATION` markers remain.

## Phase 1: Design & Contracts

Design artifacts:

- [data-model.md](./data-model.md) - static content entities, inventories, page metadata, route state, and validation rules.
- [contracts/public-website-contract.md](./contracts/public-website-contract.md) - public website content, route, URL environment, redirect, SEO, accessibility, and review contracts.
- [quickstart.md](./quickstart.md) - implementation and verification commands for local development, build, tests, content review, and deployment checks.

## Post-Design Constitution Check

- [x] **I. Contract-First** — Design artifacts confirm no API/generated contract surfaces are changed.
- [x] **II. Domain Boundaries** — Design stays within static public-web content, outbound links, and deployment configuration.
- [x] **III. Testing** — Quickstart defines app-local and workspace-level validation, accessibility checks, build diagnostics, and content/link/redirect review.
- [x] **IV. Frontend** — Design uses Astro public-web patterns and requires American English and human-quality public copy. Next.js/Relay-specific gates remain not applicable.
- [x] **V. Pattern Consistency** — Design preserves existing public-web static architecture, environment validation, Cloudflare static deployment, and build-reporting patterns.
- [x] **VI. Logging** — Design records build diagnostics, validation outputs, and review inventories as the observable outputs for this static feature.
