# MsTeams Domain Test Agent Notes

This file covers `msteams/domain/`.

## What This Area Is For

- `MsTeams.Domain.AppHost` — Aspire app host for the MsTeams domain.
- `MsTeams.Domain.FakeDependencies` — MsTeams-owned fake external dependency host for integration tests.

## Boundary

- Use this area for MsTeams-domain integration tests covering Azure tenant connection and Teams channel routing.
- For multi-domain scenarios involving booking or org events triggering Teams notifications end to end, use
  `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep MsTeams dependency readiness in `MsTeams.Domain.AppHost/AppHost.cs`.
- All resources referenced by MsTeams should have matching `WaitFor(...)` or `WaitForCompletion(...)` there rather
  than in test startup.

## Agent Rule

- Use this area for Teams-domain integration tests.
- Use system tests for platform-spanning scenarios.
- Keep Teams Aspire dependency readiness in `MsTeams.Domain.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
