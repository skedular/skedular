# Booking Domain Test Agent Notes

This file covers the `booking/domain/` area.

## What This Area Is For

- `Booking.Domain.AppHost`
    - Aspire app host for the booking domain
- `Booking.Domain.FakeDependencies`
    - booking-owned fake external dependency host for booking integration tests
- `Booking.Domain.IntegrationTests`
    - booking-domain-scoped integration tests

## Important Testing Boundary

Not every "integration" scenario belongs here.

For simple booking-domain tests, this project is fine.

For real cross-domain billing scenarios involving:

- real API calls
- Temporal workflows
- multiple domains
- email/core/org/gateway interactions

prefer `system/Skedular.SystemTests` instead.

## Guidance From Recent Billing Work

The recurring in-arrears billing scenarios were a bad fit for booking-domain integration tests when fake services were
introduced to make them pass.

Preferred rule:

- if validating pure domain-local behavior, booking-domain integration tests are acceptable
- if validating real arrears workflow behavior end to end, use system tests

## Test Style

For DI-backed integration tests:

- use constructor injection
- use the shared service-registration patterns already used by the project
- prefer repository factories over manually creating raw db contexts in the test body
- use the normal `CancellationToken` parameter pattern

## Aspire App Host Rule

- Keep booking dependency readiness in `Booking.Domain.AppHost/AppHost.cs`.
- If a booking resource references Kafka, Temporal, Redis, the booking database, shared infrastructure, or fake
  dependencies, add the matching `WaitFor(...)` or `WaitForCompletion(...)` there instead of polling for it in
  integration-test startup.

## Anti-Pattern To Avoid

- writing a planner-only or service-only test and calling it an integration test
- introducing fake external services when the system test harness already exists for the full scenario
