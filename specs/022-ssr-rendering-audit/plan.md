# Implementation Plan: Web App Performance Optimization Audit

**Branch**: `022-ssr-rendering-audit` | **Date**: 2026-06-03 | **Spec**: [spec.md](spec.md)  
**Input**: Feature specification from `specs/022-ssr-rendering-audit/spec.md`

## Summary

Comprehensive performance audit and optimization across all three web apps (`webapp`, `webapp-teams`, `webapp-spaces`). The feature runs in two sequential phases:

- **Phase 1 (Audit, T001–T039)**: Classify every route and component across all applicable optimization dimensions (Server Components, ISR, lazy loading, bundle reduction, image/font optimization, client boundary narrowing, Relay prefetch feasibility). Capture numeric baselines with `@next/bundle-analyzer` and Lighthouse. Produce a prioritized recommendation list, gated by requester review (SC-005).
- **Phase 2 (Implementation, tasks generated after SC-005)**: Implement all actionable findings from Phase 1 as production code changes across all three apps and shared packages, verified by post-implementation bundle and Lighthouse measurements (SC-007).

---

## Technical Context

**Language/Version**: TypeScript 6.0.3  
**Primary Dependencies**: React 19.2.7, Next.js 16.2.7 App Router, Relay 21.0.1 (`react-relay`), MUI v9, `@skedular/ui`, `@skedular/shared`, WorkOS AuthKit (`@workos-inc/authkit-nextjs ^4.1.1`), `@next/bundle-analyzer` (to be installed)  
**Storage**: N/A — this is a research/audit feature; no new persistence  
**Testing**: Vitest + React Testing Library — applicable to any PoC code produced; no new test requirements for research deliverables  
**Target Platform**: Browser (Next.js App Router on Vercel)  
**Project Type**: Research audit → structured markdown deliverables + optional PoC code snippets  
**Performance Goals**: Audit must produce numeric bundle size estimates (KB, parsed and gzip) for every lazy-load recommendation (SC-006); latency baseline from Lighthouse (mobile preset, 3-run average) per T010b. Bundle size recording threshold: modules with parsed size > 10 KB are captured in the baseline (T007–T009); modules > 50 KB are treated as lazy-load candidates for T028 open-ended scan.  
**Constraints**: No changes to `api-definitions/` or generated GraphQL/Relay artifacts during the audit phase; PoC code may demonstrate patterns but must not break production builds  
**Scale/Scope**: 28 routes in webapp, ~55 in webapp-teams, ~60 in webapp-spaces; all component files under `src/components/`, `src/rootPages/`, and shared packages

---

## Constitution Check

- [x] **I. Contract-First** — Does this feature touch `api-definitions/` or any generated surface?  
       **No.** The audit is a pure research deliverable. No `api-definitions/` files are modified. No generated GraphQL schemas, Relay artifacts, OpenAPI controllers, or protobuf outputs are changed. Any PoC code that calls existing GraphQL endpoints uses only already-generated Relay artifacts without modification.

- [x] **II. Domain Boundaries** — Does this feature cross domain ownership lines?  
       **No.** Frontend-only audit. No cross-domain service calls, no backend C# changes, no direct database access.

- [x] **III. Testing** — What test tier is required?  
       **Phase 1 — Proportionate**: Research deliverable with no runtime behavior change; PoC code follows Vitest/React Testing Library conventions if written. **Phase 2 — Full compliance**: All frontend code changes require Vitest/React Testing Library tests. Server Component conversions, ISR configurations, lazy-load wrappers, and client boundary changes all require component-level tests asserting observable behavior. No persistence or integration boundaries are crossed; no integration tests needed.
- [x] **IV. Frontend** — Does this feature include web changes?  
       **Yes** (Phase 1: audit scope + optional PoC; Phase 2: full production code changes across all three apps and shared packages). All Phase 2 code must: follow Relay fragment colocation patterns and regenerate artifacts after any query/fragment change (never hand-edit), use `@skedular/ui` typography wrappers (not raw MUI `Typography`), and use American spelling in all user-facing copy.
- [x] **V. Pattern Consistency** — Does this feature introduce a new pattern or deviate from an existing one?  
       **No new patterns from the audit itself.** Recommendations in the audit may propose new patterns (e.g., ISR via `revalidate`, `next/dynamic` lazy loading) but those patterns are deferred to implementation features spawned from this audit's tasks. The audit documents these as proposals, not implementations.

- [x] **VI. Logging** — Does this feature add or change behavior?  
       **LOG-001 applies** — the spec explicitly states no runtime logging changes in this feature. Any PoC code must follow LOG-002 (structured logging conventions per `Microsoft.Extensions.Logging` if ever applicable, or equivalent for frontend). Frontend PoC code must not introduce `console.log` calls in production paths.

---

## Project Structure

### Documentation (this feature)

```text
specs/022-ssr-rendering-audit/
├── plan.md              # This file (/speckit.plan output)
├── research.md          # Phase 0 output — codebase findings, resolved unknowns
├── data-model.md        # Phase 1 output — audit classification schema
├── quickstart.md        # Phase 1 output — bundle analyzer setup and usage
├── checklists/
│   └── requirements.md  # Quality checklist (pre-existing)
└── tasks.md             # Phase 2 output (/speckit.tasks — NOT created here)
```

Audit deliverable files (produced by tasks):

```text
specs/022-ssr-rendering-audit/audit/
├── baseline-bundle-sizes.md      # T007–T010: parsed/gzip sizes per library per app
├── webapp-routes.md              # T011: per-route audit — webapp (28 routes)
├── webapp-teams-routes.md        # T012: per-route audit — webapp-teams (~55 routes)
├── webapp-spaces-routes.md       # T013: per-route audit — webapp-spaces (~60 routes)
├── components-layout-auth.md     # T014: layout, shell, auth, observability components
├── components-booking-payment.md # T015: booking, marketplace, payment components
├── components-location-map.md    # T016: location, map, floor plan, resource components
├── components-org-admin.md       # T017: organization, admin, product, team, user components
├── components-analytics.md       # T018: analytics and charting components
├── components-forms-utils.md     # T019: forms, utility, and generic components
├── relay-queries.md              # T023–T026: Relay query classifications + architectural assessment
├── isr-static-candidates.md      # T027: ISR/static generation candidates with revalidation intervals
├── lazy-load-candidates.md       # T028: lazy-load candidates with measured KB savings
├── client-boundary-findings.md   # T029–T031: 'use client' boundary narrowing opportunities
├── shared-packages.md            # T032–T034: @skedular/ui and @skedular/shared export classifications
└── summary.md                    # T035–T038: prioritized top-N recommendations with numeric metrics
```

### Source Code (repository root)

Phase 1 adds build tooling only (T001–T006: `@next/bundle-analyzer` dev dependency + `next.config.ts` wrapper). Phase 2 introduces production code changes across all three apps and shared packages based on audit findings. Typical Phase 2 change surfaces:

```text
src/web/apps/webapp/src/
├── app/
│   └── [route]/
│       └── page.tsx         # Server Component conversion / ISR revalidate
src/web/apps/webapp-teams/src/   # same pattern
src/web/apps/webapp-spaces/src/  # same pattern
src/web/packages/shared/src/
└── providers/
    └── [provider].tsx       # narrowed client boundary
src/web/packages/ui/src/
└── [component].tsx          # server-safe export additions
```

**Structure Decision**: Research artifacts and Phase 1 outputs land in `specs/022-ssr-rendering-audit/audit/`. Phase 2 code changes modify existing source files in-place rather than creating new directories.

---

## Key Research Findings (from research.md)

### 1. Global Client Boundary (Highest Impact)

`ClientRootLayout` wraps **all app children** in a `'use client'` boundary due to:

- `AuthKitProvider` → `useAuth()` hook (WorkOS, client-only)
- `AuthenticatedRelayProvider` → `useContext(InMsTeamsContext)` + conditional render
- `PaletteModeProvider` / `ThemeProvider` → context hooks

**Finding**: `relay-environment.ts` already creates server-side environments with `isServer: true`. The infrastructure is Relay-server-ready. The blocker is auth token availability — `useAuth()` is client-side only.

**Recommended approach**:

1. Identify public routes where no auth token is needed (marketplace landing, product pages).
2. Introduce a parallel server-fetch path for those routes using plain `fetch` + ISR.
3. Keep the auth-gated routes client-rendered; narrow the client boundary within them where possible.

### 2. ISR Gap (Zero Usage Today)

No `generateStaticParams`, `revalidate`, `unstable_cache`, or `getStaticProps` usage anywhere in the three apps. Public marketplace pages (`/marketplace/organizations/[domain]`, `/marketplace/products/[productId]`, etc.) are prime ISR candidates.

### 3. Lazy-Load Candidates (Bundle Size)

| Library                     | Estimated Bundle Size | Current Status                                                                 |
| --------------------------- | --------------------- | ------------------------------------------------------------------------------ |
| `react-leaflet` / `leaflet` | ~200 KB               | Uses `dynamicLoadReady` state flag — NOT `next/dynamic`; still eagerly bundled |
| `@mui/x-charts`             | ~300 KB               | Eagerly imported in insight components                                         |
| `@mui/x-data-grid` family   | ~400 KB               | Eagerly imported in admin pages                                                |
| `@stripe/react-stripe-js`   | ~100 KB               | Eagerly imported in payment components                                         |

**Note**: Estimated sizes are parse-time estimates from library documentation. Actual values must be confirmed by running `ANALYZE=true pnpm build` per the quickstart guide.

### 4. Image Optimization (Quick Win)

- 47 uses of `next/image <Image>` — adoption is good.
- **Zero** `priority` props anywhere — all LCP images (org logos, location hero, product hero) lack the priority hint. Adding `priority` to above-the-fold images is the single highest-leverage, lowest-effort image fix.
- 4 raw `<img>` tags — should be replaced with `<Image>`.

### 5. Font Format (Moderate Win)

Barlow fonts loaded as TTF (4 weights). TTF is ~2–3× larger than woff2. Converting to woff2 reduces font payload with no visual change. `next/font/local` with `display: 'swap'` is already correct.

### 6. `'use client'` Spread Statistics

| Location                 | Files |
| ------------------------ | ----- |
| webapp `src/app/`        | 29    |
| webapp-teams `src/app/`  | 65    |
| webapp-spaces `src/app/` | 74    |
| @skedular/ui             | 42    |
| @skedular/shared         | 18    |

---

## Complexity Tracking

> No Constitution Check violations requiring justification. All gates pass cleanly.

| Violation | Why Needed | Simpler Alternative Rejected Because |
| --------- | ---------- | ------------------------------------ |
| (none)    | —          | —                                    |
