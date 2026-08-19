# webapp-teams — Route Audit

**Generated**: 2026-06-03  
**Total routes**: 64 (all `'use client'`)  
**All routes**: `ƒ` Dynamic (server-rendered on demand)

## Summary Statistics

| Metric                                   | Count                    |
| ---------------------------------------- | ------------------------ |
| Total routes                             | 64                       |
| `'use client'` pages                     | 64 (100%)                |
| Server Component pages                   | 0                        |
| `export const dynamic = 'force-dynamic'` | 0                        |
| `force-dynamic` equivalent (SSO/iframe)  | most `/msteams/*` routes |
| Fully public routes (no auth)            | ~3                       |
| Static/ISR candidates                    | 0                        |

**Primary SSR blockers**:

1. `useAuth()` from WorkOS — same as webapp
2. MS Teams iframe context (`InMsTeamsContext`, `useInMsTeams()`) — async token acquisition
3. `@azure/msal-browser` + `@azure/msal-common` (123K combined) loaded for all `/msteams/*` routes

---

## Route Groups

### MS Teams Embedded Routes (`/msteams/*`) — 36 routes

All rendered inside MS Teams iframe. All `'use client'`.  
Constraint: Cannot use synchronous auth; token acquisition is async via MSAL.

| Route                                                                                             | Auth       | ISR/Static | Notes                     |
| ------------------------------------------------------------------------------------------------- | ---------- | ---------- | ------------------------- |
| `/msteams`                                                                                        | no         | no         | Entry point for Teams tab |
| `/msteams/install-msteams`                                                                        | no         | no         | Install flow              |
| `/msteams/notifications`                                                                          | yes (MSAL) | no         | Notification hub          |
| `/msteams/organizations/[organizationCustomDomain]`                                               | yes (MSAL) | no         | Org dashboard             |
| `/msteams/organizations/[organizationCustomDomain]/admin`                                         | admin      | no         | Admin panel               |
| `/msteams/organizations/[organizationCustomDomain]/admin/tags/[customTagId]/edit`                 | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/admin/tags/add`                                | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/admin/zones/[zoneId]/edit`                     | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/admin/zones/add`                               | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/analytics`                                     | admin      | no         | Charts/data               |
| `/msteams/organizations/[organizationCustomDomain]/bookings`                                      | yes        | no         | Booking list              |
| `/msteams/organizations/[organizationCustomDomain]/bookings/[bookingId]`                          | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/locations`                                     | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/locations/[locationId]`                        | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/locations/[locationId]/resources/[resourceId]` | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/locations/[locationId]/resources/add`          | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/locations/[locationId]/resources/bulk-add`     | admin      | no         | Bulk import               |
| `/msteams/organizations/[organizationCustomDomain]/locations/add-private`                         | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/resources/add`                                 | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/sso-signin`                                    | no         | no         | SSO entry                 |
| `/msteams/organizations/[organizationCustomDomain]/teams`                                         | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/teams/[teamId]`                                | yes        | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/teams/add`                                     | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/users`                                         | admin      | no         | —                         |
| `/msteams/organizations/[organizationCustomDomain]/users/[customerId]`                            | admin      | no         | —                         |
| `/msteams/organizations/add-private`                                                              | admin      | no         | —                         |
| `/msteams/settings`                                                                               | yes        | no         | Settings                  |
| `/msteams/start-install-msteams`                                                                  | no         | no         | Pre-install flow          |

### Standard (Non-Teams) Routes — 28 routes

Mirrors webapp-spaces non-msteams routes. All `'use client'`.

| Route                                                                                             | Auth  | ISR/Static | Notes             |
| ------------------------------------------------------------------------------------------------- | ----- | ---------- | ----------------- |
| `/auth/signin`                                                                                    | no    | no         | WorkOS auth       |
| `/auth/signup`                                                                                    | no    | no         | —                 |
| `/install-slack`                                                                                  | no    | no         | —                 |
| `/notifications`                                                                                  | yes   | no         | —                 |
| `/settings`                                                                                       | yes   | no         | Billing included  |
| `/welcome`                                                                                        | yes   | no         | Onboarding        |
| `/slack-success-install`                                                                          | no    | no         | —                 |
| `/organizations/[organizationCustomDomain]`                                                       | yes   | no         | Admin dashboard   |
| `/organizations/[organizationCustomDomain]/admin`                                                 | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/admin/tags/[customTagId]/edit`                         | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/admin/tags/add`                                        | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/admin/zones/[zoneId]/edit`                             | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/admin/zones/add`                                       | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/analytics`                                             | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/availability`                                          | admin | no         | Dashboard charts  |
| `/organizations/[organizationCustomDomain]/bookings`                                              | yes   | no         | —                 |
| `/organizations/[organizationCustomDomain]/bookings/[bookingId]`                                  | yes   | no         | —                 |
| `/organizations/[organizationCustomDomain]/bookings/add`                                          | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations`                                             | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]`                                | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/floorPlans`                     | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/floorPlans/add`                 | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/floorPlans/admin/[floorPlanId]` | admin | no         | Floor plan editor |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/resources/[resourceId]`         | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/resources/add`                  | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/locations/[locationId]/resources/bulk-add`             | admin | no         | Bulk import       |
| `/organizations/[organizationCustomDomain]/locations/add-private`                                 | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/resources/add`                                         | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/sso-signin`                                            | no    | no         | —                 |
| `/organizations/[organizationCustomDomain]/teams`                                                 | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/teams/[teamId]`                                        | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/teams/add`                                             | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/users`                                                 | admin | no         | —                 |
| `/organizations/[organizationCustomDomain]/users/[customerId]`                                    | admin | no         | —                 |
| `/organizations/add-private`                                                                      | admin | no         | —                 |

---

## Optimization Opportunities

### 1. SSR / Server Components

**Assessment**: Minimal SSR opportunity; slightly harder than webapp due to MS Teams constraints.

- All 64 routes are `'use client'`
- `/msteams/*` routes are embedded in MS Teams iframe — async MSAL token acquisition (`@azure/msal-browser`) is incompatible with synchronous SSR
- Standard routes share the same WorkOS `useAuth()` blocker as webapp
- **Additional blocker**: `InMsTeamsContext` and `useInMsTeams()` hook detection depends on `microsoftTeams.app.initialize()` which is browser-only

### 2. Static / ISR

**Assessment**: 0 candidates.

All routes require authenticated org context. No public-facing marketplace routes exist.

### 3. Lazy Loading

See `lazy-load-candidates.md`. Key teams-specific opportunities:

- `@azure/msal-browser` + `@azure/msal-common` (123K total) — loaded globally, only needed for `/msteams/*` routes. Standard `/organizations/*` routes don't need MSAL.
- `react-svg-credit-card-payment-icons` (521K) — same barrel issue as webapp
- `@mui/x-data-grid` (119K) — confirm which analytics routes actually use it

### 4. MS Teams Iframe Constraints

**Important**: These constraints affect Phase 2 optimization implementation:

- Cannot use `Suspense` boundaries that stall rendering in iframe context
- Token acquisition is async — any SSR approach must handle unauthenticated first render gracefully
- `InMsTeamsContext` is a client-only hook that returns `undefined` on server
- Animation/transition optimization may be limited by Teams iframe rendering quirks
