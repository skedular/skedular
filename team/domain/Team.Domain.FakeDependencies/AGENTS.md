# Team Domain Fake Dependencies Notes

This file applies to `team/domain/Team.Domain.FakeDependencies`.

## Purpose

- This project hosts team-owned fake external dependencies for team-domain integration tests.
- Keep it as an Aspire-hosted executable so the team domain can talk to it over real network boundaries later.
- It is currently scaffold-only and does not host domain-specific fake APIs yet.

## Boundary

- Put team-specific fake dependency behavior and scenarios here when integration tests need them.
- Keep generic test helpers in `shared/Testing.Shared.IntegrationTests`.
- Keep generic local infrastructure bootstrapping in `shared/Infrastructure.Shared`.
