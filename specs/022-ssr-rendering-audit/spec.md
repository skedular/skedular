# Feature Specification: Web App Performance Optimization Audit

**Feature Branch**: `022-ssr-rendering-audit`
**Created**: 2026-06-03
**Status**: Draft
**Input**: User description: "Please start reviewing all three web apps. I need help identifying which parts can be generated on the server side to improve build-time compilation and server-side rendering, things that boost performance, since most rendering currently happens on the client side. I want to determine which parts of these web apps can be improved. Write me a spec that we can use to research the entire web application and identify all optimization opportunities to achieve better performance for our web app."

> **Scope clarification (2026-06-03)**: The audit covers every page route and every component file across all three apps (`webapp`, `webapp-teams`, `webapp-spaces`) — including page-level routes, shared/reusable components, layouts, feature panels, and every file under `src/components/`, `src/rootPages/`, and the shared packages (`@skedular/ui`, `@skedular/shared`). The audit investigates every technique that can make pages load faster: Server Components, static/ISR generation, lazy loading, code splitting, bundle size reduction, image optimization, font loading, and removal of unnecessary client-side work. No optimization category and no component depth level is out of scope.
>
> **Two-phase structure (2026-06-03)**: The feature runs in two sequential phases. **Phase 1 (Audit)** classifies every route and component, measures numeric baselines with bundle analysis and Lighthouse, and produces a prioritized recommendation list gated by requester review (SC-005). **Phase 2 (Implementation)** acts on all approved findings and delivers every actionable optimization as production code changes, verified against the Phase 1 baselines.

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Complete Performance Audit of Every Page and Component (Priority: P1)

A developer reviews a comprehensive audit report that maps every route and significant component across `webapp`, `webapp-teams`, and `webapp-spaces` and identifies, for each surface, every applicable technique that would make it load faster:

- **Server Components conversion** — currently `'use client'` but has no interactivity; move rendering to the server
- **Static Generation / ISR** — data is stable enough at build time or can be revalidated on a schedule
- **Lazy loading / code splitting** — component or dependency is heavy and not needed on first paint
- **Bundle reduction** — unused or oversized imports (e.g., MUI, map libraries, analytics) inflating the JS bundle
- **Image optimization** — images not using Next.js `<Image>` or missing `priority` / `sizes` hints
- **Font loading optimization** — fonts blocking first contentful paint or using suboptimal loading strategy
- **Must remain client-rendered as-is** — requires auth state, runtime user data, or interactive behavior with no feasible optimization

The developer uses the audit findings to prioritize the highest-impact changes across all three apps.

**Why this priority**: This is the research foundation. Without a complete inventory of current performance patterns across every page and component, all subsequent optimization work lacks a baseline and prioritization signal.

**Independent Test**: Can be independently validated by checking the audit report contains an entry for every page route and every component file in all three apps with at least one optimization classification or an explicit "no optimization applicable" note.

**Acceptance Scenarios**:

1. **Given** the audit is complete, **When** a developer opens the audit report, **Then** every Next.js page route across all three apps is listed with its current performance profile and all applicable optimization recommendations.
2. **Given** a page is a Server Component conversion candidate, **When** the developer reads the recommendation, **Then** it states what data the page currently fetches client-side and whether it can be fetched server-side at request time.
3. **Given** a page is a Static Generation / ISR candidate, **When** the developer reads the recommendation, **Then** it states how often the data changes and which Next.js caching strategy is most appropriate.
4. **Given** a page or component has a bundle-size or lazy-loading opportunity, **When** the developer reads the recommendation, **Then** it identifies the specific import(s) that should be split or deferred and provides a specific estimated bundle size reduction in KB.
5. **Given** a page is classified as "no optimization applicable", **When** the developer reads the rationale, **Then** it identifies the specific constraint (auth state, browser API, mandatory interactivity) that makes further optimization infeasible.

---

### User Story 2 - Relay and GraphQL Data-Fetching Pattern Review (Priority: P1)

A developer understands which Relay query roots and fragments in each app are executed client-side on every navigation and which could instead be prefetched or initiated server-side using Next.js server data-fetching patterns to eliminate the client-side loading waterfall.

**Why this priority**: The apps use Relay extensively. The majority of visible loading states today are caused by Relay fetching data on the client after hydration rather than starting the fetch on the server. Identifying which queries can move server-side directly addresses the most impactful latency source.

**Independent Test**: Validated by producing a list of Relay query roots per route and classifying each as "can preload server-side", "must stay client-side" (requires auth token only available after hydration), or "uses Relay preloaded query already".

**Acceptance Scenarios**:

1. **Given** the audit examines a route with a top-level Relay query, **When** the route does not depend on client-side auth state to form the request, **Then** the audit marks that query as a candidate for server-side prefetch.
2. **Given** the audit examines a route where the Relay query requires a user token obtained only after WorkOS AuthKit hydration, **Then** the audit documents why it must remain client-side and suggests whether partial prefetch (unauthenticated public data) is possible.
3. **Given** a Relay fragment is deeply nested inside a client subtree, **When** the parent route could become a Server Component, **Then** the audit identifies the fragment boundary and recommends a split: a Server Component outer shell initiating the fetch plus a minimal Client Component for interactivity.

---

### User Story 3 - Static, Build-Time, and Lazy-Load Opportunity Identification (Priority: P2)

A developer identifies all UI surfaces where data is stable enough at build time (or changes infrequently) to benefit from Static Site Generation or ISR, as well as all heavy components or third-party dependencies that can be lazy-loaded or code-split to reduce the initial JavaScript payload delivered to the browser.

**Why this priority**: Stable content such as public marketplace landing pages and product listing pages are currently fully client-rendered, meaning every visitor triggers a client-side data fetch and downloads the full JS bundle. Converting to static/ISR and deferring heavy components would reduce both per-request latency and bundle download time.

**Independent Test**: Validated by a list of routes identified as ISR/static candidates, each annotated with expected data-change frequency and recommended revalidation period.

**Acceptance Scenarios**:

1. **Given** a public-facing marketplace product page renders the same content for all unauthenticated visitors, **When** the audit reviews it, **Then** the audit classifies it as a static or ISR candidate and recommends an appropriate revalidation interval.
2. **Given** a route renders per-user or per-session data alongside public content, **When** the audit reviews it, **Then** the audit recommends splitting the route into a statically generated shell with a client-rendered personalization layer.
3. **Given** the audit identifies a static generation candidate, **When** the developer reads the recommendation, **Then** it includes an estimate of the build-time data volume (how many pages would be pre-rendered) to help the developer assess build duration impact.
4. **Given** a page imports a heavy library (e.g., a map renderer, a rich-text editor, a charting library) that is not needed on first paint, **When** the audit reviews it, **Then** the audit recommends dynamic import with `next/dynamic` or `React.lazy` and provides a specific estimated bundle size saving in KB.

---

### User Story 4 - Component-Level Client Boundary Minimization (Priority: P2)

A developer receives a component-level analysis that identifies where `'use client'` directives can be pushed deeper into the component tree, allowing outer layout and structural components to become Server Components even when leaf-level interactive elements must stay client-rendered.

**Why this priority**: The current pattern wraps entire page trees in `'use client'`, preventing Next.js from streaming any server-rendered HTML for those routes. Pushing client boundaries to leaf components restores streaming and reduces the JavaScript bundle sent to the browser.

**Independent Test**: Validated by identifying at least five specific component files where the `'use client'` boundary can be narrowed, along with a description of what would move to the server.

**Acceptance Scenarios**:

1. **Given** a page component uses `'use client'` only because it calls a hook for one small interactive element, **When** the audit reviews it, **Then** the recommendation shows how to extract the interactive element into its own small Client Component while converting the page wrapper to a Server Component.
2. **Given** a layout component is currently marked `'use client'` to pass theme or context down the tree, **When** the audit reviews it, **Then** the recommendation identifies whether the context can be moved to a dedicated provider component so the layout itself becomes server-rendered.
3. **Given** the audit proposes a `'use client'` boundary change, **Then** the recommendation notes any Relay fragment colocation constraints that must be preserved when splitting the component.

---

### User Story 5 - Shared Package (`@skedular/ui`, `@skedular/shared`) Rendering Compatibility Review (Priority: P3)

A developer understands which exports from `@skedular/ui` and `@skedular/shared` are compatible with Server Components and which require a client context (for example, hooks, context consumers, or browser APIs), so that design system and shared utility usage does not accidentally force parent components to become client-rendered.

**Why this priority**: If shared package exports are inappropriately imported in Server Components they will cause runtime errors or force unnecessary client boundaries. An inventory of SSR-compatible vs. client-only exports guides safe adoption.

**Independent Test**: Validated by a classification table of the major exports from both packages, categorized as "server-safe" (pure functions, React Server Component–compatible), "client-only" (hooks, context, browser APIs), or "universal" (works in both environments with appropriate Next.js patterns).

**Acceptance Scenarios**:

1. **Given** a shared export is a pure render component with no hooks or browser dependencies, **When** the audit reviews it, **Then** it is classified as server-safe and noted as safe to import in Server Components.
2. **Given** a shared export uses `useContext`, `useState`, or a browser API, **When** the audit reviews it, **Then** it is classified as client-only and the audit notes that any Server Component importing it must be converted to a Client Component or the import must be deferred behind a dynamic import.
3. **Given** the audit identifies a widely-used shared export as client-only, **Then** the audit flags all Server Component candidates in the apps that import it so developers can assess the cascading impact.

---

### Edge Cases

- What happens with routes that serve both authenticated and unauthenticated users from the same URL (e.g., custom domain subdomains)? The audit must classify the rendering strategy for each user-state branch separately.
- How does the audit handle routes with query-string-driven rendering (e.g., filtered lists)? It must note whether dynamic query parameters prevent static generation or can be handled via on-demand ISR.
- What happens when a component is used in both a client-rendered and a potentially server-rendered context? The audit must identify dual-use components and recommend if they need to be split.
- How does the audit address the `AuthenticatedRelayProvider` wrapping the entire app in a client boundary? This must be called out as the primary structural blocker for broad SSR adoption.
- What if converting a page to a Server Component breaks existing Relay fragment colocation? The audit must surface these cases and recommend the appropriate Relay server preloading pattern.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The audit MUST enumerate every Next.js page route in `webapp`, `webapp-teams`, and `webapp-spaces`, including dynamic routes and catch-all segments, AND every component file under `src/components/`, `src/rootPages/`, shared layout files, and exports from `@skedular/ui` and `@skedular/shared`. No component depth level is excluded.
- **FR-002**: The audit MUST classify each route across all applicable optimization dimensions: Server Components conversion, Static Generation / ISR, lazy loading / code splitting, bundle reduction, image optimization, and font loading.
- **FR-003**: For every Server Component or Static Generation candidate, the audit MUST identify the specific data dependencies (GraphQL queries, REST calls, env variables) that would move server-side.
- **FR-004**: For every route or component assessed as having no applicable optimization, the audit MUST document the specific blocker (auth state, browser API, third-party client-only SDK, mandatory interactivity).
- **FR-005**: The audit MUST identify where `'use client'` boundaries can be narrowed by extracting client-only behavior into leaf components, enabling outer layout and structural components to become Server Components.
- **FR-006**: The audit MUST review top-level Relay query roots per route and classify each as server-prefetchable, must-stay-client-side, or already-preloaded.
- **FR-007**: The audit MUST identify public-facing routes (unauthenticated access) that are candidates for Static Site Generation or Incremental Static Regeneration, and recommend a revalidation interval for each.
- **FR-008**: The audit MUST identify every heavy or lazily-loadable import across all three apps (e.g., map libraries, charting, rich-text editors, analytics SDKs) and recommend which can be deferred with `next/dynamic` or `React.lazy` to reduce initial bundle size. Each finding MUST include a specific estimated bundle size reduction in KB derived from bundle analysis tooling.
- **FR-009**: The audit MUST review all image usage across the three apps and identify instances not using Next.js `<Image>`, missing `priority` hints on above-the-fold images, or missing responsive `sizes` attributes. Each finding MUST include a specific estimated LCP or CLS impact derived from the Lighthouse baseline captured in the Foundational phase (T010b).
- **FR-010**: The audit MUST review font loading strategy across all three apps and identify any fonts that block first contentful paint or use a suboptimal loading approach (e.g., missing `display: swap`, unnecessary font variants). Each finding MUST include a specific estimated FCP impact derived from the Lighthouse baseline captured in the Foundational phase (T010b).
- **FR-011**: The audit MUST produce a classification table for the major exports of `@skedular/ui` and `@skedular/shared` as either server-safe, client-only, or universal.
- **FR-012**: The audit MUST identify routes or components in `webapp-teams` and `webapp-spaces` that differ in optimization constraints from their `webapp` equivalents, noting product-specific limitations (e.g., MS Teams embedding, iframe restrictions).
- **FR-013**: The audit output MUST be structured as a written report artifact stored in the feature spec directory, with a per-route summary table and a prioritized list of recommended changes. Every recommendation MUST include a specific estimated performance metric (e.g., KB saved, LCP improvement in ms, server requests eliminated) derived from bundle analysis tooling or equivalent measurement. Qualitative-only estimates are not sufficient.
- **FR-014**: The audit MUST note which recommended changes require Relay artifact regeneration and which require backend GraphQL schema changes, so that generation pipeline impact is understood upfront.
- **FR-015**: The audit MUST assess the impact of the `AuthenticatedRelayProvider` and `AuthKitProvider` wrapping strategy on the ability to server-render route content, and propose an architectural direction for enabling per-route server rendering without removing the global auth context. _(See also SC-003 — the acceptance gate for this deliverable.)_
- **FR-016**: The audit MUST run bundle analysis tooling (e.g., `@next/bundle-analyzer` or equivalent) against all three apps as part of the research process, and MUST use the resulting data to support the numeric estimates cited in recommendations.
- **FR-017**: All optimization opportunities classified as actionable (i.e., not explicitly marked "no optimization applicable") in the Phase 1 audit output MUST be implemented as production code changes in Phase 2 of this feature. No actionable recommendation may be deferred to a follow-on feature without explicit requester approval at the SC-005 review gate.
- **FR-018**: Each Phase 2 implementation change MUST be validated by re-running the relevant measurement (bundle analysis or Lighthouse audit) and confirming a measurable improvement over the Phase 1 baseline. Results MUST be recorded in `specs/022-ssr-rendering-audit/audit/post-implementation-results.md`.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: Phase 1 (audit) introduces no runtime logging changes. Phase 2 (implementation) MUST follow existing structured logging conventions for any new server-side data-fetching paths, ISR revalidation handlers, SSR error boundaries, or streaming render paths introduced. Runtime logging additions required by Phase 2 changes are scoped per-task.
- **LOG-002**: If any exploratory code samples or proof-of-concept conversions are written during the audit, they MUST follow existing structured logging conventions and MUST NOT introduce silent operations or swallowed errors.
- **LOG-003**: The audit report MUST note which recommended changes, once implemented, would require new observability instrumentation (e.g., server-side data fetch durations, SSR render errors surfaced to the user vs. silently swallowed).
- **LOG-004**: Any proof-of-concept code produced during audit research MUST include correlation context comments explaining where runtime identifiers would be threaded.

### Key Entities _(include if feature involves data)_

- **Route Audit Entry**: Represents a single Next.js page route across one of the three apps. Key attributes: app name, route path, current performance profile, applicable optimization categories (multi-select), data dependencies, blocker (if any), estimated effort.
- **Component Audit Entry**: Represents a single component file anywhere in the three apps or shared packages. Key attributes: file path, app or package scope, current rendering environment (client/server/both), applicable optimization categories, blocker (if any).
- **Relay Query Classification**: Maps a Relay query root or preloaded query to a route. Attributes: query name, current execution environment (client), recommended execution environment, auth dependency flag.
- **Client Boundary Assessment**: Records a specific component file where a `'use client'` boundary can be narrowed. Attributes: component path, current scope of client boundary, proposed narrowed boundary, impacted Relay fragments.
- **Lazy-Load Candidate**: Records a heavy import or component that can be deferred. Attributes: component/import path, affected routes, estimated bundle size impact, recommended deferral mechanism.
- **Asset Optimization Finding**: Records an image or font asset with a suboptimal loading pattern. Attributes: asset path or usage location, current pattern, recommended pattern, estimated LCP or CLS impact.
- **Shared Package Export Classification**: Records an export from `@skedular/ui` or `@skedular/shared`. Attributes: export name, package, classification (server-safe / client-only / universal), reason, affected routes.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: 100% of Next.js page routes across all three apps (`webapp`, `webapp-teams`, `webapp-spaces`) are catalogued in the audit report, each with at least one optimization finding or an explicit "no further optimization applicable" note.
- **SC-002**: At least one concrete, actionable recommendation exists in each of the following categories: Server Components conversion, Static Generation / ISR, lazy loading / code splitting, bundle size reduction, and asset (image/font) optimization.
- **SC-003**: The `AuthenticatedRelayProvider` and global client-boundary architectural constraint is clearly explained with at least one proposed strategy for enabling per-route server rendering alongside it. _(Implements FR-015.)_
- **SC-004**: All major exports from `@skedular/ui` and `@skedular/shared` are classified as server-safe, client-only, or universal so that developers can make safe import decisions when converting components.
- **SC-005**: The audit report is reviewed by the requester and confirmed as actionable — meaning they agree it provides sufficient detail to begin implementation planning without returning to re-research the same areas. This review is a required gate before the audit is considered complete.
- **SC-006**: The prioritized recommendation list identifies at least the top 5 highest-impact optimization opportunities ranked by estimated performance impact, where each entry cites a specific measured or estimated metric (e.g., KB removed from initial bundle, LCP improvement in ms, server round-trips eliminated) derived from bundle analysis tooling or equivalent measurement.
- **SC-007**: All actionable optimizations identified in Phase 1 and approved at the SC-005 review gate are implemented, tested with Vitest/React Testing Library, and verified by a post-implementation bundle analysis and Lighthouse run showing measurable improvement over the Phase 1 baselines recorded in T010 and T010b.

## Assumptions

- The audit covers the three primary product apps: `webapp`, `webapp-teams`, and `webapp-spaces`, examining every page route and every component file at all depths — pages, layouts, feature panels, shared/reusable components, and shared package exports. The help apps (`webapp-help`, `webapp-spaces-help`, `webapp-teams-help`) are out of scope as they are static documentation sites already handled in a prior feature (021).
- The optimization scope is intentionally broad: Server Components, Static Generation / ISR, lazy loading, code splitting, bundle size reduction, image optimization, and font loading are all in scope. No performance optimization category is excluded by default.
- The current WorkOS AuthKit integration (`@workos-inc/authkit-nextjs`) constrains which routes can easily move to server-rendered patterns; the audit will document these constraints but will not redesign the auth architecture.
- The feature runs in two sequential phases. **Phase 1** is the audit and research deliverable: it measures the current state, classifies every route and component, and produces a prioritized recommendation list. **Phase 2** implements all actionable optimizations identified by Phase 1 as production code changes — these are real changes to the three app code bases and shared packages. Proof-of-concept code produced in Phase 1 may be stored under `specs/022-ssr-rendering-audit/` for reference and may later be promoted to Phase 2 implementation tasks.
- Relay 21 is the version in use. The audit considers Relay's server-side data-fetching capabilities (preloaded queries, server-side entrypoints) within the Relay 21 API surface.
- The audit does not cover backend (C# .NET 10) performance changes. All improvements are scoped to the Next.js frontend layer.
- Next.js App Router (version 16.2.6) capabilities and constraints are the reference target for all rendering and loading recommendations.
- All three apps share the `@skedular/ui` and `@skedular/shared` packages. A Server Component compatibility finding or a bundle-size finding in those packages applies uniformly across all three apps.
- Performance measurement baselines (current Core Web Vitals, bundle sizes, time to first byte) MUST be gathered using bundle analysis tooling (e.g., `@next/bundle-analyzer`) as a required step of the audit. These baselines inform all numeric metric estimates cited in recommendations.

## Clarifications

### Session 2026-06-03

- Q: Should the audit scope be narrow (SSR/Server Components only) or broad (all techniques that make pages load faster)? → A: Broad — the audit covers every page and component in all three apps and investigates every applicable technique: Server Components, static/ISR generation, lazy loading, code splitting, bundle size reduction, image optimization, font loading, and removal of unnecessary client-side work. No optimization category is excluded.
- Q: What depth of component coverage is in scope — pages only, pages plus shared/reusable, or all components? → A: Full depth — every page route and every component file across all three apps and shared packages (`src/components/`, `src/rootPages/`, layouts, `@skedular/ui`, `@skedular/shared`). No component depth level is excluded.
- Q: What is the expected treatment of proof-of-concept code? → A: Optional reference only — PoC code may be written if it validates a specific recommendation and MUST be stored under `specs/022-ssr-rendering-audit/`. It is never shipped.
- Q: How should performance impact be estimated for ranking recommendations? → A: Precise numeric estimates — every recommendation must cite a specific measured or estimated metric (KB saved, LCP delta in ms, requests eliminated) derived from bundle analysis tooling or equivalent measurement run as part of the audit.
- Q: Who reviews and approves the audit report before the feature is considered done? → A: The requester personally reviews and confirms the report is actionable. This is a required gate before the audit is complete.
