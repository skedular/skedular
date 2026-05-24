# Feature Specification: Remove Member-Facing Features from Spaces App

**Feature Branch**: `013-remove-spaces-org-private`  
**Created**: 2026-05-24  
**Status**: Draft  
**Input**: User description: "Remove private organization-related features from webapp-spaces. The spaces app is for co-working space owners and admins, not for end-members who buy products from the space."

## Overview

The `webapp-spaces` application is the administrative interface for co-working space owners and managers. It currently contains several features that are intended for **end-members** (customers who purchase products from a co-working space), not for space administrators. These member-facing features must be removed from webapp-spaces, as end-members will access equivalent functionality through the main web app instead.

### Member-Facing Features Identified for Removal

The following features were discovered in `web/apps/webapp-spaces` through codebase research:

| Feature                             | Route                  | Component/Directory                           |
| ----------------------------------- | ---------------------- | --------------------------------------------- |
| My Billing & Payment                | `/billing-and-payment` | `components/myBillingAndPayment/`             |
| My Settings (Personal Profile)      | `/settings`            | `components/mySettings/`                      |
| Notifications (Invitations)         | `/notifications`       | `components/notification/notifications/`      |
| Notification bell + badge in AppBar | AppBar UI              | `components/appBar/` (both variants)          |
| Navigation menu entries for above   | Left nav               | `components/navigationMenu/no-organization-*` |

### Features That Must Remain

- All `/organizations/organization/...` admin routes (Bookings, Locations, Products, Subscriptions, Refunds, Analytics, Availability Dashboard, Users, Teams, Admin, Bank Accounts, Stripe Connect, SSO, Marketplace Setup)
- Organization selector and workspace home page
- Welcome / setup flow (for new space admins)
- MS Teams and Slack integration install routes

---

## Clarifications

### Session 2026-05-24

- Q: Is removing these features from webapp-spaces a standalone change, or coordinated with the main webapp already having these features? → A: Standalone — remove from webapp-spaces now; main webapp additions are tracked separately.
- Q: After removing the three nav items from the no-organization left nav, what happens to the nav shell? → A: Keep it as-is with only the Home (org selector) entry remaining.
- Q: Are there other member-facing areas beyond the three identified that need removing? → A: No — scope is limited to the three named features. The "Invite People to Join Organisation" button in the admin nav is admin-initiated (inviting users to become admins/managers) and must stay.

---

## User Scenarios & Testing

### User Story 1 — Admin Sees No Billing & Payment Section (Priority: P1)

A co-working space admin opens webapp-spaces. They should not find any "Billing & Payment" section because that section exists for members to manage their personal credit cards and billing address — not for space administrators.

**Why this priority**: Billing & Payment is a prominent navigation item that creates confusion about the app's purpose. Removing it is the highest-impact cleanup.

**Independent Test**: Can be fully tested by navigating to webapp-spaces and confirming no "Billing & Payment" link appears in the left-side navigation or in the profile dropdown, and that the `/billing-and-payment` route returns a 404 or redirects.

**Acceptance Scenarios**:

1. **Given** an authenticated admin is on any page of webapp-spaces, **When** they view the left-side navigation menu, **Then** no "Billing & Payment" entry is present.
2. **Given** an authenticated admin opens their profile dropdown in the top app bar, **When** they review the available menu items, **Then** no "Billing & Payment" link appears.
3. **Given** a user navigates directly to the `/billing-and-payment` URL in webapp-spaces, **When** the page loads, **Then** the user receives a not-found or redirect response (the route no longer exists).

---

### User Story 2 — Admin Has No Personal Profile Settings Page (Priority: P2)

A co-working space admin opens webapp-spaces. There is no dedicated "Settings" page for managing personal profile details (name, timezone, phone number, personal information visibility). Admins who need to update their personal profile will use the main web app.

**Why this priority**: The Settings page is a secondary navigation item. Its removal clarifies the admin-only purpose of the app with minimal disruption.

**Independent Test**: Can be fully tested by confirming no "Settings" entry in the left-side nav and that `/settings` route no longer exists.

**Acceptance Scenarios**:

1. **Given** an authenticated admin is on any page, **When** they view the left-side navigation, **Then** no "Settings" (personal profile) entry is visible.
2. **Given** an authenticated admin opens their profile dropdown, **When** they review available menu items, **Then** no link to the personal settings page is present.
3. **Given** a user navigates directly to `/settings` in webapp-spaces, **When** the page loads, **Then** the route no longer exists.

---

### User Story 3 — Admin Sees No Member Invitation Notifications (Priority: P3)

A co-working space admin opens webapp-spaces. The notifications page showing pending invitations to join organizations or teams as a member is not present. The notification bell icon with invitation count badges in the app bar is removed. Admins do not receive or act on member invitations within this app.

**Why this priority**: The notifications bell and badge are visible across all pages but the underlying feature is member-centric.

**Independent Test**: Can be fully tested by confirming no notification bell/badge in the app bar, no "Notifications" nav entry, and that `/notifications` route no longer exists.

**Acceptance Scenarios**:

1. **Given** an authenticated admin is on any page, **When** they view the top app bar, **Then** no notification bell icon or pending invitation badge is present.
2. **Given** an authenticated admin views the left-side navigation menu, **When** they review the nav items, **Then** no "Notifications" entry is visible.
3. **Given** a user navigates directly to `/notifications`, **When** the page loads, **Then** the route no longer exists.
4. **Given** a user has pending organization or team invitations, **When** they open webapp-spaces, **Then** no indication of those pending invitations appears anywhere in the UI.

---

### Edge Cases

- What happens when an authenticated user has pending invitations to organizations/teams and opens webapp-spaces? The invitations count must not be fetched or displayed.
- What happens if the profile dropdown previously had links to Settings, Billing & Payment, and Notifications? After removal only admin-relevant items (sign out, theme toggle, feedback, claim ownership) should remain.
- What happens with the "no-organization" left nav after removal? The nav shell is retained as-is with only the Home (org selector) entry; no structural layout changes are required.
- Are there mobile navigation variants that also need cleanup? Yes — `no-organization-mobile-left-side-navigation-menu.tsx` and `mobile-left-side-navigation-menu.tsx` must also be audited for the same removed links.
- The "Invite People to Join Organisation" button visible in the admin left-side nav is NOT in scope for removal. It is an admin-initiated action (inviting users to become admins or managers of the space) and must be preserved.

---

## Requirements

### Functional Requirements

- **FR-001**: The `/billing-and-payment` route MUST be removed from webapp-spaces.
- **FR-002**: The `MyBillingAndPayment` component and all files under `components/myBillingAndPayment/` MUST be removed.
- **FR-003**: The `/settings` route MUST be removed from webapp-spaces.
- **FR-004**: The `MySettings` component and all files under `components/mySettings/` MUST be removed.
- **FR-005**: The `/notifications` route MUST be removed from webapp-spaces.
- **FR-006**: The `Notifications` component and all files under `components/notification/notifications/` MUST be removed.
- **FR-007**: The "Billing & Payment" navigation entry MUST be removed from the no-organization left-side navigation menu (`no-organization-left-side-navigation-menu-content.tsx`).
- **FR-008**: The "Notifications" navigation entry MUST be removed from the no-organization left-side navigation menu.
- **FR-009**: The "Settings" navigation entry MUST be removed from the no-organization left-side navigation menu.
- **FR-010**: The notification bell icon and pending invitation count badge MUST be removed from the top app bar in both the standard (`app-bar.tsx`) and no-organization (`no-organization-app-bar.tsx`) variants.
- **FR-011**: Profile dropdown menu items linking to Notifications, Billing & Payment, and Settings MUST be removed from both app bar variants.
- **FR-012**: The `pendingOrganizationInvitationsCount` and `pendingTeamInvitationsCount` fields MUST be removed from the GraphQL fragments in both app bar components, as they are no longer needed.
- **FR-013**: The mobile navigation menu variants MUST be audited and any links to the three removed routes removed.
- **FR-014**: Any `getBillingAndPaymentLink`, `getNotificationsLink`, and `getSettingsLink` imports in navigation and app bar files MUST be removed if no longer referenced.
- **FR-015**: The `BillingAndPaymentIcon` and `NotificationsIcon` component imports MUST be removed from files where they are no longer used after this cleanup.
- **FR-016**: All Relay-generated query files associated with the removed components (`myBillingAndPayment_*`, `mySettings_*`, `notifications_*`) MUST be removed from `queries/__generated__/`.
- **FR-017**: All component test files associated with the removed components MUST be removed.
- **FR-018**: After removal, webapp-spaces MUST build and pass its existing tests without errors.
- **FR-019**: No new "my bookings" or other member-facing route MUST be introduced in webapp-spaces as part of or after this cleanup.

### Observability and Logging Requirements

- **LOG-001**: No new logging is required for this removal feature.
- **LOG-002**: Any logging instrumentation inside removed components must be deleted along with the components.
- **LOG-003**: Post-removal, the application build MUST succeed with no TypeScript errors or unused import warnings related to the removed code.
- **LOG-004**: No runtime errors MUST be introduced by residual references to removed components.

---

## Success Criteria

### Measurable Outcomes

- **SC-001**: The three removed routes (`/billing-and-payment`, `/settings`, `/notifications`) return 404 or redirect responses in webapp-spaces after deployment.
- **SC-002**: Zero references to `MyBillingAndPayment`, `MySettings`, or the `Notifications` invitations component remain anywhere in `web/apps/webapp-spaces/src/` after cleanup.
- **SC-003**: The webapp-spaces TypeScript build completes with zero new errors after the removal.
- **SC-004**: The webapp-spaces test suite passes without failures related to or caused by the removal.
- **SC-005**: No notification bell, invitation count badge, "Billing & Payment" link, "Settings" (profile) link, or "Notifications" link is visible to any user in the webapp-spaces UI after the change.
- **SC-006**: The left-side navigation menu and app bar remain visually coherent and functional with the remaining admin-only items after cleanup.

---

## Assumptions

- This removal is a standalone change. It is not blocked on or coordinated with the main web app (`webapp`) reaching feature parity. Any work to surface Billing & Payment, personal Settings, or member Notifications in the main webapp is tracked separately.
- The three member-facing features (Billing & Payment, My Settings, Notifications) are expected to be available to end-members through the main web app (`webapp`) over time, but that work is out of scope for this feature.
- The admin "Settings" in the profile dropdown (if it existed as a link to `MySettings`) is a personal profile page — not an organization admin configuration page — and is safe to remove.
- The "Welcome" / setup flow page is for new space admins onboarding and is NOT a member-facing feature; it must be kept.
- MS Teams and Slack install routes (`/install-msteams`, `/install-slack`, `/slack-success-install`, `/start-install-msteams`) are admin integration setup features and must be kept.
- The Relay-generated files under `queries/__generated__/` for removed queries are safe to delete because they are regenerated from GraphQL schema and will not be referenced after the components are removed.
- The "Invite People to Join Organisation" button and its underlying `InvitePeopleToJoinOrganizationButton` component are admin-facing features and are explicitly out of scope for this cleanup.
- The `components/notification/notifications/` sub-directory is the invitations-specific component. The parent `components/notification/` directory containing shared notification toast helpers (`notification-content.tsx`, `index.ts`) must NOT be removed as it is used throughout the app.
