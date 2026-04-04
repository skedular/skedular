# MsTeams Domain Fake Dependencies Notes

This file applies to `msteams/domain/MsTeams.Domain.FakeDependencies`.

## Purpose

- This project hosts msteams-owned fake external dependencies for msteams-domain integration tests.
- Keep it as an Aspire-hosted executable so the msteams domain can talk to it over real network boundaries later.
- It is currently scaffold-only and does not host domain-specific fake APIs yet.

## Boundary

- Put msteams-specific fake dependency behavior and scenarios here when integration tests need them.
- Keep generic test helpers in `shared/Testing.Shared.IntegrationTests`.
- Keep generic local infrastructure bootstrapping in `shared/Infrastructure.Shared`.
