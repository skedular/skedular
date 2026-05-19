# Migration Slices: Split Web Products

Each slice must be independently reviewable and must stop at the manual review checkpoint before the next slice starts.

## Migration Rule: Dual-Run Before Removal

When a journey is moved into WebApp Teams or WebApp Spaces, the original WebApp route must remain available first. The moved journey should be reviewable in the new app while the old route is still present. Only after manual approval can the old route be redirected, blocked, deleted, or kept as a longer transition adapter.

## Slice: foundation

Target app: Shared foundation  
Journey: Three app foundations  
Owner: Product web split review

### Scope

- Included: runnable shell foundations for WebApp, WebApp Spaces, and WebApp Teams; shared app identity helpers; shared app shell visual/runtime primitives; focused tests; foundation verification notes.
- Excluded: moving existing feature journeys, deleting existing routes, changing backend services or contracts, enabling a global app switcher.

### Ownership Moves

- App-owned code moved: none.
- Shared UI foundations moved: app shell layout, review banner, and organisation empty state into `@skedular/ui`.
- Shared application foundations moved: app identity, organisation type ownership helpers, shell view model helpers, and app selection diagnostics into `@skedular/shared`.
- Transitional adapters retained: all existing WebApp routes remain in place.

### Route Retirement

- Old routes: none retired in the foundation slice.
- Action: keep
- Backend-originated return URL audit: not applicable

### Verification

- Lint: `pnpm webapp#lint`, `pnpm webapp-spaces#lint`, and `pnpm webapp-teams#lint` passed.
- Tests: `pnpm webapp#test`, `pnpm webapp-spaces#test`, and `pnpm webapp-teams#test` passed.
- Build: `pnpm webapp#build`, `pnpm webapp-spaces#build`, and `pnpm webapp-teams#build` passed.
- Relay: not applicable unless later slices move GraphQL operations.
- Manual review: inspect `http://localhost:15000`, `http://localhost:15002`, and `http://localhost:15004`.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

## Proposed Slice Order After Ownership Review

Migration strategy correction: copy the existing WebApp UI surface into the target app first, keep the original WebApp route available, then prune target-app routes/components that do not belong in that product. Avoid replacing real WebApp UI with placeholder shells unless the slice explicitly says it is a temporary review shell.

1. `teams-create-private-organisation`
   - Target app: WebApp Teams
   - Journey: private organisation creation foundation and private-only organisation selection entry.
   - Reason: proves Teams private organisation boundary without product/marketplace concepts.
   - Route action: dual-run first. Keep `/organizations/add-private` in WebApp while the Teams version is reviewed.
   - Route risk: `/organizations/add-private` has unknown onboarding/auth return usage, so old route must remain until audit and manual approval pass.

2. `teams-team-management`
   - Target app: WebApp Teams
   - Journey: organisation teams list/detail/add flows.
   - Reason: strongly private/team-owned and isolated from marketplace products.
   - Route action: dual-run first. Keep the old WebApp route until the Teams version is reviewed.
   - Route risk: notification/deep links unknown; keep old WebApp route as transition initially.

3. `spaces-create-marketplace-organisation`
   - Target app: WebApp Spaces
   - Journey: marketplace/co-working organisation creation foundation and marketplace-only organisation selection entry.
   - Reason: proves Spaces marketplace organisation boundary.
   - Route action: dual-run first. Keep `/organizations/add-marketplace` in WebApp while the Spaces version is reviewed.
   - Route risk: `/organizations/add-marketplace` has unknown onboarding/auth return usage, so old route must remain until audit and manual approval pass.

4. `spaces-marketplace-setup`
   - Target app: WebApp Spaces
   - Journey: marketplace setup pages and operator setup components.
   - Reason: strongly Spaces-owned and should not appear in Teams.
   - Route action: dual-run first. Keep old WebApp route until the Spaces version is reviewed.
   - Route risk: keep old WebApp route as transition until direct links are checked.

5. `spaces-products-operator`
   - Target app: WebApp Spaces
   - Journey: operator product list/add/edit pages.
   - Reason: product management belongs to marketplace/co-working operators; Teams must not expose product concepts.
   - Route action: dual-run first. Keep old WebApp route until the Spaces version is reviewed.
   - Route risk: payment/subscription links may reference product routes; route retirement audit required.

6. `webapp-customer-marketplace-cleanup`
   - Target app: WebApp
   - Journey: customer-facing marketplace discovery, product booking, subscription, and subdomain storefront cleanup.
   - Reason: keep public/customer surfaces in WebApp after operator/private journeys move.
   - Route risk: high payment/subscription return URL sensitivity; no deletion without audit.

## Ownership Review Status

- Ready for user review: yes
- Accepted before first migration slice: no

## Slice: teams-create-private-organisation

Target app: WebApp Teams  
Journey: private organisation creation  
Owner: Product web split review

### Scope

- Included: create a Teams-owned version of the private organisation creation entry point for side-by-side review.
- Excluded: deleting, redirecting, or blocking the current WebApp `/organizations/add-private` route before manual approval.

### Ownership Moves

- App-owned code moved: created a Teams-owned private organisation creation review shell at `web/apps/webapp-teams/src/app/organizations/add-private/page.tsx`.
- Shared UI foundations moved: none expected unless a neutral primitive is identified during implementation.
- Shared application foundations moved: none expected unless a neutral redirect/transition helper is required.
- Transitional adapters retained: WebApp `/organizations/add-private` remains available.

### Route Retirement

- Old routes: `/organizations/add-private`
- Action: keep
- Backend-originated return URL audit: blocked until the user approves the Teams version and backend-originated references are audited.

### Verification

- Lint: `pnpm webapp-teams#lint` passed from `web/`.
- Tests: `pnpm webapp-teams#test` passed from `web/`.
- Build: `pnpm webapp-teams#build` passed from `web/`.
- Relay: required if this private organisation creation journey is copied into Teams rather than mounted from WebApp.
- Manual review: compare WebApp `http://localhost:15000/organizations/add-private` with Teams `http://localhost:15002/organizations/add-private` before any old-route change.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Blocker

The data-backed private organisation creation form is not moved yet. A direct route mount failed because WebApp Relay artefacts and `@/...` imports resolve relative to the consuming app. The Teams route is a review shell only; moving the full form still requires copying or regenerating the relevant Relay artefacts and rewriting WebApp-local imports into Teams-owned imports. The old WebApp route remains unchanged.

## Slice: teams-organisation-selection-foundation

Target app: WebApp Teams  
Journey: private organisation selection foundation  
Owner: Product web split review

### Scope

- Included: Teams-owned organisation selection route, private-only filtering helper, empty state, and route-selection diagnostics.
- Excluded: backend organisation query wiring, moving WebApp organisation creation or management journeys, removing old WebApp routes.

### Ownership Moves

- App-owned code moved: none from WebApp. This is a new Teams-owned foundation route.
- Shared UI foundations moved: none.
- Shared application foundations moved: none.
- Transitional adapters retained: all WebApp organisation routes remain available.

### Route Retirement

- Old routes: none.
- Action: keep
- Backend-originated return URL audit: not applicable

### Verification

- Lint: `pnpm webapp-teams#lint` passed.
- Tests: `pnpm webapp-teams#test` passed.
- Build: `pnpm webapp-teams#build` passed.
- Relay: not required; no GraphQL operations moved or changed.
- Manual review: inspect `http://localhost:15002/organization-selection`.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

## Slice: teams-team-management-shell

Target app: WebApp Teams  
Journey: private organisation team management shell  
Owner: Product web split review

### Scope

- Included: Teams-owned copy of the existing team-management page shell and a `/teams` Teams route that uses it for side-by-side review.
- Excluded: Relay-backed team list/detail/add routes, team mutations, old WebApp route removal, organisation-specific data loading.

### Ownership Moves

- App-owned code moved: copied `organization-teams-page-shell` into WebApp Teams.
- Shared UI foundations moved: none; the shell already consumes `@skedular/ui`.
- Shared application foundations moved: none.
- Transitional adapters retained: all WebApp team-management routes remain available.

### Route Retirement

- Old routes: `/organizations/[organizationCustomDomain]/teams/**`
- Action: keep
- Backend-originated return URL audit: blocked until a data-backed Teams route exists and user approves it.

### Verification

- Lint: `pnpm webapp-teams#lint` passed from `web/`.
- Tests: `pnpm webapp-teams#test` passed from `web/`.
- Build: `pnpm webapp-teams#build` passed from `web/`.
- Relay: not required; this shell slice does not move GraphQL operations or generated artefacts.
- Manual review: inspect Teams `http://localhost:15002/teams` alongside existing WebApp team routes.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Notes

- The copied Teams shell is marked as a client component because it passes theme callback functions through MUI `sx`
  props. The WebApp route remains unchanged for side-by-side review.

## Slice: spaces-organisation-selection-foundation

Target app: WebApp Spaces  
Journey: marketplace/co-working organisation selection foundation  
Owner: Product web split review

### Scope

- Included: Spaces-owned organisation selection route, marketplace-only filtering helper, empty state, and route-selection diagnostics.
- Excluded: backend organisation query wiring, moving WebApp marketplace organisation creation or marketplace setup journeys, removing old WebApp routes.

### Ownership Moves

- App-owned code moved: none from WebApp. This is a new Spaces-owned foundation route.
- Shared UI foundations moved: none.
- Shared application foundations moved: none.
- Transitional adapters retained: all WebApp marketplace/co-working routes remain available.

### Route Retirement

- Old routes: none.
- Action: keep
- Backend-originated return URL audit: not applicable

### Verification

- Lint: `pnpm webapp-spaces#lint` passed from `web/`.
- Tests: `pnpm webapp-spaces#test` passed from `web/`.
- Build: `pnpm webapp-spaces#build` passed from `web/`.
- Relay: not required; no GraphQL operations moved or changed.
- Manual review: inspect `http://localhost:15004/organization-selection`.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

## Slice: full-webapp-source-copy-baseline

Target app: WebApp Spaces and WebApp Teams  
Journey: full WebApp source baseline before app-specific pruning  
Owner: Product web split review

### Scope

- Included: exact source mirror from `web/apps/webapp/src` into `web/apps/webapp-spaces/src` and `web/apps/webapp-teams/src`, including routes, root pages, components, clients, providers, styles, tests, Relay artefacts, proxy, and shared app shell usage.
- Excluded: package identity files, app-level config files outside `src`, route deletion, route redirect, and app-specific pruning.

### Ownership Moves

- App-owned code moved: none yet. Spaces and Teams now intentionally contain the same source implementation as WebApp.
- Shared UI foundations moved: none.
- Shared application foundations moved: none.
- Transitional adapters retained: all original WebApp routes remain available.

### Route Retirement

- Old routes: all WebApp routes
- Action: keep
- Backend-originated return URL audit: blocked until each later pruning/route-retirement slice is reviewed.

### Verification

- Source parity: `diff -qr web/apps/webapp/src web/apps/webapp-spaces/src` returned no differences.
- Source parity: `diff -qr web/apps/webapp/src web/apps/webapp-teams/src` returned no differences.
- Spaces lint: `pnpm webapp-spaces#lint` passed from `web/`.
- Spaces tests: `pnpm webapp-spaces#test` passed from `web/`.
- Spaces build: `pnpm webapp-spaces#build` passed from `web/`.
- Teams lint: `pnpm webapp-teams#lint` passed from `web/`.
- Teams tests: `pnpm webapp-teams#test` passed from `web/`.
- Teams build: `pnpm webapp-teams#build` passed from `web/`.
- Relay: generated artefacts were copied as part of the source mirror. Relay compiler was not run because no GraphQL operation text changed.
- Manual review: inspect WebApp, Spaces, and Teams side by side before any pruning.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Notes

- This supersedes the earlier partial product-copy and shell-only slices. The new baseline is intentionally broad so pruning can happen one app at a time after review.
- Spaces and Teams temporarily expose the full WebApp route surface. Later slices must remove or block app-inappropriate routes only after manual review and return URL audit.
- The original WebApp source remains unchanged for side-by-side review.

## Slice: webapp-customer-facing-entry-foundation

Target app: WebApp  
Journey: public discovery and customer-facing subdomain entry resolution  
Owner: Product web split review

### Scope

- Included: explicit customer-facing entry point model, custom-domain resolver, co-working subdomain wrapper, private organisation subdomain review shell, customer-entry diagnostics helper, and focused tests.
- Excluded: backend organisation-type lookup, data-backed private organisation storefront, route deletion, payment/booking return URL changes, and broad signed-in public-root navigation removal.

### Ownership Moves

- App-owned code moved: none. This slice keeps WebApp as the owner of customer-facing entry points.
- Shared UI foundations moved: none.
- Shared application foundations moved: none.
- Transitional adapters retained: all WebApp marketplace, organisation admin/operator, and custom-domain storefront routes remain available.

### Route Retirement

- Old routes: none.
- Action: keep
- Backend-originated return URL audit: root and marketplace/customer routes remain high-risk and unchanged; no backend URL configuration changes were made.

### Verification

- Lint: `pnpm webapp#lint` passed from `web/`.
- Tests: `pnpm webapp#test` passed from `web/`.
- Build: `pnpm webapp#build` passed from `web/`.
- Relay: not required; no GraphQL operations moved or changed.
- Manual review: inspect WebApp `http://localhost:15000` and an existing co-working custom-domain storefront.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Notes

- Custom domains still resolve to the existing co-working storefront by default, preserving current behaviour.
- Private organisation custom-domain support is represented as a separate shell and resolver outcome, but it is not activated until the WebApp can determine the organisation type for the host.
- The existing signed-in public discovery root still uses the current no-organisation shell. Removing broader navigation from that shell should be handled as a separate reviewable UI change because it can affect existing account and organisation access.

## Slice: shared-neutral-foundations

Target app: Shared foundation  
Journey: neutral management shell and organisation selection helpers  
Owner: Product web split review

### Scope

- Included: neutral `ManagementPageShell` in `@skedular/ui`, neutral `filterOrganisationsByType` helper in `@skedular/shared`, Teams/Spaces wrapper updates, shared boundary tests, and ownership decision notes.
- Excluded: app-specific route copy, navigation rules, organisation ownership decisions, Relay-backed feature modules, and route retirement.

### Ownership Moves

- App-owned code moved: none. Teams and Spaces wrappers remain app-owned configuration layers.
- Shared UI foundations moved: repeated management-page layout into `web/packages/ui/src/app-shell/management-page-shell.tsx`.
- Shared application foundations moved: repeated organisation-type filtering mechanics into `web/packages/shared/src/app-shell/organisation-selection.ts`.
- Transitional adapters retained: all existing WebApp routes and target-app review routes remain available.

### Route Retirement

- Old routes: none.
- Action: keep
- Backend-originated return URL audit: not applicable

### Verification

- Lint: `pnpm --filter @skedular/shared lint`, `pnpm --filter @skedular/ui lint`, `pnpm webapp-teams#lint`, and `pnpm webapp-spaces#lint` passed from `web/`.
- Tests: `pnpm --filter @skedular/shared test`, `pnpm --filter @skedular/ui test`, `pnpm webapp-teams#test`, and `pnpm webapp-spaces#test` passed from `web/`.
- Build: `pnpm webapp-teams#build` and `pnpm webapp-spaces#build` passed from `web/`.
- Relay: not required; no GraphQL operations moved or changed.
- Manual review: inspect Teams `http://localhost:15002/teams` and Spaces `http://localhost:15004/products` after the shared shell extraction.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Notes

- The shared filter helper requires the owning app to provide allowed organisation types, so app-specific product rules remain outside `@skedular/shared`.
- Teams and Spaces keep app-owned wrappers around the shared management shell so future data-backed slices can add app-specific loading, permissions, and copy without changing the shared primitive.

## Slice: transition-safety-artefacts

Target app: Shared foundation  
Journey: route retirement and return URL safety validation  
Owner: Product web split review

### Scope

- Included: route retirement register validation helpers, backend-originated return URL audit helper, completed slice review validation, unresolved blocker documentation, deletion guard documentation, and full quickstart verification.
- Excluded: route deletion, route redirect, backend URL configuration changes, and app-specific base URL environment changes.

### Ownership Moves

- App-owned code moved: none.
- Shared UI foundations moved: none.
- Shared application foundations moved: transition-safety validation helpers in `web/packages/shared/src/app-migration`.
- Transitional adapters retained: all existing WebApp routes remain available.

### Route Retirement

- Old routes: none changed in this slice.
- Action: keep
- Backend-originated return URL audit: no completed slice has a deleted or redirected route; risky moved-route candidates remain blocked and kept.

### Verification

- Lint: `pnpm --filter @skedular/shared lint`, `pnpm webapp#lint`, `pnpm webapp-spaces#lint`, and `pnpm webapp-teams#lint` passed from `web/`.
- Tests: `pnpm --filter @skedular/shared test`, `pnpm webapp#test`, `pnpm webapp-spaces#test`, and `pnpm webapp-teams#test` passed from `web/`.
- Build: `pnpm webapp#build`, `pnpm webapp-spaces#build`, and `pnpm webapp-teams#build` passed from `web/`.
- Relay: not required; no GraphQL operations moved or changed.
- Manual review: inspect the already-recorded slice URLs before any future route transition.

### Full Quickstart Verification

- `pnpm webapp#lint` passed.
- `pnpm webapp#test` passed.
- `pnpm webapp#build` passed.
- `pnpm webapp-spaces#lint` passed.
- `pnpm webapp-spaces#test` passed.
- `pnpm webapp-spaces#build` passed.
- `pnpm webapp-teams#lint` passed.
- `pnpm webapp-teams#test` passed.
- `pnpm webapp-teams#build` passed.

### Acceptance

- Ready for user review: yes
- Accepted before next slice: no

### Notes

- Route retirement entries for completed slices are summarized in `route-retirement-register.md`.
- No route deletion is recorded without a passed backend-originated return URL audit.
- Completed slices with target-app review routes have lint, test, build, and manual review URLs recorded above.

## Final Polish Verification

- Quickstart notes updated with current manual review URLs and route transition rules.
- App ownership docs updated for WebApp, WebApp Spaces, and WebApp Teams.
- Final lint/test/build results are the same full quickstart results recorded in `transition-safety-artefacts`.
- Relay generation was not run because the completed implementation did not move or change GraphQL operations.
- Changed app-facing copy was reviewed for British English. User-facing text uses `organisation`; code identifiers and route/contract names were not renamed.
