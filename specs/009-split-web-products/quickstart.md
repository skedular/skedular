# Quickstart: Split Web Products

Use this guide to validate the foundation and each migration slice.

## Foundation Verification

From the repository root:

```bash
cd web
pnpm webapp#lint
pnpm webapp#test
pnpm webapp#build
pnpm webapp-spaces#lint
pnpm webapp-spaces#test
pnpm webapp-spaces#build
pnpm webapp-teams#lint
pnpm webapp-teams#test
pnpm webapp-teams#build
```

Run apps for manual inspection:

```bash
cd web/apps/webapp
pnpm dev
```

```bash
cd web/apps/webapp-spaces
pnpm dev
```

```bash
cd web/apps/webapp-teams
pnpm dev
```

Default local ports:

- WebApp: `http://localhost:15000`
- WebApp Teams: `http://localhost:15002`
- WebApp Spaces: `http://localhost:15004`

## Slice Verification Checklist

For each migrated slice:

1. Confirm the target owner in the ownership inventory.
2. Confirm app-owned code moved to the owning app.
3. Confirm neutral visual/runtime code moved to `@skedular/ui` or `@skedular/shared`.
4. Confirm Teams has no marketplace organisation/product concepts.
5. Confirm organisation selection is filtered by app where applicable.
6. Confirm old routes are kept, redirected, blocked, deleted, or documented as transition.
7. Confirm backend-originated return URL usage before deleting any route.
8. Run lint/test/build for affected apps and packages.
9. Run Relay generation/checks if GraphQL operations moved or changed.
10. Stop for user review before the next slice.

## Route Retirement Search

Before deleting a route, search for frontend and backend-originated URL references:

```bash
rg "route-fragment-or-base-url" .
```

Pay particular attention to payment, authentication, notification, and external callback flows.

## Review Notes

Each slice should include a short note with:

- target app
- journey moved
- routes changed
- shared code extracted
- verification commands run
- manual URL(s) to inspect
- known transition paths or blockers
