# Location Domain Fake Dependencies Notes

This file applies to `location/domain/Location.Domain.FakeDependencies`.

## Purpose

- This project hosts location-owned fake external dependencies for location-domain integration tests.
- Keep it as an Aspire-hosted executable so the location domain can talk to it over real network boundaries later.
- It is currently scaffold-only and does not host domain-specific fake APIs yet.

## Boundary

- Put location-specific fake dependency behavior and scenarios here when integration tests need them.
- Keep generic test helpers in `shared/Testing.Shared.IntegrationTests`.
- Keep generic local infrastructure bootstrapping in `shared/Infrastructure.Shared`.
