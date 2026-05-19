# Contract: Route Retirement

Route retirement is allowed per completed slice, but only after return paths are checked.

## Route Retirement Checklist

| Check | Required |
|-------|----------|
| Route owner has moved to target app | Yes |
| Visible navigation removed from wrong app | Yes |
| Direct URL behaviour decided | Yes |
| Backend-originated return URL usage checked | Yes |
| Redirect/block/delete action documented | Yes |
| Temporary transition path has removal condition | Conditional |
| Manual review path documented | Yes |

## Allowed Actions

- `keep`: route remains temporarily because it is still needed.
- `redirect`: route sends users to the target app route.
- `block`: route shows a safe unavailable/not-authorised state for the wrong app.
- `delete`: route is removed after return URL risk is cleared.
- `transition`: route remains as a documented temporary adapter.

## Backend-Originated Return URL Gate

Before a route can be deleted:

1. Search for backend, configuration, and environment references to the route or base URL.
2. Identify payment, authentication, notification, and external callback flows that can return to the route.
3. Define the target app URL or route behaviour.
4. Verify the target app can receive the return flow.
5. Document any backend configuration value that must eventually become app-specific.

Backend service and API contract changes are outside this feature. If a route cannot be safely retired without backend changes, keep or transition the route and document the blocker.
