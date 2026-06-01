# Implementation Plan: Help Webapps Documentation

**Branch**: `021-help-webapps-docs` | **Date**: 2026-06-01 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/021-help-webapps-docs/spec.md`

## Summary

Create a full first-version help center for the three existing Skedular help apps: Customer help, Teams help, and Spaces help. The work will inventory the current specs, route trees, UI pages, and help shells, then replace the generic duplicated help content with app-specific topic pages and step-by-step guides. Every route, detail page, form, status, and major component state found in the source inventory must be covered by help content, an out-of-scope decision, or a content gap. Screenshot placeholders will be included for later capture, but screenshot capture itself is not required in this slice.

## Technical Context

**Language/Version**: TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router, MDX content  
**Primary Dependencies**: Nextra 4.6.1, `nextra-theme-docs`, React, Next.js, existing help app package scripts  
**Storage**: N/A - static documentation content in repository files  
**Testing**: Help app lint/build scripts; manual review against source inventory; no Relay or API generation expected  
**Target Platform**: Web help apps under `src/web/apps/*-help`  
**Project Type**: Static documentation web apps  
**Performance Goals**: Documentation pages should build successfully and avoid new client-side data fetching or runtime product queries  
**Constraints**: Public help content must avoid sensitive customer, payment, security, integration, and internal operator details; use American spelling; include screenshot placeholders instead of final screenshots  
**Scale/Scope**: Three help apps; every reviewed route, detail page, form, status, and major component state in Customer, Teams, and Spaces must be mapped to help content, an out-of-scope decision, or a content gap

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** - This feature does not touch `api-definitions/`, generated GraphQL, generated Relay artifacts, OpenAPI clients, protobufs, or generated backend surfaces. No generator is required.
- [x] **II. Domain Boundaries** - This feature is documentation-only and does not cross backend domain ownership, persistence, service, event, or workflow boundaries.
- [x] **III. Testing** - Static help content changes require lint/build verification for the affected help apps plus source-inventory review. Unit or integration tests are not planned unless implementation adds executable logic beyond static MDX/navigation metadata.
- [x] **IV. Frontend** - This feature includes web help app changes. It does not change Relay operations or generated artifacts. User-facing copy must use American spelling and grammar. Direct MUI typography wrapper rules are not triggered unless new React components are added.
- [x] **V. Pattern Consistency** - Use the existing Nextra help app structure already present in `webapp-help`, `webapp-teams-help`, and `webapp-spaces-help`. No new framework or documentation engine is introduced.
- [x] **VI. Logging** - No new runtime business workflow is planned because the feature is static public help content. The logging exception is explicit: preserve existing help app platform/build diagnostics, record lint/build verification, and capture documentation branch decisions in source inventory and review notes. If tasks add behavior beyond static docs, they must add or preserve structured diagnostics for that behavior.

## Project Structure

### Documentation (this feature)

```text
specs/021-help-webapps-docs/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── help-content-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/web/apps/webapp-help/
├── src/app/
│   ├── layout.tsx
│   ├── page.mdx
│   └── docs/[[...mdxPath]]/page.jsx
└── src/content/
    ├── _meta.ts
    └── *.mdx

src/web/apps/webapp-teams-help/
├── src/app/
│   ├── layout.tsx
│   ├── page.mdx
│   └── docs/[[...mdxPath]]/page.jsx
└── src/content/
    ├── _meta.ts
    └── *.mdx

src/web/apps/webapp-spaces-help/
├── src/app/
│   ├── layout.tsx
│   ├── page.mdx
│   └── docs/[[...mdxPath]]/page.jsx
└── src/content/
    ├── _meta.ts
    └── *.mdx

Source inventory inputs:
├── specs/009-split-web-products/
├── specs/020-customer-landing-cleanup/
├── src/web/apps/webapp/src/app/
├── src/web/apps/webapp/src/rootPages/
├── src/web/apps/webapp-teams/src/app/
├── src/web/apps/webapp-teams/src/rootPages/
├── src/web/apps/webapp-spaces/src/app/
└── src/web/apps/webapp-spaces/src/rootPages/
```

**Structure Decision**: Implement inside the three existing help app projects using the established Nextra MDX structure. Planning artifacts remain under `specs/021-help-webapps-docs/`. No backend, shared package, generated artifact, or product app workflow changes are planned.

## Complexity Tracking

| Violation | Why Needed | Simpler Alternative Rejected Because |
|-----------|------------|--------------------------------------|
| None | N/A | N/A |

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical context items are resolved; no `NEEDS CLARIFICATION` items remain.

## Phase 1: Design & Contracts

Design artifacts:

- [data-model.md](./data-model.md) - documentation entities, validation rules, and state transitions.
- [contracts/help-content-contract.md](./contracts/help-content-contract.md) - UI/content contract for help pages, guides, screenshot placeholders, content gaps, and public-safety review.
- [quickstart.md](./quickstart.md) - review and verification commands for this feature.

## Post-Design Constitution Check

- [x] **I. Contract-First** - Design artifacts confirm no contracts or generated files are touched.
- [x] **II. Domain Boundaries** - Design artifacts stay within help app content and source-inventory review.
- [x] **III. Testing** - Quickstart defines lint/build checks and manual review against source inventory.
- [x] **IV. Frontend** - Design uses existing help app structure and American English copy.
- [x] **V. Pattern Consistency** - Design preserves Nextra/MDX pattern and avoids new documentation framework choices.
- [x] **VI. Logging** - Static-doc logging scope remains explicit; no runtime business behavior is added by the design. Verification and documentation decisions are recorded in source inventory and review notes.
