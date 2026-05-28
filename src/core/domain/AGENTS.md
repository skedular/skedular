# Core Domain Test Agent Notes

This file covers `core/domain/`.

## What This Area Is For

- `Core.Domain.AppHost` — Aspire app host for the core domain.
- `Core.Domain.FakeDependencies` — core-owned fake external dependency host for integration tests.

## Boundary

- Use this area for core-domain integration tests.
- For scenarios that are truly cross-domain (e.g. core + booking + org), prefer `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep core dependency readiness in `Core.Domain.AppHost/AppHost.cs`.
- All resources referenced by core (Kafka, Temporal, Redis, databases, shared infrastructure, fake dependencies) should
  be paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.

## Agent Rule

- Use this area for core-domain integration tests.
- If the scenario is truly cross-domain, prefer the system test harness.
- Keep core Aspire dependency readiness in `Core.Domain.AppHost/AppHost.cs`; referenced resources should be paired with
  `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
- In integration tests, assert through repository methods rather than querying `DbContext` directly.
