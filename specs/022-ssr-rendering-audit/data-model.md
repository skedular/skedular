# Data Model: Web App Performance Optimization Audit (022)

**Phase**: 1 — Design  
**Feature**: `022-ssr-rendering-audit`  
**Produced by**: `/speckit.plan`  
**Date**: 2026-06-03

---

## Overview

This audit is a research deliverable, not a persistent data model. The entities below define the **classification schema** used to structure audit findings across routes, components, and shared packages. They are documented as structured types that audit output documents (markdown tables or JSON reports) should conform to.

---

## Entities

### RouteAuditEntry

Represents the audit classification for a single Next.js page route.

| Field                         | Type                                            | Description                                                           |
| ----------------------------- | ----------------------------------------------- | --------------------------------------------------------------------- |
| `app`                         | `'webapp' \| 'webapp-teams' \| 'webapp-spaces'` | Which app the route belongs to                                        |
| `routePath`                   | `string`                                        | Next.js App Router path (e.g., `/marketplace/organizations/[domain]`) |
| `filePath`                    | `string`                                        | Repo-relative path to `page.tsx`                                      |
| `currentRendering`            | `'client' \| 'server' \| 'static' \| 'isr'`     | How the route currently renders                                       |
| `isClientDirective`           | `boolean`                                       | Whether `'use client'` is present at the top of the file              |
| `relayQueries`                | `RelayQueryClassification[]`                    | Relay queries used by this route                                      |
| `optimizations`               | `OptimizationRecommendation[]`                  | All applicable recommendations                                        |
| `canConvertToServerComponent` | `boolean`                                       | Whether the route shell can become a Server Component                 |
| `isrRevalidateSeconds`        | `number \| null`                                | Recommended ISR revalidation period (null if not applicable)          |
| `staticGenerationEstimate`    | `string \| null`                                | Build-time page count estimate (e.g., "~200 org domains")             |
| `rationale`                   | `string`                                        | Short explanation of the primary classification                       |
| `priority`                    | `'P0' \| 'P1' \| 'P2' \| 'P3'`                  | Implementation priority (P0 = highest impact)                         |

---

### ComponentAuditEntry

Represents the audit classification for a single component file.

| Field                      | Type                                                                                    | Description                                                                    |
| -------------------------- | --------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------ |
| `app`                      | `'webapp' \| 'webapp-teams' \| 'webapp-spaces' \| '@skedular/ui' \| '@skedular/shared'` | Owning app or package                                                          |
| `filePath`                 | `string`                                                                                | Repo-relative path to the component file                                       |
| `isClientDirective`        | `boolean`                                                                               | Whether `'use client'` is declared                                             |
| `reasonForClientDirective` | `string \| null`                                                                        | Why `'use client'` exists (e.g., "uses useAuth()", "uses useState for toggle") |
| `canNarrowBoundary`        | `boolean`                                                                               | Whether the `'use client'` boundary can be pushed to a child component         |
| `clientBoundaryAssessment` | `ClientBoundaryAssessment \| null`                                                      | Detailed boundary narrowing analysis                                           |
| `lazyLoadCandidates`       | `LazyLoadCandidate[]`                                                                   | Heavy imports that can be deferred                                             |
| `optimizations`            | `OptimizationRecommendation[]`                                                          | All applicable recommendations                                                 |
| `priority`                 | `'P0' \| 'P1' \| 'P2' \| 'P3'`                                                          | Implementation priority                                                        |

---

### RelayQueryClassification

Classifies a single Relay query root used by a route.

| Field                           | Type                                                                | Description                                                                 |
| ------------------------------- | ------------------------------------------------------------------- | --------------------------------------------------------------------------- |
| `queryName`                     | `string`                                                            | Name of the Relay query (e.g., `organizationStoreFrontRootShell_rootQuery`) |
| `fragmentRoot`                  | `string`                                                            | Top-level fragment or component that owns the query                         |
| `requiresAuthToken`             | `boolean`                                                           | Whether the query requires a user auth token                                |
| `requiresMsTeamsToken`          | `boolean`                                                           | Whether the query requires a MS Teams token                                 |
| `isPublicData`                  | `boolean`                                                           | Whether the query fetches only public/unauthenticated data                  |
| `serverPrefetchFeasibility`     | `'can-prefetch' \| 'public-partial-prefetch' \| 'must-stay-client'` | SSR feasibility classification                                              |
| `rationale`                     | `string`                                                            | Explanation of the feasibility decision                                     |
| `estimatedWaterfallReductionMs` | `number \| null`                                                    | Estimated latency saving from server prefetch (null if not measured)        |

---

### ClientBoundaryAssessment

Detailed analysis of a `'use client'` boundary and whether it can be narrowed.

| Field                           | Type                                                                                                                            | Description                                                                |
| ------------------------------- | ------------------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- |
| `currentBoundaryFile`           | `string`                                                                                                                        | Component file that currently holds the `'use client'` directive           |
| `clientReasonCategory`          | `'auth-hook' \| 'relay-context' \| 'browser-api' \| 'interactivity' \| 'theme-context' \| 'event-handler' \| 'third-party-sdk'` | Primary reason for client rendering                                        |
| `extractableClientPart`         | `string \| null`                                                                                                                | The specific interactive element that actually needs client context        |
| `serverComponentShell`          | `string \| null`                                                                                                                | Proposed Server Component wrapper that would replace the current component |
| `relayConstraints`              | `string \| null`                                                                                                                | Any Relay fragment colocation constraints that apply                       |
| `estimatedServerComponentLines` | `number \| null`                                                                                                                | Rough size of the extractable server-side portion                          |

---

### LazyLoadCandidate

Identifies a heavy import that can be deferred using `next/dynamic` or `React.lazy`.

| Field                     | Type                             | Description                                                                            |
| ------------------------- | -------------------------------- | -------------------------------------------------------------------------------------- |
| `importPath`              | `string`                         | The import being deferred (e.g., `'react-leaflet'`)                                    |
| `usedInFile`              | `string`                         | Component file containing the import                                                   |
| `mechanism`               | `'next/dynamic' \| 'React.lazy'` | Recommended deferral mechanism                                                         |
| `ssrEnabled`              | `boolean`                        | Whether SSR is enabled for the dynamic import (`ssr: false` for browser-only libs)     |
| `loadingFallback`         | `string \| null`                 | Suggested loading skeleton or spinner                                                  |
| `estimatedBundleSavingKB` | `number`                         | Approximate JS bundle size reduction from splitting this import                        |
| `triggerCondition`        | `string`                         | When the component first renders (e.g., "only when user navigates to location detail") |

---

### AssetOptimizationFinding

Tracks an image or font optimization opportunity.

| Field             | Type                           | Description                                                                                           |
| ----------------- | ------------------------------ | ----------------------------------------------------------------------------------------------------- |
| `type`            | `'image' \| 'font'`            | Asset category                                                                                        |
| `filePath`        | `string`                       | File containing the asset reference                                                                   |
| `finding`         | `string`                       | Description of the issue (e.g., "raw <img> tag", "missing priority prop", "font in TTF format")       |
| `recommendation`  | `string`                       | Specific fix action                                                                                   |
| `estimatedImpact` | `string`                       | Human-readable impact description (e.g., "reduces LCP by removing low-priority fetch for hero image") |
| `priority`        | `'P0' \| 'P1' \| 'P2' \| 'P3'` | Implementation priority                                                                               |

---

### SharedPackageExportClassification

Classifies a single export from `@skedular/ui` or `@skedular/shared` for Server Component compatibility.

| Field                | Type                                          | Description                                                                                            |
| -------------------- | --------------------------------------------- | ------------------------------------------------------------------------------------------------------ |
| `package`            | `'@skedular/ui' \| '@skedular/shared'`        | Package owning the export                                                                              |
| `exportName`         | `string`                                      | Named export (e.g., `BodyIconTypography`, `AuthenticatedRelayProvider`)                                |
| `filePath`           | `string`                                      | Source file of the export                                                                              |
| `ssrCompatibility`   | `'server-safe' \| 'client-only' \| 'unknown'` | Server Component compatibility                                                                         |
| `clientReason`       | `string \| null`                              | Why it requires client context (hooks, context, browser APIs)                                          |
| `hasClientDirective` | `boolean`                                     | Whether the file declares `'use client'`                                                               |
| `recommendation`     | `string \| null`                              | Suggestion for improving SSR compatibility (e.g., "split into server-safe primitive + client wrapper") |

---

### OptimizationRecommendation

A single recommendation attached to a route or component audit entry.

| Field             | Type                          | Description                                            |
| ----------------- | ----------------------------- | ------------------------------------------------------ |
| `category`        | `OptimizationCategory`        | Optimization technique category                        |
| `description`     | `string`                      | Specific recommendation text                           |
| `estimatedImpact` | `string`                      | Quantified or described impact                         |
| `effort`          | `'low' \| 'medium' \| 'high'` | Implementation effort estimate                         |
| `blockers`        | `string[]`                    | Prerequisites or architectural constraints             |
| `relatedFR`       | `string`                      | Spec functional requirement reference (e.g., `FR-001`) |

---

### OptimizationCategory (enum)

```
'server-component-conversion'   — Convert 'use client' component to Server Component
'isr-static-generation'         — Adopt ISR or static generation for stable data
'lazy-load-code-split'          — Defer heavy component with next/dynamic or React.lazy
'relay-server-prefetch'         — Move Relay query fetch to server request time
'image-optimization'            — Use <Image> priority/sizes or replace <img>
'font-optimization'             — Improve font format or loading strategy
'bundle-reduction'              — Remove or tree-shake large dependencies
'client-boundary-narrowing'     — Push 'use client' deeper in component tree
'suspense-streaming'            — Add Suspense boundaries to enable HTML streaming
'no-optimization-applicable'    — Explicit classification: no feasible optimization
```

---

## Audit Output Structure

The completed audit deliverable produces the following files:

```text
specs/022-ssr-rendering-audit/
├── audit/
│   ├── webapp-routes.md          # RouteAuditEntry table for webapp
│   ├── webapp-teams-routes.md    # RouteAuditEntry table for webapp-teams
│   ├── webapp-spaces-routes.md   # RouteAuditEntry table for webapp-spaces
│   ├── shared-packages.md        # SharedPackageExportClassification tables
│   ├── lazy-load-candidates.md   # LazyLoadCandidate table (all apps)
│   └── asset-findings.md         # AssetOptimizationFinding table (all apps)
```

These files are produced by `/speckit.tasks` implementation tasks, not by `/speckit.plan`.
