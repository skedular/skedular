# Feature Specification: Remove User-Level Billing & Payment UI

**Feature Branch**: `014-remove-user-billing-ui`  
**Created**: 2026-05-24  
**Status**: Draft  
**Input**: User description: "remove only from UI, but in all three webapp the ability for user only to add payment method for themselves and also the billing section, for now, we only want to capture that for organization of both private and marketplace, but not user level, the backend functionality stays the same"

## Clarifications

### Session 2026-05-24

- Q: Is the `myBillingAndPayment` component directory (including `add-my-payment-method-dialog.tsx` and `my-payment-method-setup-form.tsx`) imported by any component outside of the `/billing-and-payment` route page? → A: No. In both `webapp` and `webapp-teams`, the only external consumer is `rootPages/billing-and-payment/page.tsx`. The entire directory is safe to delete.
- Q: Should the "Billing & Payment" menu item be removed from the `organization-store-front-app-bar.tsx` in `webapp` (shown when browsing a marketplace storefront while logged in)? → A: Yes — remove it from there too, not required.

---

## Context

The platform currently shows a "My Billing & Payment" section to individual users in the main member-facing apps (`webapp` and `webapp-teams`). This section allows a user to manage their own personal payment methods and billing details. Going forward, billing and payment management is to be captured at the **organisation level only** (both private and marketplace organisations), not at the individual user level.

`webapp-spaces` already had its user-level billing UI removed as part of feature 013.

The backend APIs, GraphQL mutations, and data models for user-level payment management remain **unchanged** — only the UI entry points are removed.

---

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Remove User Billing UI from `webapp` (Priority: P1) 🎯 MVP

A member visiting the main customer-facing app (`webapp`) can no longer navigate to a personal "Billing & Payment" page. The route, nav entry, profile dropdown item, and all associated UI components are removed. Organisation-level billing management (accessible via the organisation admin area) is unaffected.

**Why this priority**: `webapp` is the primary member-facing app with the largest user base. Removing the user-level billing surface here has the widest immediate impact on product focus.

**Independent Test**: Log into `webapp` as a regular member — no "Billing & Payment" entry appears in the left-side navigation, no "Billing & Payment" item appears in the profile dropdown menu (including the storefront app bar variant), and navigating to `/billing-and-payment` returns 404.

**Acceptance Scenarios**:

1. **Given** a logged-in member in `webapp`, **When** they inspect the left-side nav, **Then** no "Billing & Payment" link is visible.
2. **Given** a logged-in member in `webapp`, **When** they open the profile dropdown (standard, no-org, or storefront app bar), **Then** no "Billing & Payment" menu item appears.
3. **Given** a logged-in member in `webapp`, **When** they navigate directly to `/billing-and-payment`, **Then** the page returns a 404 / not-found response.
4. **Given** a logged-in member in `webapp`, **When** they navigate directly to `/msteams/billing-and-payment`, **Then** the page returns a 404 / not-found response.
5. **Given** an organisation admin in `webapp`, **When** they visit the organisation billing section, **Then** it is still fully accessible and unaffected.

---

### User Story 2 — Remove User Billing UI from `webapp-teams` (Priority: P2)

A team member visiting the teams-focused app (`webapp-teams`) can no longer navigate to a personal "Billing & Payment" page. The same set of UI surfaces are removed as in User Story 1.

**Why this priority**: `webapp-teams` targets a narrower audience than `webapp`. Removing the surface here completes the cross-product consistency goal.

**Independent Test**: Log into `webapp-teams` as a regular member — no "Billing & Payment" nav entry, no profile dropdown item, and `/billing-and-payment` returns 404.

**Acceptance Scenarios**:

1. **Given** a logged-in member in `webapp-teams`, **When** they inspect the left-side nav, **Then** no "Billing & Payment" link is visible.
2. **Given** a logged-in member in `webapp-teams`, **When** they open the profile dropdown, **Then** no "Billing & Payment" menu item appears.
3. **Given** a logged-in member in `webapp-teams`, **When** they navigate directly to `/billing-and-payment`, **Then** the page returns a 404 / not-found response.
4. **Given** a logged-in member in `webapp-teams`, **When** they navigate directly to `/msteams/billing-and-payment`, **Then** the page returns a 404 / not-found response.

---

### Edge Cases

- What happens if a user has a bookmarked `/billing-and-payment` URL? The route returns 404 — standard Next.js not-found behaviour.
- Does removing the UI affect active payment methods already stored for a user? No — the backend data and APIs are unchanged; only the UI entry points are removed.
- Are there any deep links from emails or notifications pointing to `/billing-and-payment`? Assumed out of scope — any such links will simply resolve to 404 after this change. No redirect is required for this phase.

---

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: The `/billing-and-payment` route MUST be removed from `webapp` (both the standard and `/msteams/billing-and-payment` variants).
- **FR-002**: The `myBillingAndPayment` component directory MUST be deleted entirely from `webapp` (7 files: payment method form, dialog, section nav, main component, and tests). No component outside `rootPages/billing-and-payment/page.tsx` imports from this directory; full deletion is safe.
- **FR-003**: The "Billing & Payment" nav entry MUST be removed from the no-organisation left-side navigation menu in `webapp`.
- **FR-004**: The "Billing & Payment" profile dropdown item MUST be removed from all app bar variants in `webapp`: the standard app bar, the no-organisation app bar, and the organisation storefront app bar.
- **FR-005**: The `/billing-and-payment` route MUST be removed from `webapp-teams` (both standard and `/msteams/billing-and-payment` variants).
- **FR-006**: The `myBillingAndPayment` component directory MUST be deleted entirely from `webapp-teams` (6 files). No component outside `rootPages/billing-and-payment/page.tsx` imports from this directory; full deletion is safe.
- **FR-007**: The "Billing & Payment" nav entry MUST be removed from the no-organisation left-side navigation menu in `webapp-teams`.
- **FR-008**: The "Billing & Payment" profile dropdown item MUST be removed from all app bar variants in `webapp-teams`.
- **FR-009**: Orphaned Relay-generated artifacts for the removed components MUST be deleted from each app's `src/queries/__generated__/` directory.
- **FR-010**: Orphaned Relay-generated artifacts for the removed `myBillingAndPayment` components MUST be cleaned up by running `pnpm relay` in each app after source changes. No app bar fragment or root shell query changes are involved.
- **FR-011**: Dead `getBillingAndPaymentLink` exports in each app's `src/components/links/index.ts` MUST be removed if they have no remaining callers after the above removals.
- **FR-012**: The backend GraphQL resolvers, REST API endpoints, and database models for user-level payment method management MUST remain completely unchanged.
- **FR-013**: Organisation-level billing management UI (accessible via the organisation admin section) MUST remain fully functional and unaffected in both apps.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: No new logging is required — this is a pure UI removal with no new runtime code paths.
- **LOG-002**: Existing structured logs in the backend payment services are unchanged.
- **LOG-003**: No warnings or errors should appear in the browser console after the removal; all remaining components should compile and render cleanly.
- **LOG-004**: Build output (TypeScript compilation, Relay compilation) must emit zero errors.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: `/billing-and-payment` returns 404 in both `webapp` and `webapp-teams`.
- **SC-002**: No "Billing & Payment" entry visible in any navigation surface (left nav, profile dropdown) in either app.
- **SC-003**: `pnpm tsc --noEmit` exits 0 in both `webapp` and `webapp-teams` after the change.
- **SC-004**: All existing automated tests pass in both `webapp` and `webapp-teams` after the change (`pnpm test` exit 0).
- **SC-005**: `pnpm relay` completes without errors in both apps, and Relay artifact counts decrease to match the number of removed query documents.
- **SC-006**: Organisation admin billing pages remain accessible and render without errors.

## Assumptions

- `webapp-spaces` user-level billing was already removed in feature 013 (PR #155); no further changes are needed there.
- The three webapps the user refers to are `webapp`, `webapp-teams`, and `webapp-spaces`.
- `webapp-help`, `webapp-spaces-help`, and `webapp-teams-help` are static help/documentation apps and are assumed to contain no billing UI components; they are out of scope.
- The `getBillingAndPaymentLink` helper in each app's `links/index.ts` has no callers outside the surfaces being removed and can be deleted safely.
- No redirects from the old billing route to another destination are needed for this phase.
- The organisation-level billing section (under the org admin area) is separate from `myBillingAndPayment` and lives in a different component tree; it is out of scope.
