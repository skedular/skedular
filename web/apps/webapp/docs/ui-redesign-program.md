# Web UI Redesign Program

This document is the working backlog for the Skedular web redesign.

It is intentionally opinionated. The goal is not to preserve the current UI shape. The goal is to rebuild the web
experience into a clearer, wider, more accessible, and more reusable system.

## Product Direction

- Prefer content-first layouts over persistent left-side navigation.
- Use the available width. Do not waste large desktop surfaces on empty gutters or always-open sidebars.
- Optimize for clarity over density.
- Design the customer-facing experience first when possible, then adapt strong patterns into admin/operator surfaces.
- Keep MUI as the low-level primitive layer for now, but stop letting feature code design directly with raw MUI
  components.
- Build a reusable Skedular design system before extracting packages.

## External Product References

These products are useful reference points for information architecture, hierarchy, and booking/listing interaction
patterns:

- Airbnb: listing cards, image-first discovery, clear section grouping, marketplace confidence cues
- Gable: hybrid workspace marketplace + admin mental model, content-first discovery, flexible booking flows
- Calendly: guided setup flow, progressive disclosure for configuration-heavy editors
- Skedda: booking administration, availability and resource-management density

These are inspiration points, not fidelity targets. Skedular should not inherit their visual language blindly.

## Non-Negotiable Redesign Rules

- Avoid permanent left navigation for page-level workflows unless the navigation meaningfully reduces work.
- Prefer top navigation, local tabs, or in-page section navigation over global side rails.
- Every major page must work well on mobile widths without horizontal scrolling.
- Accessibility is a first-order requirement:
  - keyboard reachable
  - visible focus states
  - color is not the only status signal
  - dialogs and menus must be screen-reader sensible
- Build reusable patterns before redesigning large groups of pages.
- Do not preserve confusing legacy UI patterns just because they already exist.

## Architecture Direction

Target layers for the web UI:

1. Design primitives
   - typography
   - buttons
   - chips
   - cards
   - section shells
   - dialogs
   - layout primitives
2. Design patterns
   - page headers
   - cards with image/header/content/status areas
   - settings sections
   - list/detail layouts
   - empty/loading/error states
   - filter bars
   - summary rails
3. Domain components
   - products
   - bookings
   - subscriptions
   - refunds
   - locations
   - organizations
4. Page compositions
   - customer marketplace
   - admin setup
   - operational dashboards

## Package Strategy

The web monorepo already supports `web/packages/*`, but the design system is not mature enough yet to extract
immediately.

Stage 1: stabilize the system inside `web/apps/webapp/src/components`

- `design-system/`
- `patterns/`
- `domains/`

Stage 2: extract mature reusable layers into `web/packages/*`

Recommended future packages:

- `web/packages/ui`
  - buttons, cards, chips, dialogs, layout, typography, section shells
- `web/packages/forms`
  - field wrappers, settings sections, validation summaries, common editors
- `web/packages/theme`
  - design tokens, theme setup, palette/spacing/shape rules
- `web/packages/icons`
  - shared icon entry points and icon helpers

Do not extract domain widgets into packages too early. Booking/product/location/refund widgets should remain app-level
until the stable primitives and patterns are proven.

## MUI Position

Use MUI as infrastructure, not as the product’s visual language.

Keep:

- MUI theme system
- accessibility primitives
- Data Grid / Date Pickers / Tree View where justified
- low-level layout and input primitives

Reduce:

- direct feature-level MUI composition
- one-off styling in page components
- direct raw form assembly everywhere

Current recommendation:

- keep MUI
- wrap it harder
- grow Skedular primitives and patterns on top of it
- do not migrate to another UI library right now

## Redesign Attack Order

### Phase 1: Foundation

1. Navigation and shell
   - reduce persistent left-nav dependence
   - redesign top-level shell for wider content areas
   - unify page headers and page actions
2. Card system
   - define a consistent card anatomy:
     - media
     - header
     - actions
     - body
     - status area

- collection cards are previews, not mini detail pages
- do not embed heavy map previews or full-width carousels inside dense admin card grids when a compact thumb gives the user enough context

3. Form and settings system
   - section cards
   - summaries
   - guided editors
   - list/detail edit flows

### Phase 2: Customer-Facing Surfaces

4. Marketplace product, location, booking, and subscription pages
5. Checkout and payment flows
6. Customer refund and booking-state surfaces

### Phase 3: Admin/Productivity Surfaces

7. Product add/edit
8. Location add/edit
9. Resource add/edit
10. Organization settings/admin

### Phase 4: Operations

11. Booking management
12. Subscription management
13. Refund operations
14. Notifications
15. Analytics and insights

### Phase 5: Setup / Onboarding

16. Add organization flows
17. Marketplace setup
18. Getting started and installation flows

## Pattern Backlog

These patterns should be designed once and reused many times:

- top app shell
- compact top navigation
- page header with summary + actions
- card shell
- settings section shell
- summary rail
- mobile stacked action bar
- list/detail layout
- filter/search toolbar
- empty state
- error state
- loading state
- destructive confirmation flow
- status badge/timeline pattern
- guided editor pattern

## Feature Backlog By Surface

### Foundation / Global

- shell redesign
- navigation redesign
- design tokens audit
- spacing and typography hierarchy audit
- responsive breakpoints audit
- theme audit for dark/light consistency

### Products

- product list cards
- product add/edit
- pricing option editor
- cancellation policy editor
- storefront preview pattern

### Bookings / Subscriptions / Refunds

- customer detail pages
- admin detail pages
- timeline/status pattern
- action panels
- list filters and batch affordances

### Locations / Resources / Teams

- list card redesign
- align location collection cards to the product-card pattern: compact media thumb, header/action rail, status chips, preview panels, and a short CTA row
- keep compact location cards centered on the booking decision:
  - availability is the primary panel
  - address comes next as compact preview text
  - zones are optional supporting detail and should sit after address so cards stay visually aligned when zones are absent
  - remove teammate-sharing summaries from the compact card; treat that as detail-page information
  - prefer one concise availability summary instead of repeating counts, totals, and percentages in multiple lines
  - use the uploaded feature image when present, with the location icon as the empty-state fallback
- keep the organization locations collection page card-only for now:
  - remove list/grid toggles and list-specific table logic when the product direction favors compact booking-oriented cards
  - if the surface stays card-only, trim the Relay selections and page-level mutations that only existed for the old list/table mode
- detail/edit redesign
- shared admin editor patterns

### Organization / Settings

- settings section system
- organization admin information architecture
- payment and billing pages
- notification/settings forms

### Marketplace / Guest

- search and discovery
- product cards
- product details
- booking and subscription confirmation/status pages

## Testing Program

The current web app has effectively no meaningful UI test suite. That needs to change alongside the redesign.

### Component / Unit Tests

Recommended stack:

- Vitest
- React Testing Library
- jsdom

Current baseline:

- the first web UI test runner is now configured in `web/apps/webapp`
- redesign slices that simplify cards should add or update focused Vitest coverage for the compact behavior, especially when sections are intentionally removed
- it covers both app components and the extracted `web/packages/ui` primitives
- initial tests exist for:
  - `PageHeaderPanel`
  - `PageSectionCard`
  - the products page shell

First targets:

- design primitives
- card shells
- section shells
- status/timeline patterns
- form helpers
- guided editors

## Deployment Compatibility Notes

The first design-system extraction into `web/packages/ui` is compatible with the current delivery shape:

- CI already watches `web/packages/**`
- the webapp Docker build copies the full `web/` workspace before install/build
- the Terraform-managed Vercel project already builds from the workspace root with:
  - `pnpm install --recursive --frozen-lockfile`
  - `pnpm webapp#build`

That means Relay-free UI primitives are safe to extract into workspace packages without changing the current Vercel or
Docker build model.

### Interaction Tests

Use component-level interaction tests for:

- product add/edit
- cancellation editor
- refund action panels
- organization settings forms

### End-to-End Tests

Recommended stack:

- Playwright

First flows:

- marketplace booking flow
- subscription cancellation flow
- refund happy path
- product creation/edit flow
- location/resource creation flow
- organization settings save flow

### Responsive Coverage

Critical flows must be tested at:

- mobile width
- tablet width
- desktop width

## Implementation Rule

Every redesign slice should answer these questions before merging:

- What new reusable pattern did it create?
- Which legacy pattern did it remove?
- Does it improve keyboard and mobile behavior?
- Does it reduce local MUI styling in feature code?
- Does it add at least one test if the surface is now stable enough to test?

## Recommended First Execution Slices

1. Shell + navigation redesign plan
2. Card system redesign
3. Settings/form section system
4. Finish product add/edit using the new system
5. Customer marketplace product/detail redesign
6. Booking/subscription/refund detail redesign

This file is the master backlog. Use it to choose one redesign slice at a time and close it properly before opening the
next one.

## Completed Slices

### Slice 1: Page Header + Section Card Pattern

Status: implemented

Delivered:

- `PageHeaderPanel`
- `PageSectionCard`
- first application on the organization products page
- first extraction into `web/packages/ui`

Files:

- `web/packages/ui/src/page-header-panel.tsx`
- `web/packages/ui/src/page-section-card.tsx`
- `web/packages/ui/src/index.ts`
- `src/components/organization/organizationProducts/organization-products.tsx`

Why it matters:

- replaces bare heading + divider layouts with reusable, content-first page sections
- gives the redesign a first concrete visual primitive without needing to redesign every card immediately
- creates a pattern that can be reused for products, locations, refunds, subscriptions, and settings pages
- proves the package boundary: reusable presentation-only components can move out of `webapp`

Next likely uses:

- organization refunds page
- product add/edit page shell
- booking and subscription management list pages

Testing note:

- Vitest/RTL web component test infrastructure and initial tests are now in place; follow-up work should expand coverage for this redesign slice

### Slice 2: Admin Product Card Redesign

Status: implemented

Delivered:

- removed the large hero image treatment from the organization product card
- removed the inverted header strip pattern from that card
- replaced it with a compact, information-first admin card layout
- kept imagery as a small optional thumbnail instead of a dominant card header

Files:

- `src/components/organization/organizationProducts/product-card.tsx`

Why it matters:

- admin/operator cards should prioritize title, state, offer summary, and actions over decorative media
- large media headers add height and complexity without enough operational value
- inverted title strips create unnecessary theme inversion complexity and make state styling harder
- this slice establishes the new direction for admin cards:
  - compact visual identity
  - content-first layout
  - optional small thumbnail
  - no mandatory hero image

Design rule captured:

- customer discovery cards may still justify image-first layouts
- admin management cards should default to information-first layouts

Testing note:

- Vitest/RTL web component test infrastructure and initial tests are now in place; follow-up work should expand coverage for this redesign slice
