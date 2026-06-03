# Component Audit — Location, Map, Floor Plan, Resources

**Generated**: 2026-06-03  
**Task**: T016  
**Scope**: `location`, `floorPlan`, `resource`, `resourceType`, `zone`, `search`

---

## location (20 components — 0 with `'use client'`)

All 20 location components are **server-compatible** at the component level.

| Sub-category             | Count | Notes                                                                            |
| ------------------------ | ----- | -------------------------------------------------------------------------------- |
| `addLocation/`           | ~3    | Add location forms; display shell is SSR-compatible, submit handler needs client |
| `marketplaceLocation/`   | ~4    | Map display + popup; raw `<img>` found (see asset-findings.md)                   |
| `marketplaceLocations/`  | ~5    | Location list; **critical**: contains `node-ipinfo` import                       |
| `organizationLocation/`  | ~4    | Admin location view; display = SSR-compatible                                    |
| `organizationLocations/` | ~4    | Location card list; SSR-compatible                                               |

### Critical: `marketplace-locations.tsx` (node-ipinfo)

```typescript
// src/web/apps/webapp/src/components/location/marketplaceLocations/marketplace-locations.tsx:15
import { IPinfoWrapper } from "node-ipinfo";
```

`node-ipinfo` is a Node.js library being imported in a client component. This:

1. Loads 41 KB of Node.js IP library into the browser bundle
2. Exposes IP geolocation API access patterns to client-side code
3. Should be moved to a Server Action or API route

**Fix**: Replace with `fetch('/api/geolocation')` server action that calls `node-ipinfo` server-side.

### Critical: `marketplace-location.tsx` (raw `<img>`)

Uses raw `<img>` tags for location photos in the map popup. Replace with `next/image`.

---

## floorPlan (6 components — 2 with `'use client'`)

| Component                       | `'use client'` | Reason                        | Notes                           |
| ------------------------------- | -------------- | ----------------------------- | ------------------------------- |
| Floor plan editor               | **yes**        | Canvas/drag-drop interaction  | Complex interactive editor      |
| Floor plan viewer (interactive) | **yes**        | Click/hover on floor plan SVG | Needs event handlers            |
| `add-floor-plan.tsx`            | no             | Form shell                    | Uses raw `<img>` for preview    |
| `edit-floor-plan.tsx`           | no             | Form shell                    | Uses raw `<img>` for preview    |
| `floor-plans.tsx`               | no             | List display                  | Uses raw `<img>` for thumbnails |
| Floor plan detail               | no             | Display                       | —                               |

**Raw `<img>` in floor plan components**: `add-floor-plan.tsx`, `edit-floor-plan.tsx`, `floor-plans.tsx` (present in all 3 apps) use raw `<img>` for floor plan image display. For uploaded floor plan images, `next/image` would provide lazy loading and size optimization.

---

## resource (3 components — 0 with `'use client'`)

| Component       | `'use client'` | Notes          |
| --------------- | -------------- | -------------- |
| Resource list   | no             | SSR-compatible |
| Resource card   | no             | SSR-compatible |
| Resource detail | no             | SSR-compatible |

---

## Summary for Location/Map/Floor Plan/Resources

| Category  | Total | Client | Server-compatible | Key Issue                                 |
| --------- | ----- | ------ | ----------------- | ----------------------------------------- |
| location  | 20    | 0      | 20                | `node-ipinfo` server migration (P2)       |
| floorPlan | 6     | 2      | 4                 | 2 correctly interactive; raw `<img>` in 3 |
| resource  | 3     | 0      | 3                 | SC-compatible                             |

**Key actions for this category**:

1. Move `node-ipinfo` usage to server-side API route (security + perf)
2. Replace raw `<img>` in floor plan components with `next/image`
3. Leaflet map already uses dynamic import — verify chunk splitting in bundle

**Leaflet verification**: Maps use `dynamic(() => import('react-leaflet'))` but leaflet (177 KB) still appears in the client bundle. This may mean:

- The dynamic import is not at the right component boundary, OR
- Leaflet is also statically imported somewhere in the component tree
  Run a focused bundle analysis on the `/marketplace/locations/*` routes to confirm.
