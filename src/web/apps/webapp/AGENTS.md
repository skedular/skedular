# Webapp Agent Notes

This file covers `web/apps/webapp/`.

## Purpose

- This is the main Next.js application for the Skedular platform.
- It serves both the customer-facing booking/marketplace experience and the operator/admin management surfaces.

## Technology

- **Framework**: Next.js with App Router (`src/app/`)
- **GraphQL client**: Relay (`react-relay`) — all queries and mutations use generated Relay artifacts
- **UI library**: MUI (Material UI v5+)
- **Language**: TypeScript

## Directory Structure

```text
src/
  app/           # Next.js App Router pages and layouts
  clients/       # HTTP API clients (generated — do not hand-edit)
  components/    # React component library
  libs/          # Low-level utility libraries (analytics, auth, image-file-uploader)
    providers/   # Only integrated-platform-hook.tsx lives here; all other providers moved to @skedular/shared
  queries/       # Relay GraphQL query/mutation/fragment files
    __generated__/ # Relay-generated TypeScript artifacts — DO NOT hand-edit
  rootPages/     # Special root-level page components
  styles/        # Global CSS
  types/         # Shared TypeScript types
  proxy.ts       # Next.js reverse proxy configuration
scripts/
  generate.sh    # Regenerates OpenAPI TypeScript clients and Relay artifacts
```

## Typography Rule

- Never import `@mui/material/Typography` directly in feature or page components.
- Use the Skedular wrappers exported from `@skedular/ui`, e.g.:
  - `BodyIconTypography`, `SmallIconTypography`, `CaptionIconTypography`
  - `LeadIconTypography`, heading wrappers
- The only exception is inside `@skedular/ui/src/typography/` implementation files where direct MUI `Typography` is the
  low-level primitive.

## Package Boundaries

- **`@skedular/ui`**: Design system — typography wrappers, layout primitives (StackColumn, StackRow), commons components, theme. Must NEVER import from `@skedular/shared`.
- **`@skedular/shared`**: Shared runtime — providers (Relay, Theme, PaletteMode, etc.), hooks (useKnownParams, useIntegratedPlatrform), utils (date, name, relay, constants), cookie-consent, mui helpers, image uploaders. MAY import from `@skedular/ui`.
- **webapp**: Product-specific app. Imports from both packages. `@/libs/providers/` now only contains `integrated-platform-hook.tsx` (MS Teams integration, deferred for revisit).

## MS Teams Integration

- `integrated-platform-hook.tsx` stays in `webapp/src/libs/providers/` for future revisit.
- `useIntegratedPlatrform` is available from both `@skedular/shared` and the local `@/libs/providers` path.
- The `InMsTeamsProvider` + `InMsTeamsContext` live in `@skedular/shared`.

## Redesign Program

- The web app is under active redesign. Read
  `web/apps/webapp/docs/ui-redesign-program.md` before making broad UI changes.
- Do not preserve confusing legacy layouts just because they already exist.
- Prefer content-first layouts and avoid permanent left-side navigation unless it is clearly necessary.
- Use as much horizontal space as practical on desktop while still keeping sections readable.
- Redesign by shared pattern first, then apply the pattern across domains:
  - shell
  - cards
  - settings/forms
  - list/detail pages
  - status/timeline surfaces
- Keep MUI as a low-level primitive layer for now, but reduce direct feature-level MUI composition over time.
- If a redesign slice introduces a reusable card/form/layout pattern, prefer extracting it into a shared web primitive
  rather than duplicating the pattern in another feature folder.
- For admin collection cards across products, locations, teams, and similar lists, prefer the same anatomy unless the domain has a strong reason to differ:
  - compact media or icon thumb
  - title and actions in the header row
  - short status chip row
  - 2-3 preview panels instead of full detail dumps
  - a short CTA/action row at the bottom
- For home-page booking collections specifically:
  - remove list/grid toggles once the surface has moved to the compact collection-card pattern
  - prefer a bounded card grid and a dedicated bookings page shell over a divider-heavy section with table fallback
  - booking cards should emphasize location, time, payment state, and a compact booking-detail preview rather than behaving like mini record detail pages
  - payment-status chips on booking and subscription cards should stay consistent across operator and customer-facing surfaces:
    - confirmed/paid uses `success` + `filled`
    - pending uses `warning` + `filled`
    - other states use `default` + `outlined`
  - invoice actions should read as user actions such as `View Invoice`, not file-format jargon
  - if onboarding/getting-started content is present, place it above the working filter/collection surface rather than burying it between the filter bar and the cards
  - top filter bars on home and booking collection pages should sit on the same surfaced white panel language as the newer collection pages
- For the organization bookings collection specifically:
  - remove the list/grid toggle and keep the surface card-first
  - treat this as a shared-view collection page, not the future admin spreadsheet surface
  - keep join/payment/refund actions available, but fold them into the compact booking-card anatomy instead of preserving the older divider-heavy record card
  - private booking create/edit flows should use dedicated pages, not modal dialogs
  - the create flow should support one-time and recurring private bookings in the same editor, with recurrence shown as a first-class schedule choice instead of a separate hidden workflow
- For the operator subscriptions collection specifically:
  - keep it card-first and bounded like the other redesigned collection pages instead of rendering one full-width record strip per subscription
  - keep refund, cancellation, invoice, and payment-confirmation actions inside compact subscription cards
  - recurring billing periods should render as small internal panels within the card, not as page-width rows
- For compact location cards specifically:
  - keep the card focused on booking context, not management detail dumps
  - prefer this panel order: availability, address, then zones when zones exist
  - do not render teammate-sharing summaries on the compact card; move that detail to the full location surface instead
  - keep address presentation single-line on the card with full text available on hover
  - show a real feature image when present, but keep the fallback icon treatment when no image exists
- For marketplace landing/location cards specifically:
  - the left-column location cards and the map-selected popup card should share the same card anatomy
  - keep the feature image as a compact fixed-height media block instead of a tall stretched hero or carousel
  - when no image exists, use a small centered location icon inside the media area instead of a stretched placeholder
  - keep name and address compact and single-line where possible, with full text available on hover
  - treat capacity, floor area, and similar facts as a small internal details section, not floating chips, so the map popup and list card stay visually identical
  - the map popup should not add its own extra panel chrome around the card; keep the close action on the card itself so the popup reads as the same card surface
- For the organization locations index specifically:
  - treat it as a card-first surface only
  - do not preserve or reintroduce a list/grid toggle unless there is a clear product need that the compact cards cannot satisfy
  - remove list-specific query fields, handlers, and mutations when the list mode is removed
  - keep secondary location-management actions such as `Claim Location` out of the page toolbar; place them in the authenticated profile menu instead of repeating them across organization pages
- For the organization location detail page specifically:
  - do not reintroduce a nested left rail inside the broader organization shell
  - use a route-backed local top section nav for setup-style subsections
  - collapse that section nav to a single menu-trigger button on narrower screens instead of wrapping or relying on horizontal overflow
  - keep heavy admin collections such as resources and floor plans behind section-scoped Relay queries so they are not fetched on first load
  - in the resources section, prefer a compact management list with drill-down details over a wide data grid; the location detail page should support dozens of resources without turning into a spreadsheet
  - resource rows should show the real zone/custom/product tag chips inline in a compact metadata strip, not only abstract counts
  - user-specific preference actions like preferred resources should not sit in the primary row chrome on admin management lists; keep them in overflow actions until there is a dedicated personal-preferences surface
  - apply the same compact-row rule to organization-admin zones and tags; avoid bringing data grids back for small tag-like management surfaces
  - collection pages should converge on the same shell language where possible; the organization teams collection should follow the organization locations pattern instead of keeping its own list/grid toggle and old card layout
- For the organization admin page specifically:
  - do not stack another fixed left rail and another sticky app bar inside the existing organization shell
  - prefer a centered content column with a compact in-page header and a route-backed sticky top section nav
  - use the same responsive collapse pattern for the top section nav on medium and smaller widths
  - keep shell-level back navigation in the shell breadcrumbs/app bar instead of duplicating it inside the page body
- For organization team and setup-marketplace detail pages specifically:
  - use the same centered content shell + compact header + route-backed sticky top section nav pattern as location/admin
  - collapse section pills into a single section-menu trigger on medium and smaller widths
  - do not reintroduce fixed inner left rails or nested page-level app bars
  - render one active admin section at a time from the route-backed section nav; do not stack all admin sections into one long page
  - do not use right-side summary rails on these route-backed setup/admin sections; the section content should own the available width
- Avoid embedding maps, full carousels, or other heavy detail widgets directly inside dense list cards unless the card is explicitly a media-first browsing surface.
- The long-term target is extraction into `web/packages/*`, but do not extract unstable domain-specific widgets too
  early.
- The first design-system package now exists at `web/packages/ui`.
- Prefer placing Relay-free presentation primitives there when they are stable enough to be reused across more than one
  surface.
- Settings-section shells and sticky review rails belong in `web/packages/ui` when they are generic and not tied to a
  specific domain model.
- Generic editor action bars also belong in `web/packages/ui` once they are shared across product, location, or
  resource editing surfaces.
- Split onboarding/setup shells and feature-callout cards belong in `web/packages/ui` when multiple add/create flows
  use the same layout language.
- Every significant redesign slice should consider:
  - desktop width usage
  - mobile responsiveness
  - keyboard/accessibility behavior
  - whether the slice adds reusable UI building blocks
  - whether the slice is stable enough for component or end-to-end tests

## Testing Direction

- The web app currently has little meaningful UI test coverage.
- Vitest + React Testing Library + jsdom is now the default direction for new web UI tests.
- The test runner lives in `web/apps/webapp` and may cover both:
  - app components under `src/`
  - Relay-free design-system primitives under `web/packages/ui`
- New stable primitives and page patterns should start accumulating tests rather than waiting for a final cleanup phase.
- Prefer:
  - component tests for design primitives and shared patterns
  - interaction tests for guided editors and critical forms
  - Playwright end-to-end tests for high-value user flows
- Critical redesigned surfaces should be checked at mobile, tablet, and desktop widths.

## Code Generation

- OpenAPI TypeScript clients are generated by `scripts/generate.sh`.
  - Source YAML: `api-definitions/openapi/skedular/*_v1.yaml`
  - Output: `src/clients/` (generated — do not hand-edit)
- Relay artifacts are generated by the Relay compiler after the GraphQL schema updates.
  - Schema source: the composed gateway schema via `scripts/generate-graphql.sh`
  - Output: `src/queries/__generated__/` (generated — do not hand-edit)
- Use `make generate` from the repo root to run the full pipeline in the correct order.

## Relay Colocation Rule

- Keep Relay GraphQL definitions as close as possible to the component that uses them.
- Prefer colocating:
  - component
  - fragment
  - small local query or mutation
    in the same file or directly adjacent feature file.
- Do not introduce reusable shared fragments by default just to avoid repeating a small selection set.
- Prefer local clarity over cross-feature fragment reuse.
- If a field changes, the developer should be able to open the component file and see the Relay selection sitting next
  to the render logic.
- Reuse fragments only when there is a strong, repeated domain contract and the indirection is clearly worth it.
- For redesign work, bias further toward colocation, not abstraction.

## Authentication & Authorization

- Auth is cookie-based for the web app.
- `ICookieEncryptionService` (not `IXeroTokenEncryptionService`) is used for cookie/SSO concerns.
- The `libs/auth/` area manages session state and route protection.

## Adding A New Page

1. Add a new directory under `src/app/` following Next.js App Router conventions.
2. Use existing layout components and typography wrappers.
3. Define GraphQL queries in `src/queries/` using Relay fragments/queries.
4. Run `make generate` to regenerate Relay artifacts if the query uses new schema fields.

## Agent Rule

- Do not import `@mui/material/Typography` in feature components; use `@/components/commons` wrappers.
- Do not hand-edit files in `src/queries/__generated__/` or generated `src/clients/` files.
- After any backend GraphQL schema change, regenerate Relay artifacts (`make generate` or `scripts/generate.sh`).
- For all customer-facing and operator-facing UI copy, use British spelling and grammar rather than American English.
- Apply that rule to titles, labels, helper text, validation messages, dialog copy, status text, notifications, and
  empty states, but do not rename code-level identifiers, route segments, query names, or generated artefacts for
  localisation alone.
- For private booking editors:
  - keep one-off booking editing and recurring-series editing on separate pages
  - recurring private bookings should expose both `Edit this occurrence` and `Edit recurring booking` from booking cards
  - do not let the recurring-series editor convert a recurring booking into a one-time booking, or vice versa
  - use the one-off editor for generated-instance overrides and the recurring editor for series-wide changes
- Use `pnpm` for package management, not `npm` or `yarn`.
- Check `web/AGENTS.md` and `api-definitions/graphql/AGENTS.md` for upstream generation context.
- Keep Relay fragments colocated with the component that renders them whenever practical.
- When a redesign slice removes a card section or filter surface, trim the colocated Relay selections in the same change instead of leaving dead fields behind.
