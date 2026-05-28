# System AppHost Project Notes

This file applies to `system/Skedular.AppHost`.

## Purpose

- `Skedular.AppHost` is the Aspire app host that composes the entire platform for system-level end-to-end tests.
- It wires all domain APIs, jobs, processors, the gateway, and infrastructure dependencies (Kafka, Temporal, Redis,
  databases, etc.) under a single Aspire orchestration.

## Dependency Readiness

- Every resource referenced here must have a matching `WaitFor(...)` or `WaitForCompletion(...)` edge.
- Do not rely on startup polling in test code for dependencies that are wired here.
- If a new domain or infrastructure dependency is added, add the corresponding `WaitFor` edge here.

## Agent Rule

- Keep this file the authoritative source of system-level dependency readiness.
- Do not add application business logic here; this is pure orchestration/wiring.
- Read the parent `system/AGENTS.md` for system test rules.
