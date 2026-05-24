# Developer Quickstart: Remove Member-Facing Features from Spaces App

**Feature**: 013-spaces-remove-member-features  
**App**: `web/apps/webapp-spaces`

## What Changes

Three member-facing routes, their components, navigation entries, and AppBar links are removed from webapp-spaces. The backend is unchanged.

| Feature              | Route removed          | Files deleted                                      |
| -------------------- | ---------------------- | -------------------------------------------------- |
| My Billing & Payment | `/billing-and-payment` | `components/myBillingAndPayment/` (7 files)        |
| My Settings          | `/settings`            | `components/mySettings/` (4 files)                 |
| Member Notifications | `/notifications`       | `components/notification/notifications/` (2 files) |

Navigation entries ("Notifications", "Billing & Payment", "Settings") and the notification bell are also removed from the AppBar and left nav.

## Implementation Order

### Step 1 — Delete routes

```
rootPages/billing-and-payment/page.tsx
rootPages/settings/page.tsx
rootPages/notifications/page.tsx
```

### Step 2 — Delete component directories

```
components/myBillingAndPayment/   (entire directory)
components/mySettings/            (entire directory)
components/notification/notifications/  (subdirectory only; keep parent notification/)
```

### Step 3 — Delete orphaned Relay generated files (13 files)

All `queries/__generated__/myBillingAndPayment_*.graphql.ts`,
`mySettings_*.graphql.ts`, and `notifications_*.graphql.ts`.

### Step 4 — Modify `components/appBar/app-bar.tsx`

Remove from imports:

- `BillingAndPaymentIcon`, `NotificationsIcon` (from icons)
- `getBillingAndPaymentLink`, `getNotificationsLink`, `getSettingsLink` (from links)

Remove from the Relay GraphQL fragment:

- `pendingOrganizationInvitationsCount`
- `pendingTeamInvitationsCount`

Remove from component body:

- Variables: `settingsLink`, `billingAndPaymentLink`, `notificationsLink`, `pendingInvitationsCount`
- The notification bell `IconButton` block (desktop toolbar)
- `{selectedOrganizationId && ...}` Settings `MenuItem`
- `{selectedOrganizationId && ...}` Billing & Payment `MenuItem`
- Mobile-only Notifications `MenuItem` block (inside `Box sx={{ display: { xs: 'block', md: 'none' } }}`)

### Step 5 — Modify `components/appBar/no-organization-app-bar.tsx`

Same changes as Step 4 (no `selectedOrganizationId` guard on Settings/Billing items here).

### Step 6 — Modify `components/navigationMenu/no-organization-left-side-navigation-menu-content.tsx`

Remove from imports:

- `BillingAndPaymentIcon`, `NotificationsIcon` (from icons)
- `getBillingAndPaymentLink`, `getNotificationsLink`, `getSettingsLink` (from links)

Remove variables: `notificationsLink`, `billingAndPaymentLink`, `settingsBaseLink`

Remove three `ListItem` blocks: Notifications, Billing & Payment, Settings.

### Step 7 — Modify `components/links/index.ts`

Remove the three now-unused exports:

- `getBillingAndPaymentLink`
- `getNotificationsLink`
- `getSettingsLink`

### Step 8 — Run Relay compiler

```bash
cd web/apps/webapp-spaces
pnpm relay
```

This regenerates `appBar_query.graphql.ts` and `noOrganizationAppBar_query.graphql.ts` to remove the `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` fields.

## Verification

### Build

```bash
cd web/apps/webapp-spaces
pnpm build
```

Expected: zero TypeScript errors, zero unused import warnings.

### Tests

```bash
cd web/apps/webapp-spaces
pnpm test
```

Expected: all existing tests pass (removed test files are gone; no new test failures).

### Manual smoke check

1. Navigate to webapp-spaces — left nav shows no "Notifications", "Billing & Payment", or "Settings" entries.
2. Open the profile dropdown — no links to those three pages.
3. No notification bell icon is visible in the top bar.
4. Direct navigation to `/billing-and-payment`, `/settings`, `/notifications` returns 404.

## What Must NOT Change

- The "Invite People to Join Organisation" button in the admin left nav — admin-facing, must remain.
- The user avatar, name, theme toggle, feedback, and sign-out items in the profile dropdown.
- The `components/notification/` parent directory and its toast helpers (`notification-content.tsx`, `index.ts`) — used throughout the app.
- All `/organizations/organization/...` admin routes.
