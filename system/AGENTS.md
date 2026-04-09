# System Agent Notes

This file is the entry point for AI agents working in `system/`.

## Purpose

- `system/` is the full-stack composition layer for end-to-end integration and system tests.
- It is the right place for real end-to-end tests that need multiple domains, real APIs, Temporal, and real
  infrastructure wiring.
- No fake services are used here; all domain APIs and workers run as real processes under Aspire.

## Structure

```
system/
  Skedular.AppHost/      # Aspire app host that composes all domain APIs, jobs, processors, and infra
  Skedular.SystemTests/  # xUnit system tests that drive the running system through real API clients
```

## When To Add Tests Here

- Add to `Skedular.SystemTests` when the scenario requires:
    - real cross-domain interactions (e.g. booking + organization + marketplace)
    - real Temporal workflow execution
    - real Kafka event flow between domains
    - real API surface calls (not in-process service calls)
- Prefer single-domain integration tests in the respective `{Domain}.Domain.IntegrationTests` project for simpler
  scenarios that do not need the full system.

## Aspire App Host Rule

- Keep full-system dependency readiness in `Skedular.AppHost/AppHost.cs`.
- All resources (databases, Kafka, Temporal, Redis, domain services, shared infrastructure) referenced by any component
  in the app host should have matching `WaitFor(...)` or `WaitForCompletion(...)` edges.
- Do not duplicate startup polling in `Skedular.SystemTests` for dependencies that are already wired in the app host.

## Test Style

- Tests drive the system through real API clients.
- Do not instantiate internal domain services directly in system tests.
- Assert outcomes through API responses, persisted state read via API queries, or Kafka/Temporal observable side
  effects.

## Agent Rule

- If a scenario requires multiple domains and no fake services, prefer this area over any single-domain integration test
  project.
- Keep full-system dependency readiness in `system/Skedular.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup or ad hoc polling.
