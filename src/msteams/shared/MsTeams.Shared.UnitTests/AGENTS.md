# MsTeams Shared Unit Tests Agent Notes

This file applies to `msteams/shared/MsTeams.Shared.UnitTests`.

## Purpose

- Unit tests for `MsTeams.Shared` services and helpers.
- These are fast, in-process tests with no real infrastructure dependencies.

## Test File Shape

- Prefer one test class/file per public method, not one large file per service.
- Group tests under the service namespace, e.g. `MsTeamsWorkflowIdServiceTests/GenerateXShould.cs`.
- Order test method parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
- Prefer injected test inputs over hardcoded strings unless testing a specific literal contract.

## Agent Rule

- Keep unit tests fast and free of real infrastructure (no DB, no Kafka, no Temporal).
- If the test requires real infrastructure, it belongs in `msteams/domain/MsTeams.Domain.IntegrationTests` or
  the system tests instead.
- Follow the unit test file shape defined in the root `AGENTS.md`.
