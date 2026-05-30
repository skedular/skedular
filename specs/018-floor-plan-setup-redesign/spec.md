# Feature Specification: Floor Plan Setup Page Redesign

**Feature Branch**: `018-floor-plan-setup-redesign`
**Created**: 2026-05-30
**Status**: Ready for Implementation
**Input**: User description: "Redesign the floor plan setup for location (add floor plan and edit floor plan pages) to look modern like the other pages in all three webapps (webapp, webapp-teams, webapp-spaces). It needs to follow the same design system and concept, be centered in the middle of the page, have the same background as others, no extra dark app bar."

## User Scenarios & Testing _(mandatory)_

### User Story 1 - Add Floor Plan Page Looks Modern and Consistent (Priority: P1)

A location manager opens the Add Floor Plan page in the main webapp. Instead of a standalone view with a dark secondary app bar and off-center content, they see a page that matches every other location management page: standard page background, a centered content card, and no unexpected chrome. The form to configure a new floor plan is fully accessible and functional.

**Why this priority**: This is the primary entry point for setting up a floor plan. Inconsistent UI here creates a jarring experience and breaks trust in the product. Fixing this in the main webapp has the widest immediate user impact.

**Independent Test**: Open the Add Floor Plan route in webapp, compare visually to another location settings page (e.g., Add Resource). Both pages must be indistinguishable in layout, background color, and navigation chrome. All floor plan form fields and save/cancel actions must work correctly.

**Acceptance Scenarios**:

1. **Given** a location manager is logged in to webapp, **When** they navigate to the Add Floor Plan page, **Then** the page background matches the standard app background used on all other location management pages
2. **Given** a location manager is on the Add Floor Plan page, **When** the page loads, **Then** no secondary dark app bar is visible above or around the floor plan content
3. **Given** a location manager is on the Add Floor Plan page, **When** viewing on a standard desktop viewport, **Then** the floor plan form content is horizontally centered on the page inside a card or panel consistent with other settings pages
4. **Given** a location manager fills in the floor plan form and clicks Save, **When** the submission succeeds, **Then** the floor plan is created and the user is redirected as expected

---

### User Story 2 - Edit Floor Plan Page Matches the Redesigned Add Page (Priority: P1)

A location manager opens the Edit Floor Plan page for an existing floor plan. The page has the same modern look as the redesigned Add Floor Plan page — same layout, same background, same card/panel treatment, no dark app bar.

**Why this priority**: Add and Edit pages for the same entity must be visually identical in structure. Users will navigate between them and any inconsistency will be immediately noticeable.

**Independent Test**: Open the Edit Floor Plan route for an existing floor plan in webapp. Compare side-by-side with the Add Floor Plan page. Layout, chrome, and background must be indistinguishable. Edits to the floor plan must save correctly.

**Acceptance Scenarios**:

1. **Given** a location manager opens the Edit Floor Plan page, **When** the page loads, **Then** the visual layout is identical to the redesigned Add Floor Plan page (centered content, standard background, no dark app bar)
2. **Given** a location manager edits floor plan details and saves, **When** the save succeeds, **Then** changes are persisted and the user is redirected appropriately
3. **Given** a location manager opens the Edit Floor Plan page, **When** comparing with another edit page in the app (e.g., Edit Resource), **Then** both pages share the same structural layout pattern

---

### User Story 3 - Consistent Redesign Across webapp-teams (Priority: P2)

A user managing locations from within Microsoft Teams (webapp-teams) opens the Add or Edit Floor Plan page. The page looks and behaves consistently with all other pages in the Teams-embedded experience — same background, centered layout, no dark app bar.

**Why this priority**: webapp-teams users are embedded inside a Teams tab and have different viewport constraints. Visual inconsistencies here reflect poorly on the integrated experience, but this app has a smaller user base than the main webapp.

**Independent Test**: Open Add Floor Plan and Edit Floor Plan in webapp-teams. Both pages must match the layout and visual chrome of other pages in that same app (e.g., location listing or resource pages).

**Acceptance Scenarios**:

1. **Given** a user in webapp-teams navigates to Add Floor Plan, **When** the page loads, **Then** it uses the same page background and centered layout as other pages in webapp-teams
2. **Given** a user in webapp-teams navigates to Edit Floor Plan, **When** the page loads, **Then** no dark secondary app bar appears and layout matches webapp-teams design conventions

---

### User Story 4 - Consistent Redesign Across webapp-spaces (Priority: P2)

A user managing locations from webapp-spaces opens the Add or Edit Floor Plan page. The experience is visually consistent with all other pages in webapp-spaces.

**Why this priority**: Same reasoning as webapp-teams — visual consistency across the surfaces that share users or administrators.

**Independent Test**: Open Add Floor Plan and Edit Floor Plan in webapp-spaces. Both pages must match the layout and visual chrome of other pages in webapp-spaces.

**Acceptance Scenarios**:

1. **Given** a user in webapp-spaces navigates to Add Floor Plan, **When** the page loads, **Then** it matches the page layout and background used throughout webapp-spaces
2. **Given** a user in webapp-spaces navigates to Edit Floor Plan, **When** the page loads, **Then** no dark secondary app bar appears

---

### Edge Cases

- What happens if the floor plan image upload control requires specific chrome or toolbar that resembles the dark app bar — can it be restyled or repositioned without breaking the upload functionality?
- How does the centered layout behave on narrow viewports or when embedded in a Teams tab with limited width?
- What happens when the Edit Floor Plan page loads for a floor plan that has no image yet — does the empty-state still look consistent?
- What if a user navigates to a floor plan page that no longer exists — does the error/not-found state also use the updated design?

## Requirements _(mandatory)_

### Functional Requirements

- **FR-001**: Both the Add Floor Plan and Edit Floor Plan pages MUST use the same page background (color, style, and surface treatment) as all other location management pages in the same webapp
- **FR-002**: Neither the Add Floor Plan nor the Edit Floor Plan page MUST render a secondary dark application bar or toolbar distinct from the standard app navigation
- **FR-003**: The floor plan form content on both Add and Edit pages MUST be horizontally centered on the page within a card or panel that matches the pattern used by other settings and management pages in the same webapp
- **FR-004**: The redesign MUST be applied consistently across all three webapps: webapp, webapp-teams, and webapp-spaces
- **FR-005**: All existing floor plan management functionality (floor plan name, image upload, save, cancel, delete where applicable) MUST continue to work correctly after the redesign
- **FR-006**: The redesign scope includes the full page — both the outer chrome (app bar, background, layout wrapper) AND the canvas/resource-placement area where resources are positioned with x/y coordinates on the floor plan image
- **FR-007**: The floor plan canvas/resource-placement area MUST be presented inside a `SettingsSectionCard`, contained within the centered max-width column, consistent with other sectioned content on the same page
- **FR-008**: The `AddFloorPlan` and `EditFloorPlan` implementations MUST remain app-local in each of the three webapps, with the same layout, behaviour, validation, Relay fragment shape, and mutation success/error semantics kept aligned across the copies
- **FR-009**: The typography, spacing, button styles, and form field styles on both pages MUST match the design system tokens and components used throughout the rest of the app
- **FR-010**: The page heading/title treatment (e.g., page title, breadcrumb, or back navigation) MUST match the pattern used on comparable add/edit pages in the same webapp

### Observability and Logging Requirements

> **Not applicable to this feature.** This is a pure frontend layout redesign with no new business logic, backend services, or Temporal workflows. Structured logging requirements (LOG-001–LOG-004) were removed after clarification: the feature only adjusts visual chrome, app-local component alignment, and design token alignment. User-facing feedback continues to use the existing toast notification pattern (`react-toastify`). No structured logging tasks are required.

### Key Entities _(include if feature involves data)_

- **Floor Plan**: Represents a floor-level plan for a location. Key attributes: name, associated location, floor number/label, optional floor plan image. Relationships: belongs to a Location, may have associated Resources.
- **Location**: The physical venue that owns one or more floor plans. Provides context for the page title and breadcrumb.

## Success Criteria _(mandatory)_

### Measurable Outcomes

- **SC-001**: A user navigating between the Add Floor Plan page and any other location management page in the same webapp perceives no visual difference in page layout, background, or chrome — validated through design review sign-off across all three webapps
- **SC-002**: Zero secondary dark app bars appear on the Add or Edit Floor Plan pages in any of the three webapps — verified by visual regression testing or manual review across all three apps
- **SC-003**: The floor plan form is horizontally centered and contained within a panel consistent with other add/edit pages in the same app — confirmed by comparing pixel layout against an equivalent page (e.g., Add Resource)
- **SC-004**: All existing floor plan management tasks (create, update, upload image) complete successfully after the redesign, with no regression in functionality — verified by end-to-end acceptance testing on all three webapps
- **SC-005**: The redesign is implemented consistently in the app-local floor plan setup components across all three webapps, with no behavioural divergence between webapp, webapp-teams, and webapp-spaces

## Clarifications

### Session 2026-05-30

- Q: Does the redesign scope include the canvas/resource-placement area, or only the outer chrome? → A: Full redesign — both the outer chrome and the canvas/resource-placement area
- Q: Should the three webapps' floor plan components be consolidated into a single shared component? → A: No — keep the full Add/Edit Floor Plan implementations app-local, but keep the three copies aligned
- Q: How should the floor plan canvas be presented in the new layout? → A: Inside a `SettingsSectionCard`, contained within the centered max-width column
- Q: Where should shared floor plan code live? → A: Do not move the full Add/Edit Floor Plan components into `@skedular/shared`; only small reusable utilities or controls may be shared when they are already cross-app concerns

## Assumptions

- The current floor plan pages have a custom dark secondary app bar or header that is specific to the floor plan setup flow and not part of the standard navigation chrome.
- Other location management pages in all three webapps already use a consistent modern layout (centered card, standard background) that will serve as the reference design for this redesign.
- "Same design system" refers to the shared `@skedular/ui` and `@skedular/shared` packages already used across all three webapps.
- The redesign covers the full page UI — outer chrome (app bar, background, layout wrapper) AND the canvas/resource-placement area — but excludes changes to business logic, API contracts, or data models.
- "All three webapps" means webapp, webapp-teams, and webapp-spaces as currently structured in `src/web/apps/`.
- The three webapps currently each have their own copy of `add-floor-plan.tsx` and `edit-floor-plan.tsx`; the redesign keeps those components app-local and requires any layout or behavioural fixes to be applied consistently to all three copies.
