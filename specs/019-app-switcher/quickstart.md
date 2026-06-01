# Quickstart: App Switcher

## Prerequisites

- Work from the repository root.
- Use the web workspace under `src/web`.
- Ensure dependencies are installed for the existing pnpm workspace.

## Configuration

Each product app needs configured base URLs for the three app destinations:

- Skedular (`webapp`)
- Skedular Teams (`webapp-teams`)
- Skedular Spaces (`webapp-spaces`)

The implementation should use product-app environment configuration and pass those values into the shared app-switcher model. Local development may point each destination to its local app URL.

Example local values:

```env
NEXT_PUBLIC_SKEDULAR_APP_URL=http://localhost:3000/
NEXT_PUBLIC_SKEDULAR_TEAMS_APP_URL=http://localhost:3001/
NEXT_PUBLIC_SKEDULAR_SPACES_APP_URL=http://localhost:3002/
```

## Validation Steps

1. Build the shared switcher model in `@skedular/shared`.
2. Render the switcher through the shared `@skedular/ui` component.
3. Wire the current app id and destination URL configuration into authenticated navigation menu content in:
   - `src/web/apps/webapp`
   - `src/web/apps/webapp-teams`
   - `src/web/apps/webapp-spaces`
4. Verify each signed-in app exposes the switcher as a secondary navigation/menu shortcut, not as a header app bar or primary page action.
5. Verify selecting a destination navigates to that app's configured base URL.
6. Verify missing or invalid URLs do not render as active switch targets.
7. Verify configured destinations still render even when destination access is unknown.
8. Verify switching from nested or tenant-specific pages does not preserve page, organization, tenant, or workflow context.
9. Verify the switcher is keyboard usable and responsive at mobile and desktop widths.
10. Verify customer-facing coworking-space subdomain/storefront surfaces do not render the switcher.
11. Verify structured logs cover configuration filtering and user destination selection.

## Suggested Commands

```bash
pnpm --dir src/web --filter @skedular/shared test
pnpm --dir src/web --filter @skedular/ui test
pnpm --dir src/web --filter webapp test
pnpm --dir src/web --filter webapp-teams test
pnpm --dir src/web --filter webapp-spaces test
pnpm --dir src/web --filter webapp lint
pnpm --dir src/web --filter webapp-teams lint
pnpm --dir src/web --filter webapp-spaces lint
```

Run focused commands during implementation, then use broader web lint/test coverage before review.
