# Data Model: Modularize Webapp Products

**Feature**: 004-modularize-webapp-products
**Phase**: 1 — Design
**Date**: 2026-04-27

> This document defines the module ownership map — the primary "data model" for a frontend modularization initiative. It classifies every module, directory, and library currently in the web codebase into its target ownership category and describes the rules that govern each boundary.

---

## Ownership Categories

| Category           | Symbol | Description                                                                                   |
| ------------------ | ------ | --------------------------------------------------------------------------------------------- |
| Design System      | `DS`   | Visual primitives, layout building blocks, theme tokens — no state, no business logic         |
| Shared Application | `SA`   | Runtime infrastructure shared across all products — providers, hooks, cross-product utilities |
| Core Skedular      | `CS`   | Features and components owned by the Skedular scheduler product exclusively                   |
| Teams              | `TM`   | Features and components owned by Skedular for Teams exclusively                               |
| Spaces             | `SP`   | Features and components owned by Skedular for Spaces exclusively                              |
| Per-Product Config | `PC`   | Product-specific configuration that each app owns independently                               |

---

## Package Ownership Map

### `web/packages/ui` — `@skedular/ui` (Design System `DS`)

| Module                                                                | Current Location                 | Target                               | Notes                                              |
| --------------------------------------------------------------------- | -------------------------------- | ------------------------------------ | -------------------------------------------------- |
| `stack-row.tsx`                                                       | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `stack-column.tsx`                                                    | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `page-header-panel.tsx`                                               | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `page-section-card.tsx`                                               | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `settings-section-card.tsx`                                           | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `setup-feature-card.tsx`                                              | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `setup-split-layout.tsx`                                              | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `sticky-review-rail.tsx`                                              | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `guided-editor-progress.tsx`                                          | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| `editor-action-bar.tsx`                                               | `packages/ui/src/`               | `DS` — no change                     | Already correct                                    |
| Typography wrappers (`*-typography.tsx`, `icon-typography.tsx`, etc.) | `webapp/src/components/commons/` | `DS` → `packages/ui/src/typography/` | Pure visual wrappers, no state                     |
| Collection toolbar, colour picker, grid container, form stack, etc.   | `webapp/src/components/commons/` | `DS` → `packages/ui/src/commons/`    | Generic layout helpers, no product state           |
| Theme primitives (`theme-primitives.ts`)                              | `webapp/src/libs/theme/`         | `DS` → `packages/ui/src/theme/`      | Design tokens — colour palette, shadows, font vars |
| Theme construction (`theme.ts`)                                       | `webapp/src/libs/theme/`         | `DS` → `packages/ui/src/theme/`      | MUI theme factory — pure function of design tokens |

### `web/packages/shared` — `@skedular/shared` (Shared Application `SA`) — NEW

This is the **single centralised package** for all cross-product runtime modules. Any code that is shared by two or more products but is not a visual primitive belongs here. Utilities are the canonical example: date helpers, name formatters, and Relay error utilities from `webapp/src/libs/utils/` move here so every product imports from `@skedular/shared` rather than maintaining its own copy.

| Module                                                     | Current Location                       | Target | Notes                                                                                             |
| ---------------------------------------------------------- | -------------------------------------- | ------ | ------------------------------------------------------------------------------------------------- |
| `relay-provider.tsx`                                       | `webapp/src/libs/providers/`           | `SA`   | All products use Relay                                                                            |
| `theme-provider.tsx`                                       | `webapp/src/libs/providers/`           | `SA`   | Depends on `@skedular/ui` theme                                                                   |
| `palette-mode-provider.tsx` + contexts                     | `webapp/src/libs/providers/`           | `SA`   | Cross-product dark/light mode                                                                     |
| `date-picker-localization-provider.tsx`                    | `webapp/src/libs/providers/`           | `SA`   | MUI X date pickers                                                                                |
| `google-analytics-provider.tsx`                            | `webapp/src/libs/providers/`           | `SA`   | Component; tag ID config stays per-product                                                        |
| `logrocket-provider.tsx`                                   | `webapp/src/libs/providers/`           | `SA`   | Component; app ID config stays per-product                                                        |
| `in-msteams-provider.tsx` + `InMsTeamsContext`             | `webapp/src/libs/providers/`           | `SA`   | Provider component moves to `@skedular/shared` (T025); usage within Teams product is deferred     |
| `use-known-params.ts` (was `known-params-hook.tsx`)        | `webapp/src/libs/providers/`           | `SA`   | Cross-product URL param convention                                                                |
| `integrated-platform-hook.tsx` (platform detection)        | `webapp/src/libs/providers/`           | `SA`   | **DEFERRED** — MS Teams platform-detection logic; stays in `webapp` until Teams product is scoped |
| Date utilities (timezone, formatting, comparison helpers)  | `webapp/src/libs/utils/index.ts`       | `SA`   | **Canonical example** — centralised in `@skedular/shared` so all products share one copy          |
| Name utilities (`NameDetails`, formatting helpers)         | `webapp/src/libs/utils/index.ts`       | `SA`   | Generic person-name formatting — centralised in `@skedular/shared`                                |
| Relay error utilities (`RelayErrorLike`, error formatters) | `webapp/src/libs/utils/index.ts`       | `SA`   | Relay is shared across all products — centralised in `@skedular/shared`                           |
| `MuiXLicense` wrapper                                      | `webapp/src/libs/mui/`                 | `SA`   | All products using MUI X must mount it                                                            |
| `defaultGridRowSelectionModelValue`                        | `webapp/src/libs/mui/`                 | `SA`   | MUI X DataGrid utility                                                                            |
| Cookie consent                                             | `webapp/src/libs/cookie-consent/`      | `SA`   | All public-facing products need it                                                                |
| Image file uploader                                        | `webapp/src/libs/image-file-uploader/` | `SA`   | Generic upload UI, no product logic                                                               |

---

## App Ownership Map

### `web/apps/webapp` — Core Skedular (`CS` + `PC`)

| Module                                                        | Category | Notes                                                 |
| ------------------------------------------------------------- | -------- | ----------------------------------------------------- |
| `src/app/` (all routes)                                       | `CS`     | Core Skedular route tree                              |
| `src/app/signin/`, `src/app/callback/`                        | `CS`     | Auth entry points — shared by all products per FR-013 |
| `src/app/settings/`, `src/app/notifications/`                 | `CS`     | Account management — shared entry points per FR-013   |
| `src/app/marketplace/`                                        | `CS`     | Skedular marketplace domain                           |
| `src/app/organizations/`                                      | `CS`     | Organisation management                               |
| `src/app/billing-and-payment/`                                | `CS`     | Billing domain                                        |
| `src/components/booking/`                                     | `CS`     | Booking feature components                            |
| `src/components/marketplace*/`                                | `CS`     | Marketplace feature components                        |
| `src/components/organization*/`                               | `CS`     | Organisation feature components                       |
| `src/components/location/`, `resource/`, `zone/`              | `CS`     | Location and resource management                      |
| `src/components/team/`                                        | `CS`     | Team management within Skedular scheduler             |
| `src/components/product*/`                                    | `CS`     | Product catalogue components                          |
| `src/components/user/`, `mySettings/`, `myBillingAndPayment/` | `CS`     | End-user profile/billing surfaces                     |
| `src/components/stripeConnectAccount/`                        | `CS`     | Stripe integration (payments)                         |
| `src/components/notification/`                                | `CS`     | Notification UI                                       |
| `src/components/setupFlow/`, `gettingStarted/`                | `CS`     | Onboarding flows                                      |
| `src/components/navigationMenu/`, `rootShell/`, `appBar/`     | `CS`     | Skedular app shell and navigation                     |
| `src/components/slackButtons/`                                | `CS`     | Slack integration                                     |
| `src/components/msteams*/` (if any)                           | `CS`     | MS Teams embedded experience for Skedular             |
| `src/components/generics/`                                    | `CS`     | Generic helpers scoped to Skedular feature use        |
| `src/components/forms/`, `datePickers/`, `sorting/`, etc.     | `CS`     | Form patterns specific to Skedular features           |
| `src/queries/` + `__generated__/`                             | `CS`     | Relay fragments/queries for Skedular features         |
| `src/clients/`                                                | `CS`     | Generated OpenAPI clients for Skedular APIs           |
| `src/libs/analytics/`                                         | `PC`     | Google Analytics/GTM config for `webapp`              |
| `src/libs/logging/`                                           | `PC`     | Pino logger — `app: 'webapp'`                         |
| `src/rootPages/`                                              | `CS`     | Root-level page components                            |
| `src/types/`                                                  | `CS`     | TypeScript types for Skedular domain                  |
| `src/styles/`                                                 | `CS`     | Global CSS for `webapp`                               |
| `src/proxy.ts`                                                | `CS`     | Next.js reverse proxy config for Skedular             |

### `web/apps/webapp-teams` — Skedular for Teams (`TM` + `PC`) — DEFERRED

> **⏸ DEFERRED**: Product bootstrapping for `webapp-teams` is out of scope for this feature. The ownership category map below documents the intended future state. Implementation will be planned in a dedicated feature when Teams product work is prioritised.

| Module                | Category | Notes                                                                             |
| --------------------- | -------- | --------------------------------------------------------------------------------- |
| `src/app/layout.tsx`  | `TM`     | Root layout with Teams-specific fonts/metadata; uses `@skedular/shared` providers |
| `src/app/page.tsx`    | `TM`     | Teams product landing page                                                        |
| `src/libs/logging/`   | `PC`     | NEW — Pino logger — `app: 'webapp-teams'`                                         |
| `src/libs/analytics/` | `PC`     | NEW — Teams-specific GA/GTM tag config                                            |
| `src/queries/`        | `TM`     | Teams-specific Relay queries (empty initially)                                    |

### `web/apps/webapp-spaces` — Skedular for Spaces (`SP` + `PC`) — DEFERRED

> **⏸ DEFERRED**: Product bootstrapping for `webapp-spaces` is out of scope for this feature. The ownership category map below documents the intended future state.

| Module                | Category | Notes                                                                              |
| --------------------- | -------- | ---------------------------------------------------------------------------------- |
| `src/app/layout.tsx`  | `SP`     | Root layout with Spaces-specific fonts/metadata; uses `@skedular/shared` providers |
| `src/app/page.tsx`    | `SP`     | Spaces product landing page                                                        |
| `src/libs/logging/`   | `PC`     | NEW — Pino logger — `app: 'webapp-spaces'`                                         |
| `src/libs/analytics/` | `PC`     | NEW — Spaces-specific GA/GTM tag config                                            |
| `src/queries/`        | `SP`     | Spaces-specific Relay queries (empty initially)                                    |

---

## Module Boundary Rules

### Rule 1: Design System Boundary (`DS`)

- A module belongs in `@skedular/ui` if and only if it is a pure visual primitive or design token: no React state, no business logic, no product copy.
- Typography wrappers and layout building blocks qualify. Theme tokens qualify.
- Composite components that include state, context, permissions, or orchestration do NOT qualify, even if visually reusable.

### Rule 2: Shared Module Boundary (`SA`) — `@skedular/shared`

- **All** modules that are shared across two or more products and contain runtime behaviour (state, context, effects, side effects, or pure logic) belong in `@skedular/shared`.
- This includes: React providers, hooks, utilities (date, name, Relay error), MUI helpers, cookie consent, image upload, and any other cross-product runtime abstraction.
- `@skedular/shared` may import from `@skedular/ui`. The reverse is forbidden.
- `@skedular/shared` must NOT import from any product app (`webapp`, `webapp-teams`, `webapp-spaces`).
- **Utilities are the canonical example**: `date-utils`, `name-utils`, `relay-utils` — extracted once, imported everywhere as `import { formatName } from '@skedular/shared'`.

### Rule 3: Product Ownership (`CS`, `TM`, `SP`)

- A module belongs to exactly one product if its behaviour, data, or presentation is specific to that product.
- Feature components, feature routes, domain queries, and product-specific orchestration belong here.

### Rule 4: Per-Product Configuration (`PC`)

- Logger config, analytics tag IDs, product metadata, environment variable bindings — each product owns these independently.
- Per-product config is never extracted to a shared package.

### Rule 5: Auth / Account Surface (per FR-013)

- Sign-in, callback, account settings, and notifications stay in `webapp` as shared core Skedular entry points.
- `webapp-teams` and `webapp-spaces` redirect to `webapp` for these journeys and return to their domain post-authentication.
- Shared authentication primitives (WorkOS AuthKit provider, session cookies) are product-level dependencies, not extracted to `@skedular/shared`.

---

## State Transitions

| Current State                                | Trigger                            | Target State                                                             |
| -------------------------------------------- | ---------------------------------- | ------------------------------------------------------------------------ |
| Module in `webapp/src/libs/providers/`       | Confirmed used by 2+ products      | Move to `@skedular/shared/src/providers/`                                |
| Module in `webapp/src/libs/utils/`           | Confirmed not product-specific     | Move to `@skedular/shared/src/utils/` — **canonical pattern**            |
| Module in `webapp/src/components/commons/`   | Confirmed pure visual primitive    | Move to `@skedular/ui/src/typography/` or `commons/`                     |
| Module in `webapp/src/libs/theme/`           | Is a design token or theme factory | Move to `@skedular/ui/src/theme/`                                        |
| Any moved module                             | Imports using `@/` alias           | Updated to import from package name (`@skedular/ui`, `@skedular/shared`) |
| Module in `webapp` with no cross-product use | Confirmed Skedular-specific        | Stays in `webapp` — no change                                            |

---

## Allowed Transitional Exceptions

No transitional adapters are permitted for module extraction (per FR-006). Each module either:

- Moves to its target package in a single slice, OR
- Stays in its current location until its slice is started.

There is no half-moved state. The slice is complete only when the source location is removed and consumers updated.
