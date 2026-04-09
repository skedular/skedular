# Core API Unit Tests Notes

This file applies to `core/apis/Core.Api.UnitTests`.

## Purpose

- Unit tests for the core API layer.
- Fast, in-process tests that do not require running infrastructure.

## Test File Shape

- One test class/file per public method under test.
- Order test method parameters: frozen/injected constructor dependencies → `sut` → random inputs and expected values.
- Prefer injected test inputs over hardcoded strings unless testing a specific literal contract.

## Agent Rule

- Keep tests fast and infrastructure-free.
- If a test requires real infrastructure (database, Kafka, Temporal), move it to the domain integration test project.
