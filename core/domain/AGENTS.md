# Core Domain Test Agent Notes

This file covers `core/domain/`.

## Agent Rule

- Use this area for core-domain integration tests.
- If the scenario is truly cross-domain, prefer the system test harness.
- Keep core Aspire dependency readiness in `Core.Domain.AppHost/AppHost.cs`; referenced resources should be paired with
  `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
