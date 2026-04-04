# MsTeams Domain Test Agent Notes

This file covers `msteams/domain/`.

## Agent Rule

- Use this area for Teams-domain integration tests.
- Use system tests for platform-spanning scenarios.
- Keep Teams Aspire dependency readiness in `MsTeams.Domain.AppHost/AppHost.cs`; referenced resources should be paired
  with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
