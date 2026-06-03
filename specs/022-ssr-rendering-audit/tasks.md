# Tasks: Web App Performance Optimization Audit + Implementation

**Input**: Design documents from `specs/022-ssr-rendering-audit/`  
**Prerequisites**: plan.md ✅, spec.md ✅, research.md ✅, data-model.md ✅, quickstart.md ✅  
**Tests**: Phase 1 (audit) tasks produce markdown deliverables — no test tasks. Phase 2 (implementation) tasks require Vitest/React Testing Library tests per Constitution Gate III.  
**Scope**: `webapp` (28 routes, 297+ components), `webapp-teams` (~55 routes, 212+ components), `webapp-spaces` (~60 routes, 239+ components), `@skedular/ui` (44 tsx), `@skedular/shared` (16 tsx)

> **Two-phase structure**: This file covers **Phase 1 (Audit)** tasks T001–T039 and **Phase 2 (Implementation)** tasks T040–T077. Phase 2 implements the top-priority actionable findings from the audit: P0 barrel contamination fix (T040–T045), P2 font format conversion (T046–T054), P2 `node-ipinfo` API route migration (T055–T058), and P2 defensive `'use client'` directives (T059–T072), followed by Phase 2 verification (T073–T077). The `AuthKitProvider` route group scoping finding (audit Recommendation #5) is excluded from Phase 2 — deferred pending stakeholder review.

## Format: `[ID] [P?] [Story?] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: User story label (US1–US5)
- All tasks produce markdown artifacts in `specs/022-ssr-rendering-audit/audit/`

---

## Phase 1: Setup (Bundle Analysis Tooling)

**Purpose**: Install and configure `@next/bundle-analyzer` in all three apps — required for all numeric KB estimates (FR-016, SC-006).

- [x] T001 Install `@next/bundle-analyzer` as dev dependency in `src/web/apps/webapp/package.json` via `pnpm --filter webapp add -D @next/bundle-analyzer`
- [x] T002 [P] Install `@next/bundle-analyzer` as dev dependency in `src/web/apps/webapp-teams/package.json` via `pnpm --filter webapp-teams add -D @next/bundle-analyzer`
- [x] T003 [P] Install `@next/bundle-analyzer` as dev dependency in `src/web/apps/webapp-spaces/package.json` via `pnpm --filter webapp-spaces add -D @next/bundle-analyzer`
- [x] T004 Wrap `src/web/apps/webapp/next.config.ts` with `withBundleAnalyzer({ enabled: process.env.ANALYZE === 'true' })` per `specs/022-ssr-rendering-audit/quickstart.md`
- [x] T005 [P] Wrap `src/web/apps/webapp-teams/next.config.ts` with `withBundleAnalyzer({ enabled: process.env.ANALYZE === 'true' })`
- [x] T006 [P] Wrap `src/web/apps/webapp-spaces/next.config.ts` with `withBundleAnalyzer({ enabled: process.env.ANALYZE === 'true' })`

**Checkpoint**: Bundle analyzer configured in all three apps — builds can now produce treemap artifacts.

---

## Phase 2: Foundational (Baseline Measurements)

**Purpose**: Run bundle analysis for all three apps and record parsed/gzip sizes for all significant modules. These measurements are prerequisites for every numeric estimate in US1–US4.

**⚠️ CRITICAL**: No user story audit work can begin until this phase is complete — all numeric KB estimates depend on the baseline data.

- [x] T007 Run `ANALYZE=true pnpm build` in `src/web/apps/webapp/`, record parsed and gzip sizes for all modules > 10 KB → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (webapp section)
- [x] T008 [P] Run `ANALYZE=true pnpm build` in `src/web/apps/webapp-teams/`, record parsed and gzip sizes → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (webapp-teams section)
- [x] T009 [P] Run `ANALYZE=true pnpm build` in `src/web/apps/webapp-spaces/`, record parsed and gzip sizes → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (webapp-spaces section)
- [x] T010 Consolidate bundle size measurements into a reference comparison table (parsed KB and gzip KB per library per app) in `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md`
- [ ] T010b Run Lighthouse audits (mobile preset, 3 runs each, averaged) against a production-equivalent build of each app — record LCP, FCP, CLS, and TBT baselines per route → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (Lighthouse baseline section). These measurements are required for FR-009 (image → LCP estimates) and FR-010 (font → FCP estimates).

**Checkpoint**: Baseline bundle sizes AND Lighthouse render-metric baselines recorded for all three apps. Numeric estimates can now be produced for both bundle-size and render-metric recommendations.

---

## Phase 3: User Story 1 — Complete Performance Audit of Every Page and Component (Priority: P1) 🎯 MVP

**Goal**: Produce a per-route and per-component audit table for every page route and every component file across all three apps, classifying each against all applicable optimization dimensions.

**Independent Test**: Verified by confirming that every `page.tsx` file found via `find src/web/apps/*/src/app -name "page.tsx"` has a corresponding entry in the route audit tables, and every `src/components/` and `src/rootPages/` directory has corresponding component entries — each with at least one optimization classification or an explicit "no optimization applicable" note.

### Page Route Audits

- [x] T011 [P] [US1] Audit all 28 webapp page routes — classify each across all optimization dimensions using baseline bundle data → `specs/022-ssr-rendering-audit/audit/webapp-routes.md`
- [x] T012 [P] [US1] Audit all ~55 webapp-teams page routes — include MS Teams iframe constraints per FR-012 → `specs/022-ssr-rendering-audit/audit/webapp-teams-routes.md`
- [x] T013 [P] [US1] Audit all ~60 webapp-spaces page routes — include spaces-specific admin constraints per FR-012 → `specs/022-ssr-rendering-audit/audit/webapp-spaces-routes.md`

### Component Domain Audits

- [x] T014 [P] [US1] Audit layout, shell, auth, notification, and observability components (`appBar`, `auth`, `feedback`, `gettingStarted`, `generics`, `loading`, `notification`, `observability`, `rootShell`, `styled`, `transitions`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-layout-auth.md`
- [x] T015 [P] [US1] Audit booking, marketplace, and payment components (`booking`, `marketplaceProduct`, `marketplaceProductBooking`, `marketplaceProductCard`, `marketplaceProductGuest`, `carousel`, `stripeConnectAccount`, `bankAccount`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-booking-payment.md`
- [x] T016 [P] [US1] Audit location, map, floor plan, and resource management components (`location`, `floorPlan`, `resource`, `resourceType`, `zone`, `search`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-location-map.md`
- [x] T017 [P] [US1] Audit organization, admin, storefront, product, team, user, and tag components (`organization`, `organizationStoreFrontGuest`, `product`, `productTag`, `team`, `user`, `customTag`, `setupFlow`, `availabilityDashboard`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-org-admin.md`
- [x] T018 [P] [US1] Audit analytics and charting components (`analytics`, chart/insight components using `@mui/x-charts`, `@mui/x-data-grid`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-analytics.md`
- [x] T019 [P] [US1] Audit form, utility, and generic components (`forms`, `datePickers`, `sorting`, `listGridToggle`, `listingMetadata`, `address`, `contactEmail`, `contactPeople`, `contactPhone`, `icons`, `avatars`, `links`, `slackButtons`, `closedOpenAllDayCustomToggle`, `weekOpeningHours`) across all three apps → `specs/022-ssr-rendering-audit/audit/components-forms-utils.md`
- [x] T020 [P] [US1] Audit all `src/rootPages/` files (16 in webapp, 36 in webapp-teams, 44 in webapp-spaces) and classify for optimization opportunities → add `rootPages` sections to each app's audit file

### Asset Findings

- [x] T021 [P] [US1] Audit all `next/image <Image>` usages and raw `<img>` tags across all three apps — identify missing `priority` props on above-the-fold images, missing `sizes` attributes, and raw `<img>` tag replacements with estimated LCP/CLS impact per FR-009 → `specs/022-ssr-rendering-audit/audit/asset-findings.md` (image section)
- [x] T022 [US1] Audit font loading across all three apps — assess TTF vs woff2 format, `display: 'swap'` usage, font subsetting, and variant count with estimated FCP impact per FR-010 → `specs/022-ssr-rendering-audit/audit/asset-findings.md` (font section)

**Checkpoint**: Every route and component in all three apps has a classification entry. Asset optimization gaps are documented with numeric impact estimates.

---

## Phase 4: User Story 2 — Relay and GraphQL Data-Fetching Pattern Review (Priority: P1)

**Goal**: Classify all top-level Relay query roots per route across all three apps, and produce an architectural assessment of the `AuthenticatedRelayProvider` + `AuthKitProvider` coupling as the primary structural blocker for SSR.

**Independent Test**: Verified by confirming that every route audit entry in webapp-routes.md, webapp-teams-routes.md, and webapp-spaces-routes.md has a corresponding entry in relay-queries.md classifying its top-level Relay query root as `can-prefetch`, `public-partial-prefetch`, or `must-stay-client`.

- [x] T023 [P] [US2] Identify and classify all top-level Relay query roots for webapp routes — map each query to its route entry and document auth dependency, public data eligibility, and server prefetch feasibility → `specs/022-ssr-rendering-audit/audit/relay-queries.md` (webapp section)
- [x] T024 [P] [US2] Identify and classify all top-level Relay query roots for webapp-teams routes — include MS Teams token dependency analysis → `specs/022-ssr-rendering-audit/audit/relay-queries.md` (webapp-teams section)
- [x] T025 [P] [US2] Identify and classify all top-level Relay query roots for webapp-spaces routes → `specs/022-ssr-rendering-audit/audit/relay-queries.md` (webapp-spaces section)
- [x] T026 [US2] Produce architectural assessment of `AuthenticatedRelayProvider` and `AuthKitProvider` coupling per FR-015: document why `useAuth()` forces a global client boundary, confirm `relay-environment.ts` `isServer: true` infrastructure is ready, and propose at least one strategy for enabling per-route server rendering alongside the global auth context → `specs/022-ssr-rendering-audit/audit/relay-queries.md` (architectural assessment section)

**Checkpoint**: All Relay query roots are classified. Architectural direction for server-side Relay is documented. SC-003 satisfied.

---

## Phase 5: User Story 3 — Static, Build-Time, and Lazy-Load Opportunity Identification (Priority: P2)

**Goal**: Enumerate all ISR/static generation candidates with revalidation intervals and build-time page-count estimates, and all heavy import lazy-load candidates with measured KB savings from bundle analysis.

**Independent Test**: Verified by confirming that `isr-static-candidates.md` contains at least one static or ISR candidate entry per app with a recommended revalidation period, and `lazy-load-candidates.md` contains at least one entry per identified heavy library with a numeric KB estimate from baseline bundle data.

- [x] T027 [US3] Identify all public-facing routes (unauthenticated access) across all three apps that are candidates for Static Site Generation or ISR — document expected data-change frequency, recommended revalidation interval, build-time page-count estimate, and any `generateStaticParams` data-volume considerations per FR-007 → `specs/022-ssr-rendering-audit/audit/isr-static-candidates.md`
- [x] T028 [P] [US3] Identify all heavy or lazily-loadable imports across all three apps per FR-008:
  1. **Open-ended scan**: Walk the T010 bundle treemap for every module with parsed size > 50 KB not already in the pre-identified list below — add any discovered module as a new finding
  2. **Pre-identified targets**: `react-leaflet`/`leaflet` (~200 KB), `@mui/x-charts` (~300 KB), `@mui/x-data-grid*` (~400 KB), `@stripe/react-stripe-js` (~100 KB), `logrocket` (~60 KB)
  3. **Per finding**: document recommended deferral mechanism (`next/dynamic` vs `React.lazy`), `ssr: false` requirement, loading fallback, and measured KB saving from bundle treemap
     → `specs/022-ssr-rendering-audit/audit/lazy-load-candidates.md`

**Checkpoint**: ISR/static candidates documented with revalidation intervals. Lazy-load candidates documented with measured KB savings.

---

## Phase 6: User Story 4 — Component-Level Client Boundary Minimization (Priority: P2)

**Goal**: Identify at least 5 specific component files per app where `'use client'` boundaries can be narrowed, showing what moves to the server and preserving Relay fragment colocation constraints.

**Independent Test**: Verified by confirming that `client-boundary-findings.md` contains at least 5 entries per app, each with the current boundary file, the extractable client-only element, the proposed Server Component shell, and any Relay fragment colocation constraints.

- [x] T029 [P] [US4] Walk the webapp component tree and identify `'use client'` boundary narrowing opportunities — for each: document the client reason category, the extractable interactive element, the proposed Server Component outer shell, and Relay colocation constraints per FR-005, FR-015 → `specs/022-ssr-rendering-audit/audit/client-boundary-findings.md` (webapp section)
- [x] T030 [P] [US4] Walk the webapp-teams component tree and identify `'use client'` boundary narrowing opportunities — note `InMsTeamsContext` dependency and MS Teams token acquisition constraints → `specs/022-ssr-rendering-audit/audit/client-boundary-findings.md` (webapp-teams section)
- [x] T031 [P] [US4] Walk the webapp-spaces component tree and identify `'use client'` boundary narrowing opportunities → `specs/022-ssr-rendering-audit/audit/client-boundary-findings.md` (webapp-spaces section)

**Checkpoint**: At least 5 boundary narrowing candidates per app documented with extractable client parts and Server Component shells.

---

## Phase 7: User Story 5 — Shared Package SSR Compatibility Review (Priority: P3)

**Goal**: Classify all major exports from `@skedular/ui` and `@skedular/shared` as server-safe, client-only, or universal; flag all app files that import client-only exports in Server Component contexts.

**Independent Test**: Verified by confirming that `shared-packages.md` has a classification entry for every export in `src/web/packages/ui/src/index.ts` and `src/web/packages/shared/src/index.ts` (or equivalent barrel files), and that each client-only export lists any routes or components importing it that would become Server Component candidates.

- [x] T032 [P] [US5] Classify all exports from `@skedular/ui` (`src/web/packages/ui/src/`) as server-safe, client-only, or universal — document reason for client-only classification (hook, context consumer, browser API, MUI dependency) per FR-011 → `specs/022-ssr-rendering-audit/audit/shared-packages.md` (@skedular/ui section)
- [x] T033 [P] [US5] Classify all exports from `@skedular/shared` (`src/web/packages/shared/src/`) as server-safe, client-only, or universal — pay special attention to `AuthenticatedRelayProvider`, `RelayProvider`, `InMsTeamsProvider`, `ThemeProvider`, `PaletteModeProvider`, and all hooks per FR-011 → `specs/022-ssr-rendering-audit/audit/shared-packages.md` (@skedular/shared section)
- [x] T034 [US5] Cross-reference client-only shared exports against all Server Component candidates identified in T011–T031 — flag cascade impact: routes or components that import a client-only export and would need to be converted to Client Components or restructured per FR-011 → `specs/022-ssr-rendering-audit/audit/shared-packages.md` (cascading impact section)

**Checkpoint**: All shared package exports classified. Cascading client-only import impact documented across all three apps. SC-004 satisfied.

---

## Final Phase: Polish & Cross-Cutting Concerns

**Purpose**: Consolidate all findings into a prioritized summary report, add pipeline and observability impact notes, and gate completion through requester review.

- [x] T035 Produce prioritized top-N recommendation list ranked by estimated performance impact:
  1. **Per-category coverage check**: verify at least one actionable recommendation exists for each of the required categories — Server Components conversion, Static Generation / ISR, lazy loading / code splitting, bundle size reduction, and asset (image/font) optimization — per SC-002
  2. **Top-5 ranking**: identify the top-5 highest-impact opportunities, each citing a specific measured metric (KB removed, LCP delta in ms, server round-trips eliminated) per SC-006
     → `specs/022-ssr-rendering-audit/audit/summary.md`
- [x] T036 [P] Add product-specific constraint notes for webapp-teams (MS Teams iframe, async token acquisition) and webapp-spaces (spaces admin context) to each affected recommendation per FR-012 → `specs/022-ssr-rendering-audit/audit/summary.md` (product constraints section)
- [x] T037 [P] For each recommendation in the summary, annotate whether it requires Relay artifact regeneration or backend GraphQL schema changes per FR-014 → `specs/022-ssr-rendering-audit/audit/summary.md` (generation pipeline impact section)
- [x] T038 [P] For each recommendation, note which once-implemented changes would require new observability instrumentation (server-side fetch durations, SSR render error surfaces) per LOG-003 → `specs/022-ssr-rendering-audit/audit/summary.md` (observability impact section)
- [x] T039 Submit completed audit report (`specs/022-ssr-rendering-audit/audit/summary.md` and all audit sub-files) to requester for review and actionability confirmation — feature is NOT complete until requester confirms the report provides sufficient detail to begin Phase 2 implementation without re-research per SC-005. **Reviewer checklist**: (1) if any PoC code was produced during Phase 1, verify it follows structured logging conventions (LOG-002) and includes correlation context comments (LOG-004); (2) confirm all 7 success criteria (SC-001 through SC-007) are satisfied.

**Checkpoint**: Audit complete. Requester has confirmed the report is actionable (SC-005). All 7 success criteria satisfied.

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies — can start immediately
- **Foundational (Phase 2)**: Depends on Phase 1 completion (T004/T005/T006) — **BLOCKS all user stories**
- **US1 (Phase 3)**: Depends on Phase 2 (T010 baseline data) — route audits T011–T013 and component audits T014–T020 can all run in parallel after T010
- **US2 (Phase 4)**: Depends on Phase 2 baseline; T023–T025 can run in parallel with US1; T026 (architectural assessment) depends on T023–T025
- **US3 (Phase 5)**: Depends on Phase 2 baseline; T027 (ISR) and T028 (lazy-load) can run in parallel with US1/US2
- **US4 (Phase 6)**: Depends on Phase 2; T029–T031 can run in parallel with US1–US3
- **US5 (Phase 7)**: Depends on Phase 2; T032–T033 can run in parallel with US1–US4; T034 depends on T032, T033, and US1 findings (T011–T031)
- **Polish (Final Phase)**: T035 depends on all US1–US5 being complete; T036–T038 depend on T035; T039 depends on T036–T038

### User Story Dependencies

- **US1 (P1)**: Can start after Foundational — no dependency on other stories
- **US2 (P1)**: Can start after Foundational — independent from US1 research, but T026 architectural assessment is richer with US1 route data available
- **US3 (P2)**: Can start after Foundational — route data from US1 improves ISR candidate identification, but not strictly required
- **US4 (P2)**: Can start after Foundational — component tree walk is independent; US1 component audit findings inform US4 but tasks can proceed in parallel
- **US5 (P3)**: Can start after Foundational — independent from US1–US4; T034 cascade analysis requires US1 findings

### Parallel Opportunities Within Phases

- **Phase 1**: T001–T003 in parallel (three apps), then T004–T006 in parallel
- **Phase 2**: T007–T009 in parallel (three apps), then T010 consolidation
- **Phase 3**: T011–T022 all parallelizable (different files/domains)
- **Phase 4**: T023–T025 in parallel, then T026
- **Phase 6**: T029–T031 in parallel

---

## Parallel Execution Example: US1 (Phase 3)

```bash
# After T010 completes, launch all of these simultaneously:

# Route audits (separate output files):
"T011: Audit all 28 webapp page routes → webapp-routes.md"
"T012: Audit all ~55 webapp-teams routes → webapp-teams-routes.md"
"T013: Audit all ~60 webapp-spaces routes → webapp-spaces-routes.md"

# Component domain audits (separate output files):
"T014: layout/shell/auth/observability components → components-layout-auth.md"
"T015: booking/marketplace/payment components → components-booking-payment.md"
"T016: location/map/floorPlan/resource components → components-location-map.md"
"T017: org/admin/product/team/user components → components-org-admin.md"
"T018: analytics/charts components → components-analytics.md"
"T019: forms/utility/generic components → components-forms-utils.md"
"T020: all rootPages/ files → each app's audit file"

# Asset audits (same output file, separate sections):
"T021: image audit → asset-findings.md (image section)"
"T022: font audit → asset-findings.md (font section)"
```

---

## Implementation Strategy

### MVP First (US1 + US2 Only — P1 Stories)

1. Complete Phase 1: Setup (install bundle analyzer)
2. Complete Phase 2: Foundational (run builds, get baseline sizes)
3. Complete Phase 3: US1 (full route + component + asset audit)
4. Complete Phase 4: US2 (Relay classification + architectural assessment)
5. **STOP and VALIDATE**: Route tables cover 100% of routes (SC-001); relay assessment done (SC-003)
6. Proceed to Phase 5–7 if P2/P3 stories are in scope

### Incremental Delivery

1. **Foundation** (Phase 1+2) → Bundle analyzer running, baselines recorded
2. **US1 + US2** (Phase 3+4) → Complete route audit + relay analysis → SC-001, SC-002, SC-003 satisfied → share interim draft with requester
3. **US3 + US4** (Phase 5+6) → ISR/static + lazy-load + client boundary → SC-002 fully satisfied
4. **US5** (Phase 7) → Shared packages classified → SC-004 satisfied
5. **Polish** (Final Phase) → Consolidated summary with top-5 ranked → SC-006 satisfied → requester review → SC-005 gate

---

## Phase 8: Implementation — P0: Fix `@skedular/ui` Barrel Contamination (Audit Finding: LL-001)

**Goal**: Remove `react-svg-credit-card-payment-icons` (521 KB parsed / 180 KB gzip) from every route by removing the `CreditCard` re-export from the `@skedular/ui` commons barrel. Update the 2 consumers to import directly. Affects all 3 apps.

**Independent Test**: Run `ANALYZE=true pnpm build` in webapp after T044 completes — `react-svg-credit-card-payment-icons` must no longer appear in the treemap for any route except the billing/subscriptions routes.

- [x] T040 [US5] Write component test for `organization-admin-billing-payment-section.tsx` verifying `CreditCard` renders correctly (baseline — will also pass after the direct-import change) → `src/web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-billing-payment-section.test.tsx`
- [x] T041 [P] [US5] Write component test for `organization-admin-subscriptions-section.tsx` verifying `CreditCard` renders correctly → `src/web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-subscriptions-section.test.tsx`
- [x] T042 [US5] Update `organization-admin-billing-payment-section.tsx` — change `import { CreditCard } from '@skedular/ui'` to `import CreditCard from '@skedular/ui/commons/credit-card'` → `src/web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-billing-payment-section.tsx`
- [x] T043 [P] [US5] Update `organization-admin-subscriptions-section.tsx` — change `import { CreditCard } from '@skedular/ui'` to `import CreditCard from '@skedular/ui/commons/credit-card'` → `src/web/apps/webapp/src/components/organization/organizationAdmin/organization-admin-subscriptions-section.tsx`
- [x] T044 [US5] Remove `export { default as CreditCard } from './credit-card'` from `src/web/packages/ui/src/commons/index.ts`
- [x] T045 [US5] Run `pnpm --filter webapp exec tsc --noEmit` (and webapp-teams, webapp-spaces) to verify no other consumers reference `CreditCard` via the barrel — fix any additional consumers discovered by the compiler

**Checkpoint**: TypeScript build is green. `CreditCard` is no longer in the `@skedular/ui` commons barrel. Both consumer components import directly and their tests pass.

---

## Phase 9: Implementation — P2: Convert Barlow Fonts TTF → woff2 (Audit Finding: asset-findings.md)

**Goal**: Replace 4 Barlow TTF files (424 KB total per app) with woff2 equivalents (~318 KB total per app, ~106 KB saving), and update the `localFont` declarations in all 3 apps' root layouts.

**Independent Test**: After T054 completes, `pnpm build` in each app succeeds; font file size comparison confirms ≥20% reduction per variant.

- [x] T046 [US1] Write layout render test for `webapp/src/app/layout.tsx` that mocks `next/font/local` and `next/headers`, renders the layout, and asserts the Barlow font CSS variable (`--font-barlow`) is applied and no `.ttf` path strings appear in the font source configuration → `src/web/apps/webapp/src/app/layout.test.tsx`
- [x] T047 [P] [US1] Write layout render test for `webapp-teams/src/app/layout.tsx` (same pattern, mock `next/font/local` and `next/headers`) → `src/web/apps/webapp-teams/src/app/layout.test.tsx`
- [x] T048 [P] [US1] Write layout render test for `webapp-spaces/src/app/layout.tsx` → `src/web/apps/webapp-spaces/src/app/layout.test.tsx`
- [x] T049 [US1] Convert the 4 Barlow TTF files to woff2 using `woff2_compress` (or equivalent — `ffmpeg -i input.ttf output.woff2`) in `src/web/apps/webapp/src/app/fonts/` — produce `Barlow-Regular.woff2`, `Barlow-Medium.woff2`, `Barlow-SemiBold.woff2`, `Barlow-Bold.woff2`
- [x] T050 [P] [US1] Convert 4 Barlow TTF files to woff2 in `src/web/apps/webapp-teams/src/app/fonts/`
- [x] T051 [P] [US1] Convert 4 Barlow TTF files to woff2 in `src/web/apps/webapp-spaces/src/app/fonts/`
- [x] T052 [US1] Update the `barlow` `localFont` `src` array in `src/web/apps/webapp/src/app/layout.tsx` — change all 4 variant `path` values from `.ttf` to `.woff2` (Regular, Medium, SemiBold, Bold)
- [x] T053 [P] [US1] Update the `barlow` `localFont` `src` array in `src/web/apps/webapp-teams/src/app/layout.tsx`
- [x] T054 [P] [US1] Update the `barlow` `localFont` `src` array in `src/web/apps/webapp-spaces/src/app/layout.tsx`

**Checkpoint**: All 3 apps build successfully. Barlow woff2 files exist in all 3 fonts directories. Layout tests pass with no `.ttf` path references in font configuration.

---

## Phase 10: Implementation — P2: Move `node-ipinfo` to Server API Route (Audit Finding: LL-001/LL-002)

**Goal**: Remove `node-ipinfo` (41 KB) from the webapp client bundle by moving it behind a Next.js API route. Fixes both the bundle size and the security issue of server-side Node.js code running in a browser context.

**Independent Test**: After T058 completes, `grep -r "node-ipinfo" src/web/apps/webapp/src/components/` returns no results. The geolocation API route returns a valid JSON response.

- [x] T055 [US3] Write test for the new `GET /api/geolocation` route — mock the `node-ipinfo` `IPinfoWrapper` client and verify the route returns `{ city, region, country, lat, lng }` as JSON with `200` status → `src/web/apps/webapp/src/app/api/geolocation/route.test.ts`
- [x] T056 [US3] Create `src/web/apps/webapp/src/app/api/geolocation/route.ts` — `GET` handler that reads the client IP from request headers (`x-forwarded-for`), calls `node-ipinfo` server-side, and returns `{ city, region, country, lat, lng }` as a `NextResponse.json(...)` response; reads `IPINFO_TOKEN` from environment
- [x] T057 [US3] Write component test for `marketplace-locations.tsx` verifying it calls `fetch('/api/geolocation')` on mount (mock `fetch` with `vi.fn()`) and does NOT import `node-ipinfo` → `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.test.tsx`
- [x] T058 [US3] Update `src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx` — remove the direct `node-ipinfo` import and replace the geolocation call with `fetch('/api/geolocation').then(r => r.json())`

**Checkpoint**: `node-ipinfo` is no longer imported in any client component. The API route test passes. The `marketplace-locations.tsx` component test confirms it uses `fetch('/api/geolocation')`.

---

## Phase 11: Implementation — P2: Add `'use client'` Defensive Directives (Audit Finding: CB-004)

**Goal**: Add `'use client'` to 7 interactive components that currently lack the directive. These components use React state/hooks and would error silently if any parent page is later converted to a Server Component.

**Independent Test**: After T072 completes, `grep -L "'use client'" src/web/apps/webapp/src/components/datePickers/*.tsx src/web/apps/webapp/src/components/listGridToggle/*.tsx src/web/apps/webapp/src/components/sorting/*.tsx src/web/apps/webapp/src/components/weekOpeningHours/*.tsx src/web/apps/webapp/src/components/closedOpenAllDayCustomToggle/*.tsx` returns no files (all have the directive).

- [x] T059 [US4] Write render test for `day-picker.tsx` — mock `@skedular/ui`, `@skedular/shared`, `@mui/x-date-pickers/StaticDatePicker`, and Relay fragments; assert the component renders without error → `src/web/apps/webapp/src/components/datePickers/day-picker.test.tsx`
- [x] T060 [P] [US4] Write render test for `week-picker.tsx` — mock `@skedular/ui`, `@skedular/shared`, and MUI dependencies → `src/web/apps/webapp/src/components/datePickers/week-picker.test.tsx`
- [x] T061 [P] [US4] Write render test for `week-range-picker.tsx` → `src/web/apps/webapp/src/components/datePickers/week-range-picker.test.tsx`
- [x] T062 [P] [US4] Write render test for `list-grid-toggle.tsx` — mock MUI `ToggleButtonGroup`/`ToggleButton` and icon imports → `src/web/apps/webapp/src/components/listGridToggle/list-grid-toggle.test.tsx`
- [x] T063 [P] [US4] Write render test for `sorting.tsx` — mock `@skedular/ui`, MUI `IconButton`/`Divider`, and icon imports → `src/web/apps/webapp/src/components/sorting/sorting.test.tsx`
- [x] T064 [P] [US4] Write render test for `week-opening-hours.tsx` — mock `react-relay` (`useFragment`), `@skedular/ui`, `@skedular/shared`, and `ClosedOpenAllDayCustomToggle` → `src/web/apps/webapp/src/components/weekOpeningHours/week-opening-hours.test.tsx`
- [x] T065 [P] [US4] Write render test for `closed-open-all-day-custom-toggle.tsx` — mock MUI `ToggleButtonGroup`/`ToggleButton`, `Tooltip`, and icon imports → `src/web/apps/webapp/src/components/closedOpenAllDayCustomToggle/closed-open-all-day-custom-toggle.test.tsx`
- [x] T066 [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/datePickers/day-picker.tsx`
- [x] T067 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/datePickers/week-picker.tsx`
- [x] T068 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/datePickers/week-range-picker.tsx`
- [x] T069 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/listGridToggle/list-grid-toggle.tsx`
- [x] T070 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/sorting/sorting.tsx`
- [x] T071 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/weekOpeningHours/week-opening-hours.tsx`
- [x] T072 [P] [US4] Add `'use client'` directive as the first line of `src/web/apps/webapp/src/components/closedOpenAllDayCustomToggle/closed-open-all-day-custom-toggle.tsx`

**Checkpoint**: All 7 components have `'use client'` as their first line. All 7 render tests pass. TypeScript build is green.

---

## Phase 12 (Final): Phase 2 Verification & Bundle Measurement

**Purpose**: Confirm Phase 2 changes deliver the projected bundle savings and produce the post-implementation baseline that satisfies SC-007. Remove the obsolete `.ttf` files after confirming `.woff2` equivalents are in place and builds succeed.

- [x] T073 Re-run `ANALYZE=true pnpm build` in `src/web/apps/webapp/`, record post-Phase-2 parsed and gzip sizes for all modules, and verify `react-svg-credit-card-payment-icons` is absent from non-billing routes (target: −180 KB gzip from barrel fix + −106 KB from font conversion) → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (Phase 2 — webapp results section)
- [x] T074 [P] Re-run `ANALYZE=true pnpm build` in `src/web/apps/webapp-teams/` and record post-Phase-2 bundle sizes (target: −180 KB gzip from barrel fix + −106 KB from font conversion) → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (Phase 2 — webapp-teams results section)
- [x] T075 [P] Re-run `ANALYZE=true pnpm build` in `src/web/apps/webapp-spaces/` and record post-Phase-2 bundle sizes → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (Phase 2 — webapp-spaces results section)
- [x] T076 Confirm woff2 file sizes for all 4 Barlow variants across all 3 apps' `fonts/` directories, record actual savings vs TTF originals, then delete the `.ttf` originals (`Barlow-Regular.ttf`, `Barlow-Medium.ttf`, `Barlow-SemiBold.ttf`, `Barlow-Bold.ttf`) from each app's fonts directory → `specs/022-ssr-rendering-audit/audit/baseline-bundle-sizes.md` (font comparison section)
- [x] T077 Update `specs/022-ssr-rendering-audit/audit/summary.md` with Phase 2 implementation outcomes — record actual KB reductions per app vs Phase 1 projected values, note any variance, mark SC-007 (post-implementation measurement) as satisfied → `specs/022-ssr-rendering-audit/audit/summary.md` (Phase 2 outcomes section)

**Checkpoint**: All Phase 2 bundle analyzer runs complete. Actual savings match or exceed projections. SC-007 satisfied. `.ttf` files removed. Phase 2 complete.

---

## Phase 2 Dependencies & Execution Order

### Phase Dependencies

- **Phase 8 (Barrel Fix)**: No external dependencies — can start immediately after Phase 1 is complete. T040–T041 (tests) run first, then T042–T043 (consumer updates) in parallel, then T044 (barrel removal), then T045 (compiler check).
- **Phase 9 (Fonts)**: No dependency on Phase 8. T046–T048 (layout tests) run first in parallel, then T049–T051 (font conversion) in parallel, then T052–T054 (layout.tsx updates) in parallel.
- **Phase 10 (node-ipinfo)**: No dependency on Phase 8 or 9. T055 (route test) → T056 (route implementation) → T057 (component test) → T058 (component update).
- **Phase 11 ('use client')**: No dependency on Phase 8, 9, or 10. T059–T065 (all 7 render tests) in parallel, then T066–T072 (all 7 directive additions) in parallel.
- **Phase 12 (Verification)**: Depends on Phase 8 + 9 + 10 + 11 all being complete. T073–T075 in parallel, then T076, then T077.

### Parallel Opportunities Within Phase 2

```bash
# Phase 8: barrel fix — after T040/T041 tests pass:
"T042: Update billing-payment-section.tsx"  # parallel with T043
"T043: Update subscriptions-section.tsx"    # parallel with T042
# Then sequentially: T044 → T045

# Phase 9: fonts — all can run across 3 apps in parallel:
"T046 / T047 / T048: Layout tests (3 apps)"  # all parallel
"T049 / T050 / T051: Convert TTF→woff2 (3 apps)"  # all parallel
"T052 / T053 / T054: Update layout.tsx paths (3 apps)"  # all parallel

# Phase 10: sequential (each step builds on previous)
T055 → T056 → T057 → T058

# Phase 11: 'use client' — all 7 tests in parallel, then all 7 changes in parallel:
"T059–T065: Render tests for 7 components"  # all parallel
"T066–T072: Add 'use client' to 7 components"  # all parallel

# Phase 12: verification — after all phases complete:
"T073 / T074 / T075: Bundle analysis (3 apps)"  # all parallel
# Then sequentially: T076 → T077
```

---

## Phase 2 Implementation Strategy

### MVP First (Phase 8 — P0 Barrel Fix)

1. Complete Phase 8 (barrel contamination fix) — highest ROI, 180 KB gzip reduction per app, 3 apps
2. **STOP and VERIFY**: Run bundle analyzer, confirm savings match projection
3. Proceed to Phase 9 (fonts) — next-highest ROI, Very Low risk
4. Then Phase 10 (node-ipinfo) — security + bundle improvement, Medium effort
5. Then Phase 11 ('use client') — defensive, Very Low effort
6. Complete Phase 12 (verification) — SC-007 gate

### Rollback Safety

- **Phase 8**: Revert is 2 consumer file changes + 1 barrel file change. TypeScript compiler catches any missed consumers before merge.
- **Phase 9**: Revert is changing 4 path strings per app back to `.ttf` and restoring the original files.
- **Phase 10**: The API route is a new additive file. Reverting means removing `route.ts` and restoring the single import in `marketplace-locations.tsx`.
- **Phase 11**: Revert means removing `'use client'` from 7 files — zero behavioral change either way.
