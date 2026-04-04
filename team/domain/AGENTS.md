# Team Domain Test Agent Notes

This file covers `team/domain/`.

## Agent Rule

- Use this area for team-domain integration tests.
- For platform-spanning scenarios, use system tests.
- Keep team Aspire dependency readiness in `Team.Domain.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
