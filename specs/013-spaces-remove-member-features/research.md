# Research: Remove Member-Facing Features from Spaces App

**Feature**: 013-spaces-remove-member-features  
**Date**: 2026-05-24  
**Status**: Complete — all unknowns resolved

---

## Research Questions & Decisions

### 1. Which files are callers of `getBillingAndPaymentLink`, `getNotificationsLink`, `getSettingsLink`?

**Decision**: All three link helpers have exactly the following callers within `web/apps/webapp-spaces/src/`:

| Link helper                | Callers (all in webapp-spaces)                                                                                                                                  |
| -------------------------- | --------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| `getBillingAndPaymentLink` | `app-bar.tsx`, `no-organization-app-bar.tsx`, `no-organization-left-side-navigation-menu-content.tsx`, `my-billing-and-payment-section-nav.tsx` (being deleted) |
| `getNotificationsLink`     | `app-bar.tsx`, `no-organization-app-bar.tsx`, `no-organization-left-side-navigation-menu-content.tsx`                                                           |
| `getSettingsLink`          | `app-bar.tsx`, `no-organization-app-bar.tsx`, `no-organization-left-side-navigation-menu-content.tsx`                                                           |

After all deletions and modifications, every caller will be removed. The three link exports in `components/links/index.ts` will have zero callers and must also be removed.

**Rationale**: Leaving unused exports is dead code. The links file is private to webapp-spaces (not a shared package), so removal is safe.

---

### 2. Do the mobile navigation variants need direct edits?

**Decision**:

- `no-organization-mobile-left-side-navigation-menu.tsx` — **No direct changes needed**. It is a thin Drawer wrapper that renders `NoOrganizationLeftSideNavigationMenuContent` directly. Cleanup of the content component cascades automatically.
- `mobile-left-side-navigation-menu.tsx` — **No direct changes needed**. It renders `LeftSideNavigationMenuContent` (org-scoped), which has no links to the three removed routes.

**Rationale**: Both mobile files are pure layout wrappers with no independent links to the removed routes.

---

### 3. What Relay-generated files need to be deleted vs regenerated?

**Decision**:

**Delete (orphaned — source graphql tags being removed):**

- `myBillingAndPayment_addMyBillingDetailsMutation.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_query.graphql.ts`
- `myBillingAndPayment_customerPaymentMethodsDetails_refetchableFragment.graphql.ts`
- `myBillingAndPayment_removeCustomerPaymentMethodMutation.graphql.ts`
- `myBillingAndPayment_rootQuery.graphql.ts`
- `myBillingAndPayment_updateMyBillingDetailsMutation.graphql.ts`
- `mySettings_rootQuery.graphql.ts`
- `mySettings_updateCustomerDetailsMutation.graphql.ts`
- `notifications_acceptInvitationToJoinOrganizationMutation.graphql.ts`
- `notifications_acceptInvitationToJoinTeamMutation.graphql.ts`
- `notifications_rejectInvitationToJoinOrganizationMutation.graphql.ts`
- `notifications_rejectInvitationToJoinTeamMutation.graphql.ts`
- `notifications_rootQuery.graphql.ts`

**Regenerate (source fragment is modified, not deleted):**

- `appBar_query.graphql.ts` — fragment loses `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount`
- `noOrganizationAppBar_query.graphql.ts` — same

**Regeneration command**: `pnpm relay` inside `web/apps/webapp-spaces/` (runs `relay-compiler relay.config.js`).

**Rationale**: Per Constitution IV, generated Relay artefacts must not be hand-edited. Modified source fragments require a compiler run. Deleted source means the generated output is orphaned — manual deletion plus a compiler verification pass is the correct procedure.

---

### 4. What exactly does `app-bar.tsx` profile dropdown contain, and what must be removed vs kept?

**Decision — Items to remove from both AppBar variants:**

- Notification bell `IconButton` (desktop toolbar, wraps `notificationsLink`)
- "Settings" `MenuItem` in profile dropdown
- "Billing & Payment" `MenuItem` in profile dropdown
- Mobile-only Notifications `MenuItem` block (inside `Box sx={{ display: { xs: 'block', md: 'none' } }}`)
- Variables: `settingsLink`, `billingAndPaymentLink`, `notificationsLink`, `pendingInvitationsCount`
- Fragment fields: `pendingOrganizationInvitationsCount`, `pendingTeamInvitationsCount`
- Imports: `BillingAndPaymentIcon`, `NotificationsIcon` (icons), `getBillingAndPaymentLink`, `getNotificationsLink`, `getSettingsLink` (links)

**Decision — Items to keep in both AppBar variants:**

- User avatar + name display
- Theme toggle button and menu
- "Send us feedback" `MenuItem`
- "Sign out" `MenuItem`
- "Claim Location" `MenuItem` (org-scoped AppBar only)
- Organization selector `Select` dropdown
- `me` fields in fragment (name, email, photoUrl)
- `myOrganizations` field in fragment

**Rationale**: The avatar, theme, feedback, and sign-out are admin-relevant. The user identity fields remain because the admin's name and avatar still appear in the profile dropdown header.

---

### 5. Does the backend GraphQL schema need changes?

**Decision**: No backend schema changes required.

The `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` fields will simply stop being queried by the client. The fields remain available in the schema for the main `webapp` which may still use them.

**Rationale**: Removing a client-side query is purely a frontend concern. The backend schema is unchanged.

---

### 6. Is there a `data-model.md` or `contracts/` needed?

**Decision**: Neither is required.

- No new entities, fields, or relationships are introduced.
- No new API surface is exposed.
- Backend schema is unchanged.

**Rationale**: This is a pure deletion/cleanup feature. The only artifacts are `research.md`, `plan.md`, and `quickstart.md`.
