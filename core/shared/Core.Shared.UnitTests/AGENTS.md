# Core Shared Unit Tests Agent Notes

This file applies to `core/shared/Core.Shared.UnitTests`.

## Purpose

- Unit tests for `Core.Shared` services and helpers.
- These are fast, in-process tests with no real infrastructure dependencies.

## Test File Shape

- Prefer one test class/file per public method, not one large file per service.
- Group tests under the service namespace, e.g. `CoreWorkflowIdServiceTests/GenerateXShould.cs`.
- Order test method parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
- Prefer injected test inputs over hardcoded strings unless testing a specific literal contract.

## Agent Rule

- Keep unit tests fast and free of real infrastructure (no DB, no Kafka, no Temporal).
- If the test requires real infrastructure, it belongs in `core/domain/Core.Domain.IntegrationTests` or
  the system tests instead.
- Follow the unit test file shape defined in the root `AGENTS.md`.
