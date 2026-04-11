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

```
src/
  app/           # Next.js App Router pages and layouts
  clients/       # HTTP API clients (generated — do not hand-edit)
  components/    # React component library
    commons/     # Shared typography wrappers, layout primitives (see Typography Rule)
  libs/          # Low-level utility libraries (analytics, auth, mui, providers, theme, utils)
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
- Use the Skedular wrappers exported from `@/components/commons`, e.g.:
  - `BodyIconTypography`, `SmallIconTypography`, `CaptionIconTypography`
  - `LeadIconTypography`, heading wrappers
- The only exception is inside `src/components/commons/` implementation files where direct MUI `Typography` is the
  low-level primitive.

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
- For compact location cards specifically:
  - keep the card focused on booking context, not management detail dumps
  - prefer this panel order: availability, address, then zones when zones exist
  - do not render teammate-sharing summaries on the compact card; move that detail to the full location surface instead
  - keep address presentation single-line on the card with full text available on hover
  - show a real feature image when present, but keep the fallback icon treatment when no image exists
- For the organization locations index specifically:
  - treat it as a card-first surface only
  - do not preserve or reintroduce a list/grid toggle unless there is a clear product need that the compact cards cannot satisfy
  - remove list-specific query fields, handlers, and mutations when the list mode is removed
- For the organization location detail page specifically:
  - do not reintroduce a nested left rail inside the broader organization shell
  - use a route-backed local top section nav for setup-style subsections
  - keep heavy admin collections such as resources and floor plans behind section-scoped Relay queries so they are not fetched on first load
- Avoid embedding maps, full carousels, or other heavy detail widgets directly inside dense list cards unless the card is explicitly a media-first browsing surface.
- The long-term target is extraction into `web/packages/*`, but do not extract unstable domain-specific widgets too
  early.
- The first design-system package now exists at `web/packages/ui`.
- Prefer placing Relay-free presentation primitives there when they are stable enough to be reused across more than one
  surface.
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
- Use `pnpm` for package management, not `npm` or `yarn`.
- Check `web/AGENTS.md` and `api-definitions/graphql/AGENTS.md` for upstream generation context.
- Keep Relay fragments colocated with the component that renders them whenever practical.
- When a redesign slice removes a card section or filter surface, trim the colocated Relay selections in the same change instead of leaving dead fields behind.
