# Component Audit — Organization, Admin, Products, Teams, Users

**Generated**: 2026-06-03  
**Task**: T017  
**Scope**: `organization`, `organizationStoreFrontGuest`, `product`, `productTag`, `team`, `user`, `customTag`, `setupFlow`, `availabilityDashboard`

---

## organization (91 components — 0 with `'use client'`)

The largest component category. 91 components with zero `'use client'` directives — all inherit the page-level client boundary.

### Sub-categories

| Sub-category                       | Estimated Count | Notes                                          |
| ---------------------------------- | --------------- | ---------------------------------------------- |
| `addOrganization/`                 | ~4              | Setup wizard steps; form shells                |
| `organizationAdmin/`               | ~20             | Admin settings, billing, subscription sections |
| `organizationAdminBillingPayment/` | ~5              | Billing forms — **contains CreditCard**        |
| `organizationAnalytics/`           | ~5              | Analytics display                              |
| `organizationBookings/`            | ~6              | Booking list/filter                            |
| `organizationLocations/`           | ~5              | Location card list                             |
| `organizationLocation/`            | ~8              | Location detail admin view                     |
| `organizationProducts/`            | ~5              | Product admin list                             |
| `organizationProduct/`             | ~5              | Product detail admin                           |
| `organizationTeams/`               | ~4              | Team list                                      |
| `organizationTeam/`                | ~6              | Team detail                                    |
| `organizationUsers/`               | ~4              | User list                                      |
| `organizationUser/`                | ~4              | User detail                                    |
| `storefront/`                      | ~5              | Public storefront display                      |
| `sso/`                             | ~3              | SSO config UI                                  |
| `misc`                             | ~7              | Misc org components                            |

### Critical: `organization-admin-billing-payment-section.tsx`

This component imports `CreditCard` from `@skedular/ui/commons/credit-card` (or via the barrel).  
This is one of only 2 components that use the `CreditCard` component.  
It transitively imports `react-svg-credit-card-payment-icons` (521 KB).

When the barrel fix (LL-001) is applied, this component should import `CreditCard` directly:

```typescript
// After barrel fix:
import { CreditCard } from "@skedular/ui/commons/credit-card"; // direct
// Not: import { CreditCard } from '@skedular/ui'; // barrel
```

### Display Components vs Interactive Components

The organization components are overwhelmingly display-layer:

- Section headers, cards, stat displays, labels — **SSR-compatible**
- Form fields with `onChange` handlers — **need `'use client'`**
- Dialog triggers, action buttons — **need `'use client'`**

Pattern recommendation: Extract interactive islands (form submit handlers, dialog triggers) from display shells. Let admin section shells be server-rendered, with only the interactive sub-components as client islands.

---

## availabilityDashboard (6 components — 2 with `'use client'`)

| Component         | `'use client'` | Reason                    | Notes                           |
| ----------------- | -------------- | ------------------------- | ------------------------------- |
| Dashboard chart   | **yes**        | `@mui/x-charts` animation | Chart animations require client |
| Date range picker | **yes**        | Calendar interaction      | Interactive date selection      |
| Dashboard header  | no             | Display                   | SSR-compatible                  |
| Summary cards     | no             | Display                   | SSR-compatible                  |
| Resource grid     | no             | Display                   | SSR-compatible                  |
| Filter panel      | no             | Display shell             | SSR-compatible shell            |

**Assessment**: 2 correctly use `'use client'` for charts and date pickers. The 4 display components (headers, cards, grid) could be server-rendered shells.

---

## product (4 components — 0 with `'use client'`)

All 4 product components are display-layer. SSR-compatible.  
`product-editor-form.tsx` uses `next/image` but no `priority`/`sizes`.

---

## team (3 components — 0 with `'use client'`)

All SSR-compatible. `team-card.tsx` uses `next/image` without `sizes`.

---

## user (2 components — 0 with `'use client'`)

All SSR-compatible.

---

## organizationStoreFrontGuest, productTag, customTag, setupFlow

All display-layer. No `'use client'`. SSR-compatible.

---

## Summary for Org/Admin/Products/Teams/Users

| Category              | Total | Client | Server-compatible | Priority                |
| --------------------- | ----- | ------ | ----------------- | ----------------------- |
| organization          | 91    | 0      | 91                | Display shells → SC     |
| availabilityDashboard | 6     | 2      | 4                 | Charts correctly client |
| product               | 4     | 0      | 4                 | SC-compatible           |
| team                  | 3     | 0      | 3                 | SC-compatible           |
| user                  | 2     | 0      | 2                 | SC-compatible           |

**Total**: 106 components. 2 correctly client. 104 server-compatible.

**Key finding**: This is the largest opportunity set for client boundary narrowing. 91 organization components running client-side today — all are SSR-compatible display components that would render correctly on the server once the page-level boundary is changed.
