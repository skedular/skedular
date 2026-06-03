# Research: Web App Performance Optimization Audit (022)

**Phase**: 0 — Research  
**Feature**: `022-ssr-rendering-audit`  
**Produced by**: `/speckit.plan`  
**Date**: 2026-06-03

---

## 1. Route Inventory

### webapp (`src/web/apps/webapp`)

| Route                                                                | Pattern                                                              |
| -------------------------------------------------------------------- | -------------------------------------------------------------------- |
| `/`                                                                  | Root — dispatches to private org, co-working, or marketplace landing |
| `/auth/signin`                                                       | Sign-in                                                              |
| `/auth/signup`                                                       | Sign-up                                                              |
| `/install-slack`                                                     | Slack OAuth install                                                  |
| `/slack-success-install`                                             | Slack install success                                                |
| `/welcome`                                                           | Post-onboarding welcome                                              |
| `/notifications`                                                     | Notifications list                                                   |
| `/settings`                                                          | User/org settings                                                    |
| `/marketplace/bookings`                                              | My bookings list                                                     |
| `/marketplace/bookings/[bookingId]`                                  | Booking detail                                                       |
| `/marketplace/subscriptions`                                         | My subscriptions                                                     |
| `/marketplace/subscriptions/[subscriptionId]`                        | Subscription detail                                                  |
| `/marketplace/locations/[locationId]`                                | Location landing                                                     |
| `/marketplace/locations/[locationId]/floorPlans`                     | Floor plan browser                                                   |
| `/marketplace/products/[productId]`                                  | Product detail                                                       |
| `/marketplace/products/[productId]/book`                             | Booking flow                                                         |
| `/marketplace/products/[productId]/subscribe`                        | Subscription flow                                                    |
| `/marketplace/products/[productId]/bookings/[bookingId]`             | Product-scoped booking detail                                        |
| `/marketplace/organizations/[domain]`                                | Org storefront landing                                               |
| `/marketplace/organizations/[domain]/page`                           | Org page                                                             |
| `/marketplace/organizations/[domain]/products/[productId]`           | Product in org context                                               |
| `/marketplace/organizations/[domain]/products/[productId]/book`      | Booking in org context                                               |
| `/marketplace/organizations/[domain]/products/[productId]/subscribe` | Subscription in org context                                          |
| `/marketplace/organizations/[domain]/bookings`                       | Org-scoped bookings                                                  |
| `/marketplace/organizations/[domain]/bookings/[bookingId]`           | Org-scoped booking detail                                            |
| `/marketplace/organizations/[domain]/subscriptions`                  | Org-scoped subscriptions                                             |
| `/marketplace/organizations/[domain]/subscriptions/[subscriptionId]` | Org-scoped subscription detail                                       |
| `/marketplace/unsupported/[[...path]]`                               | Unsupported catch-all                                                |

**webapp total routes**: 28

### webapp-teams (`src/web/apps/webapp-teams`)

Teams app duplicates the `organizations/[domain]/…` tree twice: under `/organizations/…` (browser) and `/msteams/organizations/…` (Teams iframe). Additional routes for Teams install, SSO, analytics, teams/users admin.

**webapp-teams total routes**: ~55 (split between `/organizations/` and `/msteams/organizations/` branches)

### webapp-spaces (`src/web/apps/webapp-spaces`)

Mirrors webapp-teams structure with spaces-specific admin pages (product-tags, zones, resources).

**webapp-spaces total routes**: ~60

---

## 2. `'use client'` Spread

| App              | Files with `'use client'` in `src/app/` |
| ---------------- | --------------------------------------- |
| webapp           | 29                                      |
| webapp-teams     | 65                                      |
| webapp-spaces    | 74                                      |
| @skedular/ui     | 42                                      |
| @skedular/shared | 18                                      |

**Decision**: The global client boundary (`ClientRootLayout`) forces nearly all route files to render client-side regardless of whether they actually need interactivity. Pushing the boundary lower would free most route shells to become Server Components.

---

## 3. Global Client Boundary Architecture

### Current layering (webapp)

```
layout.tsx (Server Component ✅)
└── ClientRootLayout ('use client')           ← global boundary
    ├── Microanalytics <Script>
    ├── AppRouterCacheProvider (MUI SSR)
    │   └── PaletteModeProvider
    │       └── InnerRootLayout
    │           ├── ThemeProvider
    │           ├── DatePickerLocalizationProvider
    │           ├── AuthKitProvider
    │           └── AuthenticatedRelayProvider   ← Relay + auth
    │               └── {children}              ← ALL app routes here
    ├── Analytics / SpeedInsights (Vercel)
    ├── MuiXLicense
    ├── ToastContainer
    ├── GoogleAnalytics / GoogleTagManager
    └── LogRocketProvider
```

### Key findings

- **`layout.tsx` is already a Server Component** — it uses `next/font/local` and `next/headers` correctly.
- **`ClientRootLayout` owns the global boundary** — wrapping all children in `'use client'` because it needs `useContext(PaletteModeContext)` and `useAuth()`.
- **`AuthenticatedRelayProvider`** uses `useContext(InMsTeamsContext)` and `useAuth()` — both browser hooks. The provider itself cannot be a Server Component.
- **`relay-environment.ts` already has server-side support** — `getEnvironment()` detects `isServer` and creates a separate environment with `isServer: true`. The infrastructure is ready for server-side Relay.
- **AuthKitProvider requires client context** — WorkOS `useAuth()` is a client hook; auth state is only available after hydration.
- **webapp-teams adds `InMsTeamsProvider` and `TeamsUserCredential`** — MS Teams token acquisition is async and browser-only; `msteams/` routes cannot preload Relay queries server-side without architectural changes.

### Rationale for boundary minimization approach

The entire `InnerRootLayout` subtree (ThemeProvider → AuthKitProvider → RelayProvider → children) must remain client-rendered due to auth state dependency. However:

- **Static / public marketplace pages** (e.g., `/marketplace/organizations/[domain]`) could be ISR-rendered with a client shell for personalized content.
- **Analytics providers** (LogRocket, Vercel Analytics, Google Analytics) are already rendered outside the children stream and do not block routes.
- **`PaletteModeProvider`** could potentially be restructured to allow Server Component rendering above the theme boundary.

---

## 4. Heavy Dependency Map

| Library                                         | Usage                                     | Bundle Impact              | Lazy-Load Candidate                                                                       |
| ----------------------------------------------- | ----------------------------------------- | -------------------------- | ----------------------------------------------------------------------------------------- |
| `react-leaflet` / `leaflet`                     | Map browser in location/marketplace pages | ~200 KB                    | ✅ Yes — currently uses `dynamicLoadReady` state flag instead of `next/dynamic`           |
| `@mui/x-charts`                                 | Analytics insight charts                  | ~300 KB                    | ✅ Yes — used only in analytics/insight components                                        |
| `@mui/x-data-grid(-pro/-premium)`               | Data tables in admin pages                | ~400 KB                    | ✅ Yes — admin-only, not in main navigation flow                                          |
| `@stripe/react-stripe-js` + `@stripe/stripe-js` | Payment form                              | ~100 KB                    | ✅ Yes — only loaded in payment/checkout flows                                            |
| `logrocket`                                     | Session recording                         | ~60 KB                     | ⚠️ Partially — already conditionally rendered but eagerly imported in `LogRocketProvider` |
| `@vercel/analytics` + `@vercel/speed-insights`  | Analytics                                 | ~20 KB                     | ⚠️ Loaded outside children tree; acceptable but could be deferred                         |
| `react-toastify`                                | Toast notifications                       | ~30 KB                     | ⚠️ Global — difficult to split; small impact                                              |
| `globalize`                                     | Localization                              | ~40 KB                     | ✅ Assess usage scope                                                                     |
| `date-fns`                                      | Date utilities                            | ~70 KB (with tree-shaking) | ✅ Ensure tree-shaking is applied                                                         |
| `countries-list`                                | Country data                              | ~20 KB                     | ✅ Assess if needed globally                                                              |

**Decision**: Map rendering (leaflet), charts (@mui/x-charts), and data grids (@mui/x-data-grid) are the highest-impact lazy-load opportunities.

---

## 5. Image Optimization Findings

| Finding                    | Count   | Details                                                                                      |
| -------------------------- | ------- | -------------------------------------------------------------------------------------------- |
| `next/image <Image>` usage | 47      | Good adoption — most images use the optimized component                                      |
| Raw `<img>` tags           | 4       | Should be converted to `<Image>`                                                             |
| `priority` prop usage      | 0       | **Critical** — no above-the-fold images declare `priority`; LCP images will be deprioritized |
| `sizes` prop usage         | Unknown | Needs audit pass                                                                             |

**Decision**: Adding `priority` to LCP images (org logos, product hero images, location images) is a low-effort, high-impact quick win.

---

## 6. Font Loading Assessment

| Finding                     | Status                                                                           |
| --------------------------- | -------------------------------------------------------------------------------- |
| `next/font/local` used      | ✅ Optimal                                                                       |
| `display: 'swap'` set       | ✅ No blocking                                                                   |
| `adjustFontFallback: false` | ✅ Prevents layout shift                                                         |
| Font formats                | ⚠️ Barlow loaded as TTF (4 weights); InterVariable as woff2                      |
| Barlow format               | ⚠️ TTF is ~2–3× larger than woff2; converting to woff2 would reduce font payload |
| Subsetting                  | Unknown — needs assessment                                                       |

**Decision**: Converting Barlow fonts from TTF to woff2 is a moderate-effort improvement. Inter is already woff2.

---

## 7. Static / ISR Potential

| Route Type                                                 | Candidate?                   | Rationale                                                               |
| ---------------------------------------------------------- | ---------------------------- | ----------------------------------------------------------------------- |
| `/marketplace/organizations/[domain]`                      | ✅ ISR candidate             | Public storefront content changes infrequently; could revalidate hourly |
| `/marketplace/organizations/[domain]/products/[productId]` | ✅ ISR candidate             | Product descriptions change on operator update, not per-user            |
| `/marketplace/locations/[locationId]`                      | ✅ ISR candidate             | Location data is stable                                                 |
| `/marketplace/products/[productId]`                        | ✅ ISR candidate             | Same as org-scoped product                                              |
| `/auth/signin`, `/auth/signup`, `/welcome`                 | ✅ Static candidate          | No dynamic data                                                         |
| `/marketplace/bookings`, `/marketplace/subscriptions`      | ❌ Must stay dynamic         | Per-user data, already uses `force-dynamic`                             |
| `/organizations/[domain]/...` (admin)                      | ❌ Must stay dynamic         | Requires authenticated user session                                     |
| MS Teams routes (`/msteams/...`)                           | ❌ Must stay client-rendered | Token acquisition is async browser-only                                 |

**Decision**: Public marketplace and product pages are strong ISR candidates. Zero uses of `generateStaticParams` or `revalidate` currently — this is a major optimization gap.

---

## 8. Relay Query Classification (Architecture Assessment)

### Summary findings

- **`relay-environment.ts` already creates server-side environments** with `isServer: true`. Infrastructure foundation exists.
- **All Relay providers are `'use client'`** — `RelayProvider` uses `useContext` and `RelayEnvironmentProvider` which is a React context consumer.
- **Auth token required for most queries** — `useAuth()` from WorkOS is client-side only; without a server-side session cookie approach, authenticated queries cannot move server-side without changes to the auth architecture.
- **Public marketplace data** (org storefront, product listings, location details) could be pre-fetched server-side without auth.
- **Relay preloaded queries** (`usePreloadedQuery`) — not currently in use; this is an optimization opportunity.

### Relay server-side path feasibility

- **Short-term (no auth change)**: Public GraphQL data (marketplace listing, product details) can be fetched server-side using plain `fetch` in Server Components and passed as props.
- **Medium-term**: Implementing WorkOS server-side session access (cookie-based) would unlock server-side authenticated Relay prefetch.
- **Architectural constraint**: `AuthenticatedRelayProvider` depends on `useAuth()` → cannot be a Server Component without rearchitecting auth flow.

---

## 9. Bundle Analysis Tooling

- **`@next/bundle-analyzer` is NOT installed** in any of the three apps.
- All three apps use the same `next.config.ts` structure and can adopt the analyzer with the same pattern.
- The analyzer requires a `ANALYZE=true` environment variable trigger.
- **Recommendation**: Install `@next/bundle-analyzer` as a dev dependency in each app and wrap `next.config.ts`.

---

## 10. Suspense / Streaming Assessment

- 14 `<Suspense>` uses in webapp — partial streaming adoption.
- Most route pages lack `<Suspense>` wrappers, preventing Next.js from streaming HTML progressively.
- React 19 concurrent features (automatic batching, transitions) are available but not fully leveraged.

---

## 11. Shared Package SSR Compatibility

### @skedular/ui (42 files with `'use client'`)

- All interactive/styled components require `'use client'` due to MUI dependencies.
- Pure layout primitives and typography wrappers may be server-safe if they avoid hooks.
- Needs export-level audit.

### @skedular/shared (18 files with `'use client'`)

- Providers (`ThemeProvider`, `RelayProvider`, `AuthenticatedRelayProvider`, `InMsTeamsProvider`) are all `'use client'`.
- Utilities and non-hook exports are likely server-safe.
- Needs export-level audit.

---

## 12. Key Resolved Decisions

| Unknown                                   | Decision                                                                     | Rationale                                                                         |
| ----------------------------------------- | ---------------------------------------------------------------------------- | --------------------------------------------------------------------------------- |
| Can Relay move server-side?               | Partially — public data yes, authenticated data requires auth rearchitecture | relay-environment already has `isServer: true` support; auth token is the blocker |
| Is bundle analyzer installed?             | No — must be added as dev dependency to all three apps                       | Not found in any package.json                                                     |
| Are there ISR/static routes?              | Zero currently — large gap                                                   | No `generateStaticParams`, `revalidate`, or `unstable_cache` usage found          |
| What is the laziest win for bundle size?  | Leaflet maps + @mui/x-charts + @mui/x-data-grid families                     | Largest single-component bundle impacts                                           |
| Are raw `<img>` tags a problem?           | Minor (only 4) but LCP `priority` is the critical gap                        | All 47 `<Image>` usages lack `priority`                                           |
| Are font formats optimal?                 | Barlow in TTF needs woff2 conversion; Inter already woff2                    | TTF is significantly larger                                                       |
| Can `layout.tsx` stay a Server Component? | Yes — currently is, no 'use client'                                          | Server Component boundary is already preserved at layout level                    |
