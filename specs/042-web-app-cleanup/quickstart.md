# Quickstart: Web App Component Cleanup Validation

## Prerequisites

Work from the repository root on `042-web-app-cleanup` with the existing workspace dependencies installed. Exclude `node_modules`, `.next`, generated Relay output, shared packages, and UI packages from deletion.

## Audit

For each of `src/web/apps/webapp`, `webapp-spaces`, `webapp-teams`, and `webapp-host`, inventory `src/app`, `src/rootPages`, layouts, middleware/proxy, API routes, dynamic/custom-domain routes, application-owned components/support files, `src/test`, and app-specific `tests`. Trace static imports, aliases, barrels, dynamic imports, test setup, and workspace references. Record results using [contracts/reachability-inventory.md](contracts/reachability-inventory.md). The inventory is intentionally Markdown-only for this cleanup; no JSON or CSV output is required.

Expected result: every candidate is classified and every deletion has evidence; protected routes and route-level tests are not in the default deletion set.

## Cleanup

Delete confirmed-unreachable components and component-only tests. Follow their application-owned dependency chains, deleting helpers, hooks, styles, fixtures, configuration, and tests only when no retained consumer remains. Retain shared or ambiguous files.

## Validation

Run the existing commands for each app:

```bash
pnpm --dir src/web --filter webapp lint && pnpm --dir src/web --filter webapp test && pnpm --dir src/web --filter webapp build
pnpm --dir src/web --filter webapp-spaces lint && pnpm --dir src/web --filter webapp-spaces test && pnpm --dir src/web --filter webapp-spaces build
pnpm --dir src/web --filter webapp-teams lint && pnpm --dir src/web --filter webapp-teams test && pnpm --dir src/web --filter webapp-teams build
pnpm --dir src/web --filter webapp-host lint && pnpm --dir src/web --filter webapp-host test && pnpm --dir src/web --filter webapp-host build
```

Run relevant Playwright smoke suites where supported. Run an app’s Relay compiler only if source operations changed; never hand-edit generated artifacts. Compare the post-cleanup route inventory with the protected pre-cleanup inventory.

Expected result: all four apps pass validation, protected routes remain, no unresolved imports remain, and the final record lists deletions and retained ambiguities.

## Observed Environment Results

The cleanup change passed lint, unit/component tests, and production builds for all four apps. Playwright passed for `webapp`; Spaces and Teams each have one pre-existing strict-locator failure where `getByRole('link', { name: 'Sign in' })` resolves to two links; Host could not start because port 15006 was already occupied. These e2e results are recorded as validation findings and are not attributed to the removed component.
