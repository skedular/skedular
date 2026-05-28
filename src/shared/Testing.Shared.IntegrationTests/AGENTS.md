# Testing.Shared.IntegrationTests Agent Notes

This file applies to everything under `shared/Testing.Shared.IntegrationTests`.

## Purpose

- This project holds reusable helpers for domain integration tests.
- Keep it focused on test infrastructure that can be shared across multiple domain integration test projects.

## What Belongs Here

- Polling and eventual-consistency helpers used by integration tests.
- Shared transport helpers for test clients, such as gRPC channel creation.
- Generic Aspire/testing bootstrap helpers that are not domain-specific.
- Reusable HTTP or test-host utilities that do not belong to one domain.

## What Does Not Belong Here

- Domain-specific fake dependency behavior.
- Fake external service implementations that are owned by domain-local fake dependency hosts.
- Test scenarios that only make sense for one domain.

## Design Rules

- Prefer small, composable helpers over large framework-like abstractions.
- If a helper depends on a specific fake dependency behavior, keep that behavior in the owning domain fake dependency
  project and keep only the generic test-side utility here.
- If a helper is likely to be reused by multiple integration test projects, prefer adding it here instead of repeating
  it in each project startup.
- Keep these helpers transport-correct but lightweight. They should support real HTTP/gRPC interaction without hiding
  too much of the underlying test flow.
- Integration tests using these helpers should inspect persisted state through repository methods, not by reaching for
  `DbContext`/Entity Framework directly inside the test classes.

## Current Patterns

- `Eventually` is the shared polling primitive for async integration assertions.
- `GrpcChannelFactory` is the shared gRPC client channel helper.
- `HttpClientExtensions` contains reusable readiness/wait helpers for started test hosts.

## Aspire Rule

- Domain app hosts should own dependency readiness through Aspire resource wiring such as `WithHttpHealthCheck(...)`,
  `WaitFor(...)`, and `WaitForCompletion(...)`.
- If a project uses `.WithReference(...)` to a dependency in an app host, prefer adding the matching Aspire
  `WaitFor(...)` edge in that same app host.
- Do not add extra liveness/readiness polling inside each integration-test startup unless there is a proved gap that
  cannot be expressed in the app host.
