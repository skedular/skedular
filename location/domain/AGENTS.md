# Location Domain Test Agent Notes

This file covers `location/domain/`.

## Agent Rule

- Use this area for location-domain integration tests.
- For broader booking-plus-location behavior, prefer system tests.
- Keep location Aspire dependency readiness in `Location.Domain.AppHost/AppHost.cs`; referenced resources should be
  paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
