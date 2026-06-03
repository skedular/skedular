# webapp-spaces — Route Audit

**Generated**: 2026-06-03  
**Total routes**: 74 (73 `'use client'`, 1 server shell with client delegation)  
**All routes**: `ƒ` Dynamic (server-rendered on demand)

## Summary Statistics

| Metric                                   | Count                                                              |
| ---------------------------------------- | ------------------------------------------------------------------ |
| Total routes                             | 74                                                                 |
| `'use client'` pages                     | 73                                                                 |
| Server Component pages                   | 1 (`/organizations/[customDomain]/subscriptions/[subscriptionId]`) |
| `export const dynamic = 'force-dynamic'` | 0                                                                  |
| Fully public routes                      | ~3                                                                 |
| Static/ISR candidates                    | 0                                                                  |

**Primary SSR blockers**: Same as webapp-teams — `useAuth()` from WorkOS + MS Teams MSAL constraints.

---

## Route Groups

### MS Teams Embedded Routes (`/msteams/*`) — ~30 routes

Similar structure to webapp-teams `/msteams/*` routes but includes marketplace-specific routes.

| Route                                                                                 | Auth  | Notes                          |
| ------------------------------------------------------------------------------------- | ----- | ------------------------------ |
| `/msteams`                                                                            | no    | Entry                          |
| `/msteams/install-msteams`                                                            | no    | —                              |
| `/msteams/organizations/[customDomain]`                                               | yes   | —                              |
| `/msteams/organizations/[customDomain]/admin`                                         | admin | —                              |
| `/msteams/organizations/[customDomain]/admin/product-tags/[productTagId]/edit`        | admin | Product tags (spaces-specific) |
| `/msteams/organizations/[customDomain]/admin/product-tags/add`                        | admin | —                              |
| `/msteams/organizations/[customDomain]/admin/tags/[customTagId]/edit`                 | admin | —                              |
| `/msteams/organizations/[customDomain]/admin/tags/add`                                | admin | —                              |
| `/msteams/organizations/[customDomain]/admin/zones/[zoneId]/edit`                     | admin | —                              |
| `/msteams/organizations/[customDomain]/admin/zones/add`                               | admin | —                              |
| `/msteams/organizations/[customDomain]/analytics`                                     | admin | —                              |
| `/msteams/organizations/[customDomain]/bookings`                                      | yes   | —                              |
| `/msteams/organizations/[customDomain]/bookings/[bookingId]`                          | yes   | —                              |
| `/msteams/organizations/[customDomain]/locations`                                     | yes   | —                              |
| `/msteams/organizations/[customDomain]/locations/[locationId]`                        | yes   | —                              |
| `/msteams/organizations/[customDomain]/locations/[locationId]/resources/[resourceId]` | yes   | —                              |
| `/msteams/organizations/[customDomain]/locations/[locationId]/resources/add`          | admin | —                              |
| `/msteams/organizations/[customDomain]/locations/[locationId]/resources/bulk-add`     | admin | —                              |
| `/msteams/organizations/[customDomain]/locations/add-marketplace`                     | admin | Marketplace-only               |
| `/msteams/organizations/[customDomain]/marketplace-public`                            | no    | Public marketplace view        |
| `/msteams/organizations/[customDomain]/marketplace-setup`                             | admin | —                              |
| `/msteams/organizations/[customDomain]/products/[productId]`                          | yes   | —                              |
| `/msteams/organizations/[customDomain]/products/add`                                  | admin | —                              |
| `/msteams/organizations/[customDomain]/resources/add`                                 | admin | —                              |
| `/msteams/organizations/[customDomain]/sso-signin`                                    | no    | —                              |
| `/msteams/organizations/[customDomain]/stripe-connect-accounts/[id]`                  | admin | Payment setup                  |
| `/msteams/organizations/[customDomain]/stripe-connect-accounts/add`                   | admin | —                              |
| `/msteams/organizations/[customDomain]/users`                                         | admin | —                              |
| `/msteams/organizations/[customDomain]/users/[customerId]`                            | admin | —                              |
| `/msteams/organizations/add-marketplace`                                              | admin | —                              |
| `/msteams/start-install-msteams`                                                      | no    | —                              |

### Standard Organization Routes (`/organizations/*`) — ~42 routes

| Route                                                                                 | Auth              | Notes                         |
| ------------------------------------------------------------------------------------- | ----------------- | ----------------------------- |
| `/organizations/[customDomain]`                                                       | yes               | —                             |
| `/organizations/[customDomain]/admin`                                                 | admin             | —                             |
| `/organizations/[customDomain]/admin/product-tags/[productTagId]/edit`                | admin             | —                             |
| `/organizations/[customDomain]/admin/product-tags/add`                                | admin             | —                             |
| `/organizations/[customDomain]/admin/tags/[customTagId]/edit`                         | admin             | —                             |
| `/organizations/[customDomain]/admin/tags/add`                                        | admin             | —                             |
| `/organizations/[customDomain]/admin/zones/[zoneId]/edit`                             | admin             | —                             |
| `/organizations/[customDomain]/admin/zones/add`                                       | admin             | —                             |
| `/organizations/[customDomain]/analytics`                                             | admin             | —                             |
| `/organizations/[customDomain]/availability-dashboard`                                | admin             | Dashboard charts              |
| `/organizations/[customDomain]/bank-accounts/[id]`                                    | admin             | Payment                       |
| `/organizations/[customDomain]/bank-accounts/add`                                     | admin             | —                             |
| `/organizations/[customDomain]/bookings`                                              | yes               | —                             |
| `/organizations/[customDomain]/bookings/[bookingId]`                                  | yes               | —                             |
| `/organizations/[customDomain]/bookings/add`                                          | admin             | —                             |
| `/organizations/[customDomain]/locations`                                             | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]`                                | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]/floorPlans`                     | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]/floorPlans/add`                 | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]/floorPlans/admin/[floorPlanId]` | admin             | Floor plan editor             |
| `/organizations/[customDomain]/locations/[locationId]/resources/[resourceId]`         | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]/resources/add`                  | admin             | —                             |
| `/organizations/[customDomain]/locations/[locationId]/resources/bulk-add`             | admin             | Bulk import                   |
| `/organizations/[customDomain]/locations/add-marketplace`                             | admin             | —                             |
| `/organizations/[customDomain]/products`                                              | admin             | Product list                  |
| `/organizations/[customDomain]/products/[productId]`                                  | admin             | —                             |
| `/organizations/[customDomain]/products/add`                                          | admin             | —                             |
| `/organizations/[customDomain]/resources/add`                                         | admin             | —                             |
| `/organizations/[customDomain]/setup-marketplace`                                     | admin             | Marketplace setup             |
| `/organizations/[customDomain]/sso-signin`                                            | no                | SSO                           |
| `/organizations/[customDomain]/stripe-connect-accounts/[id]`                          | admin             | Payment                       |
| `/organizations/[customDomain]/stripe-connect-accounts/add`                           | admin             | —                             |
| `/organizations/[customDomain]/subscriptions`                                         | admin             | —                             |
| `/organizations/[customDomain]/subscriptions/[subscriptionId]`                        | **server→client** | Server shell, client page.tsx |
| `/organizations/[customDomain]/users`                                                 | admin             | —                             |
| `/organizations/[customDomain]/users/[customerId]`                                    | admin             | —                             |
| `/organizations/add-marketplace`                                                      | admin             | —                             |

### Auth/Utility Routes — 3 routes

| Route                    | Auth | Notes |
| ------------------------ | ---- | ----- |
| `/auth/signin`           | no   | —     |
| `/auth/signup`           | no   | —     |
| `/install-slack`         | no   | —     |
| `/slack-success-install` | no   | —     |
| `/welcome`               | yes  | —     |

---

## Spaces-Specific Features vs webapp-teams

| Feature                 | webapp-spaces   | webapp-teams |
| ----------------------- | --------------- | ------------ |
| Product management      | ✓ (more routes) | ✗            |
| Marketplace setup       | ✓               | ✗            |
| Stripe Connect accounts | ✓               | ✗            |
| Bank accounts           | ✓               | ✗            |
| Product tags            | ✓               | ✗            |
| Subscriptions (admin)   | ✓               | ✗            |
| Bulk resource import    | ✓               | ✓            |
| Floor plan admin        | ✓               | ✓            |
| Analytics               | ✓               | ✓            |

---

## Optimization Opportunities

### 1. SSR / Server Components

**Assessment**: Same constraints as webapp-teams — no meaningful SSR opportunity without architectural changes.

- 73/74 routes are `'use client'`
- The 1 server route (`/organizations/[customDomain]/subscriptions/[subscriptionId]`) is a delegation wrapper; the actual content is still a client component
- **Spaces-specific payment routes** (`stripe-connect-accounts`, `bank-accounts`): These add Stripe React SDK (`@stripe/react-stripe-js`, 9K) but it's already small and correctly scoped

### 2. Static / ISR

**Assessment**: 0 candidates. All routes are auth-gated or org-context-dependent.

### 3. Lazy Loading

See `lazy-load-candidates.md`. Spaces-specific opportunities:

- `react-svg-credit-card-payment-icons` (521K) — same barrel issue as all apps
- `@azure/msal-browser` + `@azure/msal-common` (123K) — only needed for `/msteams/*` routes
- Analytics routes (`/analytics`, `/availability-dashboard`) use chart components — verify `@mui/x-charts` is lazy-loaded

### 4. Client Bundle Size (app-code)

webapp-spaces has the **largest app-code** of the three apps: 602K parsed / 138K gzip.  
This is 184K more than webapp-teams and 326K more than webapp. The extra routes (products, subscriptions, marketplace setup, payments) explain the difference.

**Action**: T029-T031 (client boundary narrowing) should prioritize webapp-spaces as the highest-impact app.
