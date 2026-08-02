# Team Domain Test Agent Notes

This file covers `team/domain/`.

## What This Area Is For

- `Team.Domain.AppHost` — Aspire app host for the team domain.
- `Team.Domain.FakeDependencies` — team-owned fake external dependency host for team integration tests.

## Boundary

- Use this area for team-domain integration tests covering team membership, invitations, and team-local authorization.
- For platform-spanning scenarios (e.g. team + booking + org), use `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep team dependency readiness in `Team.Domain.AppHost/AppHost.cs`.
- All resources referenced by team (Kafka, Temporal, Redis, databases, shared infrastructure, fake dependencies) should
  have matching `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.

## Test Style

- Seed state through team repositories first; trigger behavior through real API boundaries.
- Assert persisted team-domain outcomes through repository methods, not by querying `DbContext` directly.

## Agent Rule

- Use this area for team-domain integration tests.
- For platform-spanning scenarios, use system tests.
- Keep team Aspire dependency readiness in `Team.Domain.AppHost/AppHost.cs`; referenced resources should be paired with
  `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
