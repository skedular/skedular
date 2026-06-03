# Lazy-Load Candidates

**Generated**: 2026-06-03  
**Source data**: Bundle analysis + route audit + code search

---

## Summary

| Package                                               | Size (parsed) | Affected Apps | Impact     | Fix Effort            |
| ----------------------------------------------------- | ------------- | ------------- | ---------- | --------------------- |
| `react-svg-credit-card-payment-icons`                 | 521 KB        | All 3         | CRITICAL   | Medium                |
| `@azure/msal-browser` + `@azure/msal-common`          | 123 KB        | teams, spaces | High       | Medium                |
| `@mui/x-data-grid`                                    | 119 KB        | All 3         | Medium     | Low (verify)          |
| `leaflet` + `react-leaflet` + `leaflet.markercluster` | 177 KB        | webapp only   | Medium     | Low (already dynamic) |
| `logrocket`                                           | 61 KB         | All 3         | Low        | Low                   |
| `node-ipinfo`                                         | 41 KB         | webapp only   | Low-Medium | Medium (server API)   |
| `pino`                                                | 6 KB          | webapp only   | Low        | Low                   |

---

## LL-001: `react-svg-credit-card-payment-icons` — Barrel Contamination

**Priority**: P0 — Critical  
**Size**: 521 KB parsed / 180 KB gzip  
**Affected apps**: All 3 (webapp, webapp-teams, webapp-spaces)

### Problem

`@skedular/ui` barrel export forces this library into every route:

```
@skedular/ui/index.ts
  → export * from './commons'
  → @skedular/ui/commons/index.ts
    → export { CreditCard }
    → @skedular/ui/commons/credit-card.tsx
      → import { PaymentIcon } from 'react-svg-credit-card-payment-icons'
```

**Only 2 components actually use `CreditCard`**:

- `organization-admin-billing-payment-section.tsx` (billing settings)
- `organization-admin-subscriptions-section.tsx` (subscriptions view)

### Fix Options (in order of preference)

**Option A: Remove `CreditCard` from commons barrel** (recommended, Low effort)  
Remove `export { CreditCard }` from `@skedular/ui/commons/index.ts`. The 2 consumers import it directly from `@skedular/ui/commons/credit-card` instead.

- No runtime change
- Eliminates 521 KB from all routes that don't need billing UI
- Risk: Other consumers not found by search — verify with TypeScript compiler (`tsc --noEmit`)

**Option B: Dynamic import in CreditCard component** (Medium effort)  
Replace static import in `credit-card.tsx` with `dynamic(() => import('react-svg-credit-card-payment-icons'), { ssr: false })`.

- Defers 521 KB to first billing-related render
- Complexity: `react-svg-credit-card-payment-icons` exports SVG icons, not components — needs wrapper

**Option C: Remove `react-svg-credit-card-payment-icons` entirely** (High effort)  
Replace with inline SVG or `@mui/icons-material` for the supported card types.

- Eliminates dependency entirely
- Most resilient solution but requires design review

**Recommendation**: Option A immediately. Option C if card icons need a redesign pass.

---

## LL-002: `@azure/msal-browser` + `@azure/msal-common` — Non-Teams Routes

**Priority**: P1 — High  
**Size**: 123 KB combined (72 KB msal-browser + 51 KB msal-common)  
**Affected apps**: webapp-teams, webapp-spaces

### Problem

MSAL is loaded globally in webapp-teams and webapp-spaces. However, MSAL is only needed for `/msteams/*` routes (MS Teams iframe). The standard `/organizations/*` routes use WorkOS AuthKit instead.

### Current Import Path

MSAL is likely imported in the Teams-specific provider or hook (e.g., `InMsTeamsContext`, `MsalProvider`). Exact import path should be traced in Phase 2.

### Fix

Wrap MSAL imports in a dynamic import with `ssr: false` in the component/provider that handles MS Teams auth:

```typescript
// Before (in InMsTeamsContext or equivalent)
import { PublicClientApplication } from "@azure/msal-browser";

// After
const { PublicClientApplication } = await import("@azure/msal-browser");
// or use dynamic() for React components wrapping MsalProvider
```

**Estimated saving**: 123 KB removed from initial bundle for non-Teams routes (any route not under `/msteams/*`).

---

## LL-003: `@mui/x-data-grid` — Type-Only Import Verification

**Priority**: P2 — Medium  
**Size**: 119 KB parsed  
**Affected apps**: All 3

### Problem

`@skedular/shared/src/mui/index.ts` contains:

```typescript
import type { GridRowSelectionModel } from "@mui/x-data-grid";
```

This is a TypeScript `import type` — it should be tree-shaken by webpack/TypeScript. However, `@mui/x-data-grid` still appears in the bundle analyzer output at ~119 KB.

### Investigation Needed

- Verify whether there are _runtime_ (non-type) imports of `@mui/x-data-grid` elsewhere in the shared packages or apps
- Check if `@skedular/shared` exports `GridRowSelectionModel` as a type re-export that accidentally triggers tree inclusion

### Fix (if confirmed runtime import)

1. Find all runtime imports of `@mui/x-data-grid` in shared packages and apps
2. Add route-level dynamic import: `const DataGrid = dynamic(() => import('@mui/x-data-grid').then(m => ({ default: m.DataGrid })), { ssr: false })`
3. Only load on analytics/admin routes that actually use data grids

---

## LL-004: `leaflet` + `react-leaflet` + `leaflet.markercluster` — Map Components

**Priority**: P2 — Medium  
**Size**: 177 KB total (144 KB leaflet + 33 KB react-leaflet)  
**Affected apps**: webapp only (3 routes)

### Problem

Map-related packages appear in the client bundle despite the code using dynamic imports. Likely the dynamic import is correctly declared but the chunk is still eagerly included in the initial JS bundle.

### Affected Routes

- `/marketplace/locations/[locationId]` — main map view
- `/marketplace/locations/[locationId]/floorPlans` — floor plan with map
- `/` (home) — small map component on landing

### Current Code Pattern

```typescript
// marketplace-locations.tsx (approximate)
const MapComponent = dynamic(() => import("../map/map-component"), { ssr: false });
```

### Fix

Verify the dynamic import boundary is at the correct level:

1. Ensure no static imports of leaflet anywhere in the component chain
2. The `node-ipinfo` import in `marketplace-locations.tsx` forces this component to be bundled client-side — moving it server-side (LL-006) will also help here
3. Verify `.next/analyze/client.html` chunk grouping — leaflet may be correctly code-split but still shown in aggregate

---

## LL-005: `logrocket` — Deferred Analytics Init

**Priority**: P3 — Low  
**Size**: 61 KB parsed  
**Affected apps**: All 3

### Problem

LogRocket is initialized synchronously in the global client layout. It doesn't need to be part of the critical render path.

### Fix

Defer LogRocket initialization to after the page is interactive using `requestIdleCallback` or a `useEffect` with a delay:

```typescript
// In LogRocketProvider
useEffect(() => {
  const init = () => LogRocket.init(process.env.NEXT_PUBLIC_LOGROCKET_APP_ID!);

  if ("requestIdleCallback" in window) {
    requestIdleCallback(init);
  } else {
    setTimeout(init, 0);
  }
}, []);
```

This keeps LogRocket in the bundle (no lazy load needed) but defers initialization so it doesn't block rendering.

---

## LL-006: `node-ipinfo` — Move to Server-Side API

**Priority**: P2 — Medium  
**Size**: 41 KB parsed  
**Affected apps**: webapp only

### Problem

`node-ipinfo` is a Node.js library imported in a client component (`marketplace-locations.tsx`):

```typescript
import { IPinfoWrapper } from "node-ipinfo"; // Line 15
```

This is a security concern as well as a performance issue — IP geolocation should be done server-side:

1. It exposes the API key to the client bundle
2. `node-ipinfo` should not run in browsers (Node.js API used)
3. 41 KB loaded client-side unnecessarily

### Fix

Replace with a Next.js API route or Server Action:

```typescript
// app/api/geolocation/route.ts (server-side)
import { IPinfoWrapper } from "node-ipinfo";
// Runs server-side only — no bundle impact
```

Then `marketplace-locations.tsx` fetches from `/api/geolocation` instead.

---

## LL-007: `pino` — Browser-Safe Logger

**Priority**: P3 — Low  
**Size**: 6 KB parsed  
**Affected apps**: webapp only

### Problem

`pino` is a Node.js JSON logger imported in `src/libs/logging/index.ts`. While pino has a browser mode, using it in client code is non-standard and may cause issues.

### Fix

Replace with `console.*` wrapper in the client logging util, or use `pino/browser` export explicitly:

```typescript
// Instead of: import pino from 'pino'
// Use:
const logger = process.env.NODE_ENV === "development" ? console : { info: () => {}, warn: () => {}, error: console.error };
```

---

## Priority Matrix

| #   | Package                                          | Bundle Saving   | Effort | Priority |
| --- | ------------------------------------------------ | --------------- | ------ | -------- |
| 1   | `react-svg-credit-card-payment-icons` barrel fix | 521 KB × 3 apps | Medium | **P0**   |
| 2   | MSAL lazy load (teams/spaces)                    | 123 KB          | Medium | **P1**   |
| 3   | `node-ipinfo` → server API                       | 41 KB           | Medium | **P2**   |
| 4   | Verify `@mui/x-data-grid` tree-shaking           | 119 KB          | Low    | **P2**   |
| 5   | Barlow TTF → woff2                               | 106 KB          | Low    | **P2**   |
| 6   | Leaflet dynamic import verification              | 177 KB          | Low    | **P2**   |
| 7   | LogRocket deferred init                          | 0 KB (perf)     | Low    | **P3**   |
| 8   | `pino` → browser logger                          | 6 KB            | Low    | **P3**   |
