# webapp — Route Audit

**Generated**: 2026-06-03  
**Total routes**: 28 (26 `'use client'` + 2 server shell with client delegation)  
**All routes**: `ƒ` Dynamic (server-rendered on demand, no static prerendering)

## Summary Statistics

| Metric                                   | Count                                    |
| ---------------------------------------- | ---------------------------------------- |
| Total routes                             | 28                                       |
| `'use client'` pages                     | 26                                       |
| Server Component pages                   | 2 (both delegate to client-page.tsx)     |
| `export const dynamic = 'force-dynamic'` | 2                                        |
| Fully public routes (no auth)            | 5                                        |
| Partial-public routes (auth-conditional) | ~20                                      |
| Auth-gated routes (login required)       | ~3                                       |
| Static/ISR candidates                    | 0                                        |
| Relay query roots                        | 4 direct (others delegate to components) |

**Primary SSR blocker**: `useAuth()` from `@workos-inc/authkit-nextjs/components` — used in 23/28 routes either directly or through shared root shell components.

---

## Route Classification Table

| Route                                                                                 | Boundary      | Auth        | Relay | Public  | SSR Blocker                          | ISR/Static | Heavy Imports                            | Notes                                     |
| ------------------------------------------------------------------------------------- | ------------- | ----------- | ----- | ------- | ------------------------------------ | ---------- | ---------------------------------------- | ----------------------------------------- |
| `/`                                                                                   | client        | conditional | yes   | partial | `useAuth()`                          | no         | leaflet (177K)                           | Marketplace home; map view                |
| `/auth/signin`                                                                        | client        | no          | yes   | yes     | client auth components               | no         | none                                     | Uses preloaded query; org custom domain   |
| `/auth/signup`                                                                        | client        | no          | yes   | yes     | client auth components               | no         | none                                     | Uses preloaded query                      |
| `/callback`                                                                           | client        | no          | no    | yes     | none                                 | no         | none                                     | Auth callback handler                     |
| `/welcome`                                                                            | client        | yes         | yes   | no      | `useAuth()`                          | no         | none                                     | Onboarding flow                           |
| `/notifications`                                                                      | client        | yes         | no    | no      | auth shell                           | no         | none                                     | Simple notification list                  |
| `/settings`                                                                           | client        | yes         | no    | no      | auth shell                           | no         | react-svg-credit-card (521K via billing) | Delegates to settings components          |
| `/install-slack`                                                                      | client        | no          | no    | yes     | none                                 | no         | none                                     | Pure UI                                   |
| `/slack-success-install`                                                              | client        | no          | no    | yes     | `useSearchParams()`                  | no         | none                                     | Query string dependent                    |
| `/marketplace/bookings`                                                               | server→client | yes         | no    | partial | `useAuth()` + `dynamic({ssr:false})` | no         | none                                     | `force-dynamic`, custom domain resolution |
| `/marketplace/bookings/[bookingId]`                                                   | client        | yes         | no    | partial | `useAuth()`                          | no         | none                                     | Booking detail                            |
| `/marketplace/subscriptions`                                                          | server→client | yes         | no    | partial | `useAuth()` + `dynamic({ssr:false})` | no         | none                                     | `force-dynamic`, custom domain            |
| `/marketplace/subscriptions/[subscriptionId]`                                         | client        | yes         | no    | partial | `useAuth()`                          | no         | none                                     | Subscription detail                       |
| `/marketplace/locations/[locationId]`                                                 | client        | conditional | yes   | partial | `useAuth()`, `useContext`            | no         | leaflet (177K)                           | Map + preloaded query                     |
| `/marketplace/locations/[locationId]/floorPlans`                                      | client        | conditional | no    | partial | `useAuth()`                          | no         | leaflet (177K)                           | Shares map component                      |
| `/marketplace/products/[productId]`                                                   | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | Product detail                            |
| `/marketplace/products/[productId]/book`                                              | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | Booking flow                              |
| `/marketplace/products/[productId]/subscribe`                                         | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | Subscription flow                         |
| `/marketplace/products/[productId]/bookings/[bookingId]`                              | client        | yes         | no    | partial | `useAuth()`                          | no         | none                                     | Booking detail                            |
| `/marketplace/unsupported/[[...path]]`                                                | client        | no          | no    | yes     | none                                 | no         | none                                     | Catch-all fallback                        |
| `/marketplace/organizations/[customDomain]`                                           | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | Custom domain storefront                  |
| `/marketplace/organizations/[customDomain]/bookings`                                  | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/bookings/[bookingId]`                      | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/subscriptions`                             | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/subscriptions/[subscriptionId]`            | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/products/[productId]`                      | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/products/[productId]/book`                 | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/products/[productId]/subscribe`            | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |
| `/marketplace/organizations/[customDomain]/products/[productId]/bookings/[bookingId]` | client        | conditional | no    | partial | `useAuth()`                          | no         | none                                     | —                                         |

---

## Optimization Opportunities

### 1. SSR / Server Components

**Assessment**: Near-zero SSR opportunity for this app today.

- `useAuth()` from WorkOS is used in every meaningful route via `UnauthenticatedRootShell`, `NoOrganizationRootShell`, and similar shared shells.
- The 2 `force-dynamic` routes (`/marketplace/bookings`, `/marketplace/subscriptions`) cannot be statically prerendered because they read `window.location.hostname` at runtime for custom-domain organization resolution.
- **Theoretical opportunity**: The 5 fully public routes (`/auth/signin`, `/auth/signup`, `/install-slack`, `/slack-success-install`, `/marketplace/unsupported`) could have server-rendered shells. Blocked by the shared auth provider wrapping the whole app.
- **High-value direction**: If `AuthKitProvider` were moved from the root layout to a route group layout (wrapping only auth-required routes), public routes could become server-rendered. This is a significant architecture change tracked in `relay-queries.md`.

### 2. Static / ISR

**Assessment**: 0 candidates.

- All routes are either auth-gated, runtime-user-dependent, or custom-domain-dependent.
- Even the org storefront pages (`/marketplace/organizations/[customDomain]`) read hostname at runtime.
- **Note**: If the custom-domain resolution moved to middleware (rewrites), the storefront pages COULD be ISR'd per-domain. This is an architectural opportunity but requires significant work.

### 3. Lazy Loading

See `lazy-load-candidates.md` for full analysis. Routes affected:

- 3 routes load the full leaflet+react-leaflet bundle (~177K) — already using dynamic import pattern but it's still in the client bundle
- All routes (via `@skedular/ui` barrel) load `react-svg-credit-card-payment-icons` (521K) — **critical barrel import issue**

### 4. Bundle Contamination

**Critical finding**: `@skedular/ui` barrel export chain forces `react-svg-credit-card-payment-icons` (521K parsed / 180K gzip) into every route.

Chain: `@skedular/ui/index.ts` → `export * from './commons'` → `export { CreditCard }` → `import { PaymentIcon } from 'react-svg-credit-card-payment-icons'`

`CreditCard` is only used on billing/settings pages. Fix: remove from commons barrel or make `PaymentIcon` a dynamic import.

### 5. Server-Side-Only Code in Client Bundle

| Package             | File                        | Issue                                                                           |
| ------------------- | --------------------------- | ------------------------------------------------------------------------------- |
| `pino` (6K)         | `src/libs/logging/index.ts` | Node.js logger in client bundle — should use browser-safe logger or conditional |
| `node-ipinfo` (41K) | `marketplace-locations.tsx` | Node.js IP geolocation lib in client bundle — should be server-side API call    |

---

## rootPages Classification

| rootPage                                                           | Boundary   | Relay                    | Notes                            |
| ------------------------------------------------------------------ | ---------- | ------------------------ | -------------------------------- |
| `rootPages/page.tsx` (home)                                        | server     | yes (`useLazyLoadQuery`) | Loads initial location data      |
| `rootPages/welcome/page.tsx`                                       | server     | yes                      | Onboarding; loads relay          |
| `rootPages/notifications/page.tsx`                                 | server     | no                       | Simple; no relay                 |
| `rootPages/settings/page.tsx`                                      | server     | no                       | Delegates to settings components |
| `rootPages/install-slack/page.tsx`                                 | server     | no                       | Static content                   |
| `rootPages/slack-success-install/page.tsx`                         | server     | no                       | Query param dependent            |
| `rootPages/marketplace/page.tsx`                                   | server     | no                       | Storefront shell                 |
| `rootPages/marketplace/bookings/page.tsx`                          | **client** | no                       | `useAuth()` required             |
| `rootPages/marketplace/bookings/booking/page.tsx`                  | server     | no                       | —                                |
| `rootPages/marketplace/subscriptions/page.tsx`                     | **client** | no                       | `useAuth()` required             |
| `rootPages/marketplace/subscriptions/subscription/page.tsx`        | server     | no                       | —                                |
| `rootPages/marketplace/products/product/page.tsx`                  | server     | no                       | Product detail                   |
| `rootPages/marketplace/products/product/book/page.tsx`             | server     | no                       | —                                |
| `rootPages/marketplace/products/product/subscribe/page.tsx`        | server     | no                       | —                                |
| `rootPages/marketplace/products/product/bookings/booking/page.tsx` | server     | no                       | —                                |
| `rootPages/marketplace/locations/location/page.tsx`                | server     | yes                      | Relay query load                 |

**Note**: rootPages that are Server Components (`S`) are still wrapped by client-component `app/page.tsx` files (which have `'use client'`), so the effective boundary is determined by the `app/` wrapper.
