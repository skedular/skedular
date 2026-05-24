# Feature Specification: Smart Organization Landing Page

**Feature Branch**: `015-smart-org-landing-page`
**Created**: 2026-05-24
**Status**: Draft
**Input**: User description: "in both webapp-teams and spaces, can we have instead of saying generic message of choosing an org, do a different thing, remember this page shows you are member of: 1) if user is only part of one organization, switch automatically to the only available organization; 2) if multiple organization available, give the same option that is currently available them through the UI, make it available in the middle of landing page, also give them the option from there to create an organization; 3) if no organization is yet selected, give the user the option from the middle of screen to create an organization. Make sure they are aligned with how the other pages layout are done, it has to be in the middle of the screen and has maximum width with proper top, left, right and bottom margin. Also remove the left navigation side bar for the landing page from all apps when no organization is yet selected."

## User Scenarios & Testing _(mandatory)_

### User Story 1 — Organization Selection Panel (Priority: P1)

A signed-in user who belongs to one or more organizations visits the landing page of **webapp-teams** or **webapp-spaces**. Instead of seeing a generic "select an organisation" message, they see a centered selection panel listing all their organizations as cards. They can pick one to enter, or choose to create an additional organization.

**Why this priority**: Replaces the current ambiguous message with a directly actionable UI. Covers all users who already have at least one organization, which is the most common case.

**Independent Test**: Can be fully tested by signing in as a user with one or more organization memberships and navigating to the app root. Delivers value by confirming the org-selection panel replaces the generic message and navigation to an org works.

**Acceptance Scenarios**:

1. **Given** a signed-in user who is a member of one or more organizations, **When** they land on the root page of webapp-teams or webapp-spaces, **Then** they see a centered panel listing all their organizations as individual cards.
2. **Given** the organization selection panel is visible, **When** the user selects an organization card, **Then** they are navigated to that organization's home page.
3. **Given** the organization selection panel is visible, **When** the user chooses "Create organization", **Then** they are navigated to the appropriate organization-creation flow for that app.
4. **Given** the organization selection panel is visible, **When** viewed on any screen size, **Then** the panel is horizontally centered with a maximum width and proper margins on all four sides, consistent with how other pages in the app are laid out.
5. **Given** a signed-in user with exactly one organization, **When** they land on the root page, **Then** they see that single organization displayed as a card in the centered panel (no automatic redirect occurs).

---

### User Story 2 — No-Organization Create Prompt (Priority: P2)

A signed-in user who is not yet a member of any organization visits the landing page of webapp-teams or webapp-spaces. Instead of a generic message, they see a centered prompt in the middle of the screen that invites them to create their first organization.

**Why this priority**: Guides users who have no memberships toward the only available action — creating an organization.

**Independent Test**: Can be fully tested by signing in as a user with zero organization memberships. Delivers value by confirming the user is never stranded with a message that has no actionable call-to-action.

**Acceptance Scenarios**:

1. **Given** a signed-in user who is not a member of any organization, **When** they land on the root page of webapp-teams or webapp-spaces, **Then** they see a centered prompt with a clear call-to-action to create a new organization.
2. **Given** the create-organization prompt is visible, **When** the user activates the create action, **Then** they are navigated to the appropriate organization-creation flow for that app.
3. **Given** the create-organization prompt is visible, **When** viewed on any screen size, **Then** it is horizontally centered with a maximum width and proper margins on all four sides.

---

### User Story 3 — Remove Left Navigation on No-Organization Landing (Priority: P2)

When a signed-in user is on the landing page of any of the three apps (webapp, webapp-teams, webapp-spaces) and no organization has been selected yet, the left-side navigation menu is not shown. The user can still access notifications and settings via the profile menu in the app bar.

**Why this priority**: Removes visual clutter from a page where the left nav items are not relevant to the user's immediate task (choosing or creating an organization).

**Independent Test**: Can be fully tested by navigating to the root landing page as a signed-in user with no active organization context and confirming the left nav is absent while the app bar profile menu remains accessible.

**Acceptance Scenarios**:

1. **Given** a signed-in user on the landing page with no organization context, **When** the page renders in any of the three webapps, **Then** the left-side navigation menu is not displayed.
2. **Given** the left navigation is hidden on the landing page, **When** the user opens the profile menu in the app bar, **Then** notifications and settings remain accessible from there.
3. **Given** a signed-in user who navigates away from the landing page to an organization's section, **When** that organization page renders, **Then** the left-side navigation is visible as normal.

---

### Edge Cases

- What happens when organization membership data is still loading? The page MUST show a loading state rather than incorrectly presenting the no-org prompt.
- What happens if a user has one organization but does not want to enter it immediately? They see it as a card and can choose to create a new one instead.

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: When a signed-in user with one or more organization memberships lands on the root page of webapp-teams or webapp-spaces, the system MUST display a centered organization-selection panel that lists all their organizations as individual cards, each showing the organization name. No automatic redirect occurs regardless of membership count.
- **FR-002**: The organization selection panel MUST include a clearly labelled action to create a new organization, linking to the appropriate creation flow for the current app.
- **FR-003**: When a signed-in user with no organization memberships lands on the root page of webapp-teams or webapp-spaces, the system MUST display a centered prompt with a call-to-action to create a new organization.
- **FR-004**: The centered panel/prompt MUST have a constrained maximum width and proper equal margins on all four sides (top, right, bottom, left), consistent with the layout conventions used on other content pages within the same app.
- **FR-005**: The centered panel/prompt MUST be horizontally centered within the content area with a consistent top margin, matching the layout conventions of other single-column content pages in the same app.
- **FR-006**: The left-side navigation menu MUST NOT be rendered (must be absent from the DOM, not merely collapsed or hidden) on the landing root page of any of the three apps (webapp, webapp-teams, webapp-spaces) when no organization is selected. This applies to webapp even though its current landing page already uses a `collapsed` prop — the sidebar must be fully removed from the layout.
- **FR-007**: The app bar profile menu MUST continue to surface links to notifications and settings when the left nav is hidden on the landing page.
- **FR-008**: While organization membership data is loading, the landing page MUST display a loading indicator rather than prematurely showing the no-org or selection states.
- **FR-009**: The org-selection panel and no-org prompt MUST only be shown when the signed-in user's onboarding is already complete. If onboarding is not yet complete, the existing onboarding redirect MUST take precedence and the new landing page states MUST NOT be rendered.

### Observability and Logging Requirements _(mandatory)_

- **LOG-001**: The landing page MUST emit a structured log event recording which state was shown (one-or-more orgs panel, or no-org prompt), including the organization count.
- **LOG-002**: Any failure during the organization membership fetch on the landing page MUST produce an actionable warning log with enough context to diagnose the failure.
- **LOG-003**: All log entries from the landing page MUST include the user identifier (anonymized/hashed as appropriate) as a correlation context field and MUST NOT include raw personal data.

### Key Entities _(include if feature involves data)_

- **Organization Membership**: The association between a signed-in user and one or more organizations; the count and identity of these memberships drives the landing page behavior.
- **Organization**: A workspace unit (private team for webapp-teams, coworking space for webapp-spaces); the target of auto-redirect and manual selection.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: A signed-in user with one or more organizations can select and navigate to any of their organizations within 1 click from the landing page.
- **SC-002**: A signed-in user with no organizations can reach the organization-creation flow within 1 click from the landing page.
- **SC-003**: The landing page with no organization renders without the left navigation panel in all three apps in 100% of test scenarios.
- **SC-004**: The centered panel/prompt passes visual alignment checks: maximum width constraint respected, equal margins present, and content vertically centered in the viewport across desktop and tablet screen sizes.
- **SC-005**: Zero regression on signed-in users who already have an active organization context (they continue to land on their organization's page as before).

## Clarifications

### Session 2026-05-24

- Q: How should the user's organization list be sourced for the smart landing page logic? → A: Extend the existing `noOrganizationRootShell` root query to include the user's organizations (count + IDs/names) — no separate query or new API needed.
- Q: How should each organization be presented in the multi-org selection panel? → A: Cards showing the organization name.
- Q: For webapp's landing page, does "remove the left nav" mean fully absent from the DOM or is the existing collapsed state sufficient? → A: Fully absent (not rendered) — same treatment across all three apps, including webapp.
- Q: Where should a single-org user be auto-redirected on landing? → A: Auto-redirect is out of scope for this feature entirely. Even users with only one organization must see the selection panel. Automatic switching will be reconsidered in a future iteration.
- Q: Should the new org-selection panel take precedence over the existing onboarding redirect, or vice versa? → A: Onboarding redirect takes precedence. The org panel and no-org prompt are only shown once the user's onboarding is already marked complete.

## Assumptions

- The list of organizations a signed-in user belongs to MUST be sourced by extending the existing `noOrganizationRootShell` root query to include the user's organization memberships (at minimum: count, organization IDs, and display names). No separate query or new backend API is required.
- Automatic redirection (even for single-org users) is explicitly out of scope for this feature. All signed-in users with organizations see the selection panel. Auto-redirect may be revisited in a future iteration.
- "Create organization" in webapp-teams means creating a private organization; in webapp-spaces it means creating a marketplace/coworking organization — each app routes to its own existing creation flow.
- The "no organization selected" landing page applies to signed-in users only; unauthenticated users follow existing sign-in prompts and are out of scope for the smart-routing changes.
- webapp (the marketplace consumer app) does not require the org-selection panel because its landing page already shows marketplace content; only the left-nav DOM removal applies to webapp.
- The visual layout maximum-width constraint should match whatever constraint is already used for content pages in the same app so the design is consistent, not introduce a new arbitrary value.
