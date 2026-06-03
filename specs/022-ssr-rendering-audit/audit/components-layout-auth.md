# Component Audit — Layout, Shell, Auth, Notification, Observability

**Generated**: 2026-06-03  
**Task**: T014  
**Scope**: `appBar`, `auth`, `feedback`, `gettingStarted`, `generics`, `loading`, `notification`, `observability`, `rootShell`, `transitions`

---

## Key Finding

Components in this category are used on nearly every route. Any one of them with a `'use client'` directive establishes a top-level client boundary. The good news: most of these components **do not** have `'use client'` themselves — the boundary is correctly at the page/layout level.

However, because pages currently declare `'use client'`, ALL these components run client-side regardless.

---

## appBar (4 components)

| Component                               | `'use client'` | Hooks Used   | Browser API | SSR-compatible? | Notes                           |
| --------------------------------------- | -------------- | ------------ | ----------- | --------------- | ------------------------------- |
| `no-organization-app-bar.tsx`           | no             | none visible | none        | **Yes**         | Static shell; uses `next/image` |
| `unauthenticated-app-bar.tsx`           | no             | none visible | none        | **Yes**         | Static shell                    |
| `authenticated-app-bar.tsx` (if exists) | no             | —            | —           | Likely          | Conditional auth display        |
| App bar container                       | no             | —            | —           | **Yes**         | Layout-only                     |

**Optimization**: App bars are ideal Server Components candidates. Currently client-side because pages are client. If pages became server components, app bars would naturally be server-rendered.

---

## auth (1 component)

| Component                           | `'use client'` | Hooks Used               | Browser API | SSR-compatible? | Notes                       |
| ----------------------------------- | -------------- | ------------------------ | ----------- | --------------- | --------------------------- |
| `custom-organization-auth-page.tsx` | **yes**        | `useAuth()`, `useRouter` | none        | No              | WorkOS auth; must be client |

**Assessment**: Correctly uses `'use client'`. No optimization opportunity.

---

## feedback, gettingStarted, generics (3 components combined)

| Component         | `'use client'` | Notes                                     |
| ----------------- | -------------- | ----------------------------------------- |
| `feedback/`       | no             | Static content; SSR-compatible            |
| `gettingStarted/` | no             | Conditional display; SSR-compatible       |
| `generics/`       | no             | Generic layout primitives; SSR-compatible |

---

## loading (1 component)

| Component         | `'use client'` | Hooks Used      | Browser API | Notes                                      |
| ----------------- | -------------- | --------------- | ----------- | ------------------------------------------ |
| Loading component | **yes**        | animation hooks | none        | Correctly client; uses CSS animation state |

---

## notification (2 components)

| Component         | `'use client'` | Notes                        |
| ----------------- | -------------- | ---------------------------- |
| Notification list | no             | Data display; SSR-compatible |
| Notification item | no             | Data display; SSR-compatible |

**Note**: Notifications require auth (Relay query) but the display components themselves are server-compatible.

---

## observability (4 components)

| Component                        | `'use client'` | Notes                                   |
| -------------------------------- | -------------- | --------------------------------------- |
| `LogRocketProvider` (or similar) | no             | Provider wrapping; uses effect for init |
| Error boundary                   | no             | React error handling                    |
| Analytics wrapper                | no             | Script injection                        |
| Observability init               | no             | Side effect only                        |

**Assessment**: Observability providers should all be in `ClientRootLayout` — this is correct. Components themselves are thin wrappers.

---

## rootShell (4 components)

| Component                  | `'use client'` | Hooks Used       | Notes                                 |
| -------------------------- | -------------- | ---------------- | ------------------------------------- |
| `UnauthenticatedRootShell` | no             | none (delegated) | Renders auth UI; SSR-compatible shell |
| `NoOrganizationRootShell`  | no             | none             | Renders org-not-found UI              |
| `AuthenticatedRootShell`   | no             | none             | Renders authenticated content         |
| Root shell container       | no             | —                | Layout wrapper                        |

**Assessment**: Root shells are server-compatible display components. The auth logic is in the page, not the shell. **High-value opportunity**: These shells could render their static structure server-side (loading spinners, navigation chrome) and hydrate the dynamic parts client-side.

---

## Summary for Layout/Auth/Shell/Observability

| Category       | Total | Client | Server-compatible | Key Opportunity                    |
| -------------- | ----- | ------ | ----------------- | ---------------------------------- |
| appBar         | 4     | 0      | 4                 | Convert to SC when pages become SC |
| auth           | 1     | 1      | 0                 | Correctly client                   |
| feedback       | 1     | 0      | 1                 | Convert to SC                      |
| gettingStarted | 1     | 0      | 1                 | Convert to SC                      |
| generics       | 1     | 0      | 1                 | Convert to SC                      |
| loading        | 1     | 1      | 0                 | Correctly client                   |
| notification   | 2     | 0      | 2                 | Display part = SC                  |
| observability  | 4     | 0      | 4                 | Keep in ClientRootLayout           |
| rootShell      | 4     | 0      | 4                 | SC opportunity                     |

**Total**: 19 components. 2 correctly client. 17 server-compatible but currently running client-side because pages declare `'use client'`.

**Overall assessment**: Excellent SSR conversion potential in this category — once the page-level `'use client'` boundary is removed (see `client-boundary-findings.md`), most of these components would automatically become server-rendered with zero additional changes.
