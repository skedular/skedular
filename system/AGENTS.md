# System Agent Notes

This file is the entry point for AI agents working in `system/`.

## Purpose

- `system/` is the full-stack composition layer.
- It is the right place for real end-to-end tests that need multiple domains, real APIs, Temporal, and real
  infrastructure wiring.

## Important Subareas

- `system/Skedular.AppHost`
- `system/Skedular.SystemTests`

## Agent Rule

- If a scenario requires multiple domains and no fake services, prefer this area over any single-domain integration test
  project.
- Keep full-system dependency readiness in `system/Skedular.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup or ad hoc polling.
