# Research: Smart Organization Landing Page

**Feature**: 015-smart-org-landing-page
**Date**: 2026-05-24

---

## Decision 1: How to source `myOrganizations` data for the landing page

**Decision**: Use a **page-level `useQueryLoader`** in `rootPages/page.tsx` with a new
`noOrganizationLandingPage_rootQuery` Relay query — the same pattern used by webapp's
`pageHome_rootQuery`. The landing page component loads its own data independently; the root
shell continues to handle auth/layout via its existing `noOrganizationRootShell_rootQuery`.

**Rationale**: The spec said to extend the root shell query, but codebase exploration revealed
that the root shell does not pass `rootData` to its children, so extending it would require
adding a React Context layer. The page-level query pattern is already established in webapp and
is self-contained with no impact on the root shell component or any other page that uses it.

**Alternatives considered**:

- Extend `noOrganizationRootShell_rootQuery` and share data via React Context: clean but more
  invasive and adds a new coupling between the shell and landing page.
- Use the `noOrganizationAppBar_query` fragment ref passed from the app bar: not possible
  because app bar fragment refs are not propagated to page children.

---

## Decision 2: Removing the left navigation sidebar

**Decision**: Add a `hideSideNav` boolean prop to `NoOrganizationRootShell` in all three apps.
When `hideSideNav={true}`, the `<NoOrganizationLeftSideNavigationMenu>` element is not rendered
(completely absent from the DOM). The landing root page passes `hideSideNav={true}`.

**Rationale**: The current `collapsed` prop merely collapses the nav to a narrow icon rail; it
does not remove it from the DOM. A dedicated `hideSideNav` prop makes the intent explicit,
avoids side-effects on other pages that use `collapsed`, and satisfies FR-007 (not rendered,
not merely hidden).

**Alternatives considered**:

- Reuse `collapsed` by also conditionally rendering `null` when collapsed: too indirect and
  changes existing behaviour for other callers.
- Route-based conditional render inside the shell: tight coupling between routing and layout
  component; harder to test.

---

## Decision 3: Component structure for the landing content

**Decision**: Create a single new `NoOrganizationLandingContent` component per app
(under `src/components/noOrganizationLanding/`). This component:

- Declares a Relay fragment `noOrganizationLandingContent_query` (added to the page-level
  root query) for `myOrganizations { name, uniqueId, customDomain, logoUrl }`
- Renders one of three states based on org count: loading, no-org prompt, org-selection panel
- Encapsulates all layout (centered, max-width 760, `p: { xs: 2, md: 4 }`) consistent with
  existing single-column pages in each app

**Rationale**: Fragment colocation keeps the component self-contained and testable in isolation.
Placing it under `noOrganizationLanding/` mirrors the naming conventions of other feature
directories (`myBillingAndPayment/` was under `components/`).

---

## Decision 4: Layout constants

**Decision**: Use `maxWidth: 760` and `p: { xs: 2, md: 4 }` for the centered landing content,
wrapped in a `Box` with `sx={{ display: 'flex', justifyContent: 'center', alignItems:
'flex-start', width: '100%' }}` to center the fixed-width panel horizontally. This matches the
existing pattern used in `organization-admin-setup-section.tsx`, `organization-team.tsx`, and
other single-column pages.

**Rationale**: `maxWidth: 760` is the established standard for single-column content pages
across webapp-teams. `maxWidth: 1200` is reserved for wide grid/analytics pages. The landing
page panel is single-column content, so 760 is the correct value.

---

## Decision 5: Onboarding redirect guard

**Decision**: The landing content component checks `me.isOnboardingDone` before rendering the
org panel. If `isOnboardingDone` is false, the existing root shell redirect logic takes
precedence (already handled in the root shell); the landing content simply does not render.

**Rationale**: The spec (FR-009 / clarification Q5) states onboarding redirect takes
precedence. The root shell already has `useEffect` logic that redirects to the welcome page if
`!me.isOnboardingDone`. The landing content should guard against rendering in that state.

---

## GraphQL Field Reference

| Field                            | Type                        | Source                  |
| -------------------------------- | --------------------------- | ----------------------- |
| `myOrganizations(types: [...])`  | `[MyOrganizationDetails!]!` | `Query` root            |
| `myOrganizations[].name`         | `String!`                   | `MyOrganizationDetails` |
| `myOrganizations[].uniqueId`     | `String!`                   | `MyOrganizationDetails` |
| `myOrganizations[].customDomain` | `String`                    | `MyOrganizationDetails` |
| `myOrganizations[].logoUrl`      | `String`                    | `MyOrganizationDetails` |
| `me.isOnboardingDone`            | `Boolean!`                  | `CustomerDetails`       |

## Navigation Link Reference

| App           | Create org link function                                     | Target route                                |
| ------------- | ------------------------------------------------------------ | ------------------------------------------- |
| webapp-teams  | `getOrganizationAddPrivateLink(integratedPlatrform)`         | `/organizations/add-private`                |
| webapp-spaces | `getOrganizationAddMarketplaceLink(integratedPlatrform)`     | `/organizations/add-marketplace`            |
| All           | `getOrganizationBaseLink(integratedPlatrform, org.uniqueId)` | `/{integratedPlatrform}/organizations/{id}` |

## Component Reuse Reference

The following components from `@skedular/ui` and the existing app bar are confirmed for reuse:

| Component             | Import                            | Usage                             |
| --------------------- | --------------------------------- | --------------------------------- |
| `StackColumn`         | `@skedular/ui`                    | Page content wrapper              |
| `StackRow`            | `@skedular/ui`                    | Per-org card row layout           |
| `LeadIconTypography`  | `@skedular/ui`                    | Org name in card                  |
| `BodyIconTypography`  | `@skedular/ui`                    | Descriptive text in no-org prompt |
| `OrganizationAvatar`  | `@/components/organizationAvatar` | Org logo/avatar in card           |
| `Card`, `CardContent` | `@mui/material`                   | Card container per org            |
