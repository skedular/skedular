# Customer Domain Test Agent Notes

This file covers `customer/domain/`.

## What This Area Is For

- `Customer.Domain.AppHost` — Aspire app host for the customer domain.
- `Customer.Domain.FakeDependencies` — customer-owned fake external dependency host for integration tests.

## Boundary

- Use this area for customer-domain integration tests covering identity, profile, and authentication state.
- For cross-domain scenarios that need booking or org behavior, use `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep customer dependency readiness in `Customer.Domain.AppHost/AppHost.cs`.
- All resources referenced by customer should have matching `WaitFor(...)` or `WaitForCompletion(...)` there rather than
  in test startup.

## Test Style

- Use constructor injection and existing wiring patterns.
- Assert through repository methods, not by querying `DbContext` directly.

## Agent Rule

- Use constructor injection and existing wiring patterns.
- Prefer system tests for cross-domain scenarios instead of faking neighbors here.
- Keep customer Aspire dependency readiness in `Customer.Domain.AppHost/AppHost.cs`; referenced resources should be
  paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
