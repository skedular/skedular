# Enterprise.Shared Agent Notes

This file applies to everything under `shared/Enterprise.Shared`.

## Shared Cross-Cutting Helpers

- Keep genuinely cross-domain helpers here when they are not owned by one runtime host or one domain.
- If a setting, toggle, or utility is used by multiple app hosts or multiple integration test projects, prefer placing
  it here instead of inside a specific host project.

## Domain App Host Toggles

- `DomainAppHostEnvironmentVariables` lives here because it is shared by app hosts and integration tests.
- Do not move app-host-wide toggles into `Infrastructure.Shared` unless they are truly owned by that host alone.
