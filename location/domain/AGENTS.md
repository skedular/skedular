# Location Domain Test Agent Notes

This file covers `location/domain/`.

## What This Area Is For

- `Location.Domain.AppHost` — Aspire app host for the location domain.
- `Location.Domain.FakeDependencies` — location-owned fake external dependency host for location integration tests.

## Boundary

- Use this area for location-domain integration tests covering resources, availability, and slot generation.
- For broader booking-plus-location behavior, prefer `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep location dependency readiness in `Location.Domain.AppHost/AppHost.cs`.
- All resources referenced by location (Kafka, Temporal, Redis, databases, shared infrastructure, fake dependencies)
  should have matching `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.

## Test Style

- Seed state through location repositories first; trigger behavior through real API boundaries.
- Assert persisted location-domain outcomes through repository methods, not by querying `DbContext` directly.
- Pay extra attention to time-boundary edge cases in availability/slot generation tests.

## Agent Rule

- Use this area for location-domain integration tests.
- For broader booking-plus-location behavior, prefer system tests.
- Keep location Aspire dependency readiness in `Location.Domain.AppHost/AppHost.cs`; referenced resources should be
  paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
