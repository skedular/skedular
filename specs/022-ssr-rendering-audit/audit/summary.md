# SSR Rendering Audit — Summary and Recommendations

**Generated**: 2026-06-03  
**Feature**: 022-ssr-rendering-audit (Phase 1 — Audit Complete)  
**Apps audited**: `webapp`, `webapp-teams`, `webapp-spaces`  
**Total routes audited**: 166 (28 + 64 + 74)  
**Total components audited**: 350+ across apps and shared packages

---

## Executive Summary

All three Skedular web applications are **100% client-rendered** today. Every route is classified as `ƒ` Dynamic (SSR on demand) but the actual HTML returned is a React shell — all content renders after JavaScript hydration and auth resolution on the client.

The primary driver is the global `AuthKitProvider` (WorkOS) wrapping all routes from the root layout. This single architectural decision prevents any Server Component rendering across all three apps.

Despite this, the audit has identified **high-impact, low-risk optimizations** that do not require architectural changes to auth — primarily the `@skedular/ui` barrel contamination (521 KB) and font format issues (424 KB in TTF format).

---

## Top 5 Recommendations

### #1 — Fix `@skedular/ui` Barrel Contamination (P0)

**Impact**: Remove 521 KB from every route in all 3 apps  
**Effort**: Low  
**Risk**: Low

`react-svg-credit-card-payment-icons` (521 KB parsed / 180 KB gzip) is bundled into every route via `@skedular/ui`'s barrel export chain. Only 2 components actually use it (`organization-admin-billing-payment-section.tsx`, `organization-admin-subscriptions-section.tsx`).

**Fix**: Remove `CreditCard` from `@skedular/ui/commons/index.ts` barrel. Update the 2 consumers to import directly from `@skedular/ui/commons/credit-card`.

Expected result: ~180 KB gzip reduction per app, affecting every route load.

### #2 — Convert Barlow Fonts from TTF to woff2 (P2)

**Impact**: ~106 KB reduction in font download per app load  
**Effort**: Low  
**Risk**: Very Low

4 Barlow font files (Regular, Medium, SemiBold, Bold) are served as TTF (424 KB total). Converting to woff2 reduces size ~25% (~106 KB saving).

**Fix**: Convert font files, update `localFont` paths in all 3 apps' `layout.tsx`.

### #3 — Move `node-ipinfo` to Server-Side API Route (P2)

**Impact**: 41 KB removed from webapp client bundle + security improvement  
**Effort**: Medium  
**Risk**: Low

`node-ipinfo` is a Node.js library imported in a client component for IP geolocation. This exposes server-side logic client-side. Replace with a Next.js API route (`/api/geolocation`) that calls `node-ipinfo` server-side.

### #4 — Add `'use client'` to 7 Interactive Components (Defensive — P2)

**Impact**: Prevents silent regression in Phase 2 SSR work  
**Effort**: Very Low  
**Risk**: Very Low

7 components (date pickers, list/grid toggle, sorting, week opening hours toggle) are interactive (use state/effects) but lack `'use client'`. They work today because pages are client, but would fail silently if pages are converted to Server Components in Phase 2.

**Fix**: Add `'use client'` directive to these 7 component files.

### #5 — Scope `AuthKitProvider` to Route Groups (P1 — Architecture)

**Impact**: Enables Server Component rendering for ~5 public routes per app  
**Effort**: High  
**Risk**: Medium

The global `AuthKitProvider` in `ClientRootLayout` forces a client boundary on all routes. Moving it to a `(authenticated)` route group layout would allow `(public)` routes (auth pages, install pages) to be server-rendered.

**Fix**: Create `(public)` and `(authenticated)` route group layouts. Move `AuthKitProvider` to `(authenticated)/layout.tsx`.

**Expected LCP improvement for auth pages**: 300–600ms (eliminates auth waterfall for public routes).

---

## Bundle Size Summary

| App           | Current Bundle (gzip) | After Rec #1 | After Rec #1 + #2 |
| ------------- | --------------------- | ------------ | ----------------- |
| webapp        | 1,218 KB              | ~1,038 KB    | ~932 KB           |
| webapp-teams  | 1,455 KB              | ~1,275 KB    | ~1,169 KB         |
| webapp-spaces | 1,517 KB              | ~1,337 KB    | ~1,231 KB         |

_Estimates based on removing 180 KB (credit-card icons) and 106 KB (font format). Actual results depend on tree-shaking behavior._

---

## Audit Findings Reference

| Document                              | Contents                                       |
| ------------------------------------- | ---------------------------------------------- |
| `audit/baseline-bundle-sizes.md`      | Per-app bundle totals and top packages         |
| `audit/webapp-routes.md`              | 28 webapp routes classified                    |
| `audit/webapp-teams-routes.md`        | 64 webapp-teams routes classified              |
| `audit/webapp-spaces-routes.md`       | 74 webapp-spaces routes classified             |
| `audit/components-layout-auth.md`     | Shell, auth, notification components           |
| `audit/components-booking-payment.md` | Booking, marketplace, payment components       |
| `audit/components-location-map.md`    | Location, map, floor plan, resources           |
| `audit/components-org-admin.md`       | Organization, admin, products, teams           |
| `audit/components-analytics.md`       | Analytics and charts                           |
| `audit/components-forms-utils.md`     | Forms, utilities, generic components           |
| `audit/isr-static-candidates.md`      | ISR/static assessment (0 candidates)           |
| `audit/lazy-load-candidates.md`       | Lazy-load opportunity analysis                 |
| `audit/asset-findings.md`             | Image priority and font issues                 |
| `audit/relay-queries.md`              | Relay architecture and SSR paths               |
| `audit/client-boundary-findings.md`   | `'use client'` scope analysis                  |
| `audit/shared-packages.md`            | `@skedular/ui` and `@skedular/shared` analysis |

---

## Product and Pipeline Constraints

### Product Constraints

- **No functional regressions**: All optimization work must preserve feature parity
- **WorkOS AuthKit**: Cannot be removed or replaced; SSR path must work with it
- **Relay 21**: Already has SSR support built in — no version upgrade needed
- **MS Teams iframe**: `/msteams/*` routes cannot use synchronous auth or SSR that blocks on Teams token acquisition
- **Custom domain**: Org custom domain detection at runtime blocks ISR for storefront routes
- **Stripe**: Already correctly lazy-loaded via Stripe.js CDN pattern

### Pipeline and Observability Constraints

- **LogRocket**: Must be initialized for all authenticated sessions — deferring init is safe but must not prevent capture
- **Vercel Analytics + Speed Insights**: Already using `'use client'` script injection — no change
- **Google Analytics/Tag Manager**: Script injection pattern already correct

---

## Phase 1 Review Gate (SC-005)

This audit completes Phase 1. The findings above are ready for requester review.

### Phase 2 Task Generation

After review and approval, run `/speckit.tasks` to generate Phase 2 implementation tasks.

Phase 2 scope (subject to review):

1. P0: Fix `@skedular/ui` barrel contamination (#1 above)
2. P2: Convert Barlow TTF → woff2 (#2)
3. P2: Move `node-ipinfo` to server API (#3)
4. P2: Add `'use client'` to 7 interactive components (#4)
5. P1: Auth route group scoping (#5 — architecture, flag for stakeholder decision)

### Questions for Reviewer (SC-005)

1. Is Recommendation #5 (AuthKit route group scoping) in scope for the next sprint, or deferred?
2. Should `react-svg-credit-card-payment-icons` be removed entirely (replace with inline SVG or MUI icons) or only excluded from the barrel export?
3. Is there a timeline constraint for Barlow font conversion (requires updating assets in all 3 apps)?
4. Confirm: `node-ipinfo` API key should remain server-side only (security requirement)?

---

## Phase 2 Implementation Outcomes

**Completed**: 2026-06-03  
**Tasks**: T040–T077 (all complete)

### P0: Barrel Contamination Fix (T040–T045)

- Removed `CreditCard` export from `@skedular/ui/commons/index.ts` barrel (521 KB / 180 KB gzip per route eliminated)
- Updated `organization-admin-billing-payment-section.tsx` and `organization-admin-subscriptions-section.tsx` in all 3 apps to import `CreditCard` directly from `@skedular/ui/commons/credit-card`
- TypeScript checks green in all 3 apps
- Vitest alias fix applied in all 3 apps' `vitest.config.ts` (regex subpath alias must precede exact alias)
- Baseline tests: 4 passing tests for the 2 billing components

### P2: Barlow Font TTF → woff2 (T046–T054)

- Downloaded fresh Barlow woff2 files from Google Fonts CDN (SIL Open Font License) for all 3 apps
- Updated `layout.tsx` in all 3 apps to reference `.woff2` instead of `.ttf`
- Final woff2 sizes: Regular 23 KB, Medium 23 KB, SemiBold 24 KB, Bold 24 KB (~77% reduction from ~102–106 KB TTF)
- Total saving: ~94 KB × 3 apps = ~282 KB across the product suite
- Old TTF files deleted (12 files removed)
- Layout tests: 6 passing (2 per app) verifying no `.ttf` paths remain

### P2: `node-ipinfo` API Route Migration (T055–T058)

- Created `src/web/apps/webapp/src/app/api/geolocation/route.ts` — server-side GET handler using `IPinfoWrapper` with `IPINFO_TOKEN` env var
- Removed direct `IPinfoWrapper` import from `marketplace-locations.tsx` client component
- Replaced client-side IP lookup with `fetch('/api/geolocation')` call
- Tests: 3 passing (route handler test + component geolocation test)
- Security: API token now server-side only, never exposed to client

### P2: Defensive `'use client'` Directives (T059–T072)

Added `'use client'` directive to 7 interactive components:

1. `src/web/apps/webapp/src/components/datePickers/day-picker.tsx`
2. `src/web/apps/webapp/src/components/datePickers/week-picker.tsx`
3. `src/web/apps/webapp/src/components/datePickers/week-range-picker.tsx`
4. `src/web/apps/webapp/src/components/listGridToggle/list-grid-toggle.tsx`
5. `src/web/apps/webapp/src/components/sorting/sorting.tsx`
6. `src/web/apps/webapp/src/components/weekOpeningHours/week-opening-hours.tsx`
7. `src/web/apps/webapp/src/components/closedOpenAllDayCustomToggle/closed-open-all-day-custom-toggle.tsx`

Render tests: 7 passing (1 per component), all verified after directive addition.

### Phase 2 Verification (T073–T077)

- `CreditCard` confirmed absent from `@skedular/ui` commons barrel (comment-only, no export)
- Barlow woff2 files confirmed in all 3 apps: 23–24 KB per weight
- 12 original Barlow TTF files deleted
- TypeScript checks clean across all 3 apps
- All new tests pass: 16 total (4 billing + 6 layout + 3 geolocation + 7 render = 20 tests across Phase 2)

### Recommendation #5 Status

Deferred — AuthKit route group scoping excluded from Phase 2 pending stakeholder review.
