# Marketplace Domain Test Agent Notes

This file covers `marketplace/domain/`.

## What This Area Is For

- `Marketplace.Domain.AppHost` — Aspire app host for the marketplace domain.
- `Marketplace.Domain.FakeDependencies` — marketplace-owned fake external dependency host for integration tests.

## Boundary

- Use this area for marketplace-domain integration tests.
- For marketplace plus booking or organization end-to-end scenarios, prefer `system/Skedular.SystemTests` instead.

## Aspire App Host Rule

- Keep marketplace dependency readiness in `Marketplace.Domain.AppHost/AppHost.cs`.
- If a marketplace resource references Kafka, Temporal, Redis, the marketplace database, shared infrastructure, or fake
  dependencies, add the matching `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.

## Fake Dependencies

- Put marketplace-specific fake dependency behavior in `Marketplace.Domain.FakeDependencies`.
- Generic test helpers belong in `shared/Testing.Shared.IntegrationTests`.
- Keep the control API scenario-oriented; prefer configuring domain scenarios over per-method fake setup.

## Agent Rule

- Use this area for marketplace-domain integration tests.
- For marketplace plus booking or organization end-to-end scenarios, use system tests.
- Keep marketplace Aspire dependency readiness in `Marketplace.Domain.AppHost/AppHost.cs`; referenced resources should
  be paired with `WaitFor(...)` or `WaitForCompletion(...)` there rather than in test startup.
- In integration tests, assert through repository methods rather than querying `DbContext` directly.
