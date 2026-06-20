# Implementation Plan: Skedular Competitor Comparison Hub

**Branch**: `029-competitor-comparison-hub` | **Date**: 2026-06-20 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/029-competitor-comparison-hub/spec.md`

## Summary

Replace the current one-off public-web comparison implementation with a complete, data-driven competitor comparison hub. The feature removes the existing hardcoded Skedda-style comparison surface with no redirect or alias, introduces `/compare` as the comparison index, generates all required comparison and supporting SEO pages from a shared competitor dataset and normalized feature matrix, and keeps Skedular and competitor claims publishable only when backed by current evidence or explicit review status. The implementation stays inside the existing Astro `src/web/apps/public-web` static site and extends its current content inventory, metadata, sitemap, structured-data, and Vitest/JSDOM validation patterns.

## Technical Context

**Language/Version**: TypeScript 6.0.3; Astro 6.4.x; Node.js 22; pnpm 11.x  
**Primary Dependencies**: Astro static site generation, existing public-web components/layouts, `@astrojs/check`, Vitest, JSDOM, Testing Library DOM, Prettier with Astro plugin  
**Storage**: Static repository-owned TypeScript data files only; no database, CMS, server session, or runtime persistence  
**Testing**: `pnpm --dir src/web/apps/public-web test`, `check`, `lint`, `build`; Vitest/JSDOM route tests; content inventory validation; generated route/link/metadata/structured-data validation  
**Target Platform**: Static public website under `src/web/apps/public-web`, deployed as existing public-web static output  
**Project Type**: Static Astro web application with generated content routes and repository-owned data contracts  
**Performance Goals**: Preserve static-first output; generated comparison pages should add no client-side runtime dependency and should keep existing public-web build/test performance practical for app-local validation  
**Constraints**: Remove existing one-off comparison page first; no legacy comparison redirect or alias; publish only when `/compare`, all 10 individual comparison pages, and all 6 supporting pages validate together; no hardcoded page-specific comparison claims; no unsupported Skedular or competitor claims; American spelling and grammar  
**Scale/Scope**: One comparison hub, 10 individual competitor pages, 6 supporting SEO pages, shared comparison components, normalized feature matrix, content inventory, metadata, FAQ schema, structured data, sitemap/public page integration, and validation coverage

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature does not touch `api-definitions/`, generated GraphQL schemas, Relay artifacts, OpenAPI clients, protobufs, or backend contracts. No generator is required.
- [x] **II. Domain Boundaries** — This feature is static public website work only. It reads existing specs/help/product content as evidence but does not cross backend domain ownership, persistence, service, workflow, or event boundaries.
- [x] **III. Testing** — Web/static content changes require app-local Vitest/JSDOM route tests, Astro check, lint, build validation, content inventory validation, link/canonical/metadata checks, and structured-data checks. Backend unit/integration tests are not required because no backend code or persistence boundary is touched.
- [x] **IV. Frontend** — This feature includes web changes in the existing Astro public website, not the Next.js/Relay product apps. Relay colocation and typography-wrapper gates are not applicable. User-facing copy must use American spelling and grammar.
- [x] **V. Pattern Consistency** — Use the existing `src/web/apps/public-web` Astro static-site pattern, content data files, `SiteLayout`, SEO helpers, structured-data component, and Vitest build validation. The new comparison dataset is a larger version of the current `comparisonPages` pattern, not a new framework.
- [x] **VI. Logging** — No runtime business workflow is added. Observability scope is build/test-time diagnostics: invalid comparison data, missing evidence/review status, duplicate slugs/canonical paths, incomplete required page set, unpublished page references, structured-data mismatches, and route generation failures must produce actionable validation output without sensitive data.

## Project Structure

### Documentation (this feature)

```text
specs/029-competitor-comparison-hub/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── comparison-content-contract.md
│   └── public-route-contract.md
└── tasks.md                  # Created by /speckit-tasks, not this command
```

### Source Code (repository root)

```text
src/web/apps/public-web/
├── package.json
├── src/
│   ├── components/
│   │   ├── FeatureComparisonSections.astro        # replace/expand for shared comparison sections
│   │   └── StructuredData.astro                   # extend only if needed for FAQ/Breadcrumb graph support
│   ├── data/
│   │   ├── comparison-pages.ts                    # replace one-off comparison records with generated targets
│   │   ├── comparison/                            # planned shared comparison dataset and feature matrix
│   │   ├── content-inventory.ts                   # include hub, comparison pages, supporting pages
│   │   ├── seo.ts                                 # existing sitemap/robots/llms inputs
│   │   └── content-types.ts                       # extend static content types for comparison entities
│   ├── layouts/
│   │   └── SiteLayout.astro
│   └── pages/
│       └── compare/
│           ├── index.astro                        # planned comparison hub
│           └── [slug].astro                       # generated comparison and supporting pages
└── tests/
    └── public-site-content.test.ts                # extend generated route/link/content validation
```

**Structure Decision**: Implement inside the existing `src/web/apps/public-web` Astro app. Keep all comparison content, evidence state, feature matrix, page targets, FAQs, and structured-data inputs in repository-owned static data files. Do not add backend services, GraphQL operations, Relay artifacts, OpenAPI definitions, CMS integrations, or runtime persistence.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical context items are resolved; no open clarification markers remain.

## Phase 1: Design & Contracts

Design artifacts:

- [data-model.md](./data-model.md) - comparison data entities, feature matrix, page targets, evidence state, lifecycle rules, and validation constraints.
- [contracts/comparison-content-contract.md](./contracts/comparison-content-contract.md) - shared comparison dataset, feature matrix, evidence, FAQ, and structured-data content contract.
- [contracts/public-route-contract.md](./contracts/public-route-contract.md) - public route, hub, metadata, canonical, sitemap, link, and no-legacy-redirect contract.
- [quickstart.md](./quickstart.md) - validation guide for app-local test, check, lint, build, and manual content review scenarios.

## Post-Design Constitution Check

- [x] **I. Contract-First** — Design artifacts confirm no API/generated contract surfaces are changed.
- [x] **II. Domain Boundaries** — Design stays within static public-web content and references current specs/help/source files as evidence only.
- [x] **III. Testing** — Quickstart defines app-local validation for generated routes, content inventory, metadata, canonical paths, structured data, sitemap, no legacy alias/redirect, and page completeness.
- [x] **IV. Frontend** — Design uses Astro public-web patterns, shared static components, and American English. Next.js/Relay-specific gates remain not applicable.
- [x] **V. Pattern Consistency** — Design expands the existing static data and dynamic route pattern instead of introducing a new app, CMS, or runtime content layer.
- [x] **VI. Logging** — Design treats static-site observability as build/test diagnostics and review inventories for invalid data, missing evidence, incomplete publication, and route/metadata failures.
