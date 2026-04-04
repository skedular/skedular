# Booking Domain Integration Test Notes

This file applies to `booking/domain/Booking.Domain.IntegrationTests`.

## Purpose

- Keep a fast, booking-domain-focused place for billing scenarios that need the real booking DB, real booking services,
  and direct state seeding.
- Treat non-booking dependencies as explicit test seams here.

## Boundary

- If the goal is to validate booking-domain billing logic with deterministic stubs for organization/core/Xero/Stripe
  edges, add the test here.
- If the goal is to validate true cross-domain behavior end to end, add the test to `system/Skedular.SystemTests`
  instead.

## Test Structure

- Seed state through booking repositories first.
- Trigger behavior through real boundaries only.
- Prefer booking API clients first.
- If a workflow is involved, start it through an API call, a Kafka message, or another real entry point into the running
  domain.
- Assert persisted booking-domain outcomes, not just status codes.
- When a fake external dependency is involved, configure it through the generated `InfrastructureTestService` client
  exposed by `Booking.Domain.FakeDependencies`, rather than direct access to in-memory fake state.
- Prefer scenario-style control API calls over low-level per-method fake setup.
- If a test needs to inspect outbound fake calls, read recorded requests through the control API and only clear them
  explicitly when the scenario requires it.

## Fake Dependency Usage

- These tests currently share one `Booking.Domain.FakeDependencies` instance for the project run.
- Keep tests in the same xUnit collection unless the project-level parallelism rules are changed deliberately.
- Use the printed `pgadmin` and `kafka-ui` endpoints from startup when local debugging needs direct DB or Kafka access.

## Dependency Rule

- Do not let these tests depend on live external services.
- If non-booking dependencies must be stubbed, stub them at the boundary of the running domain rather than by
  instantiating internal services directly.
