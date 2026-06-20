# Implementation Plan: Skedular Documentation Center

**Branch**: `033-documentation-center` | **Date**: 2026-07-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/033-documentation-center/spec.md`

## Summary

Build a static, public Documentation Center inside `src/web/apps/public-web`. Add `/docs` navigation and a scalable content collection for Teams, Spaces, Host, and clearly labeled shared-concept documentation. Render a shared documentation shell with product landing pages, categories, article navigation, related guidance, metadata, breadcrumbs, and structured discovery. Seed the verified feature inventory with useful placeholder articles and fully written Getting Started guides. Extend the public page, sitemap, robots, and AI-readable inventories so published documentation is discoverable, while withdrawn pages use a non-indexable retirement route or verified replacement redirect.

## Technical Context

**Language/Version**: TypeScript 6.0.3, Astro 7.0.7, Markdown content  
**Primary Dependencies**: Astro static routing and built-in content collections; existing `SiteLayout`, `SeoHead`, `StructuredData`, Vitest, JSDOM, axe-core  
**Storage**: Repository-managed Markdown and typed metadata; static site output only  
**Testing**: Vitest, static production build, JSDOM route assertions, existing public-site content/discovery tests, manual keyboard/responsive/color-mode review  
**Target Platform**: Static `public-web` deployment and search-engine/AI crawler consumers  
**Project Type**: Static public website feature  
**Performance Goals**: Documentation stays statically generated; no product-application calls or client-side data fetching; representative pages retain the public-site performance targets documented in `public-web/README.md`  
**Constraints**: Public web app only; American English; no legacy redirect work; only verified product behavior; public-safe payment, identity, accounting, refund, and integration guidance; no generated or API-contract changes  
**Scale/Scope**: `/docs` hub, three product landing pages, seven required categories per product, complete Getting Started guides, and one published useful placeholder/article for each verified initial capability; model supports hundreds of articles and future API, release-note, localization, version, media, and search extensions

## Constitution Check

_GATE: Passed before Phase 0 research and re-checked after Phase 1 design._

- [x] **I. Contract-First** — No `api-definitions/`, generated source, GraphQL, OpenAPI, event, or gRPC surface changes are planned. No generator is required.
- [x] **II. Domain Boundaries** — The feature is static public-web content and presentation only. It reads no domain data and changes no service, event, persistence, payment, or workflow boundary.
- [x] **III. Testing** — Public-web behavior requires Vitest/JSDOM coverage plus the existing static build, check, and lint validation. No persistence or integration tests apply.
- [x] **IV. Frontend** — This is Astro public-web work, not a Relay product-app change. It reuses the existing public layout and uses American English; no generated artifacts or direct MUI typography imports are involved.
- [x] **V. Pattern Consistency** — The design extends the public site's typed page inventory, static route generation, shared SEO layout, sitemap, robots, and LLM outputs. Astro's built-in content collection is justified for hundreds of long-form documentation pages; a large TypeScript object would make authored guides difficult to maintain.
- [x] **VI. Logging** — No runtime business workflow is introduced. Existing build diagnostics remain the operational signal; catalog validation and link/discovery tests provide actionable failures for duplicate paths, missing metadata, invalid evidence, and broken internal links.

## Project Structure

### Documentation (this feature)

```text
specs/033-documentation-center/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── documentation-content-contract.md
└── tasks.md                         # Created by /speckit-tasks
```

### Source Code (repository root)

```text
src/web/apps/public-web/
├── src/
│   ├── content/
│   │   └── docs/                    # Product-scoped and shared-concept Markdown articles and front matter
│   ├── content.config.ts            # Typed docs content collection
│   ├── components/
│   │   ├── DocumentationLayout.astro
│   │   ├── DocumentationNavigation.astro
│   │   ├── DocumentationArticle.astro
│   │   └── DocumentationCardGrid.astro
│   ├── data/
│   │   ├── documentation.ts         # Taxonomy, article registry, evidence and navigation helpers
│   │   ├── content-inventory.ts     # Published docs participate in publicPages
│   │   ├── routes.ts                # `/docs` route family and navigation label
│   │   └── seo.ts                   # Sitemap/LLM metadata derives from the page inventory
│   ├── pages/
│   │   └── docs/
│   │       ├── index.astro
│   │       └── [...slug].astro      # Product/category/article static paths
│   ├── layouts/SiteLayout.astro
│   └── styles/global.css
└── tests/
    ├── documentation-content.test.ts
    └── public-site-content.test.ts
```

**Structure Decision**: Keep all implementation in the static `public-web` app. Use a repository-owned Markdown content collection for authored documentation and a small typed catalog for product taxonomy, evidence, URLs, ordering, and relationships. This preserves the public site's existing static architecture while making a large documentation library maintainable.

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical-context decisions are resolved; no `NEEDS CLARIFICATION` items remain.

## Phase 1: Design & Contracts

Design artifacts:

- [data-model.md](./data-model.md) — documentation entities, validation, publication states, and relationships.
- [documentation-content-contract.md](./contracts/documentation-content-contract.md) — public URL, metadata, navigation, source-evidence, and initial coverage contract.
- [quickstart.md](./quickstart.md) — local validation and review steps.

## Post-Design Constitution Check

- [x] **I. Contract-First** — Design limits changes to static public-web files; no generator or contract work is required.
- [x] **II. Domain Boundaries** — Content receives evidence from code and completed artifacts but does not call or alter domain-owned systems.
- [x] **III. Testing** — The design adds catalog and rendered-page assertions, and retains `check`, `lint`, and static build validation.
- [x] **IV. Frontend** — The shared public layout, responsive styling, accessibility conventions, and American English remain the baseline. The product apps are not changed.
- [x] **V. Pattern Consistency** — The only new organizing primitive is a built-in Astro content collection, selected specifically to prevent a hundreds-page documentation library from becoming an unmaintainable data file.
- [x] **VI. Logging** — Build diagnostics and deterministic content validation cover the static publication path; no runtime business logging is applicable.

## Complexity Tracking

| Decision | Why Needed | Simpler Alternative Rejected Because |
| --- | --- | --- |
| Built-in Markdown content collection | Guides and future versions/locales/media require independently authored, typed pages. | A single TypeScript data file would become difficult to review and safely extend across hundreds of articles. |
