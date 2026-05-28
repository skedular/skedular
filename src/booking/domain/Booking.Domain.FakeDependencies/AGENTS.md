# Booking Domain Fake Dependencies Notes

This file applies to `booking/domain/Booking.Domain.FakeDependencies`.

## Purpose

- This project hosts booking-owned fake external dependencies for booking-domain integration tests.
- It provides real protocol endpoints that the running booking domain can call over gRPC.
- It also hosts the booking-owned test-control API for configuring scenarios and inspecting recorded requests.

## Boundary

- Put booking-specific fake dependency behavior here.
- Put booking-specific fake scenarios here.
- Do not move generic test helpers here if they can live in `shared/Testing.Shared.IntegrationTests`.
- Do not move generic local infrastructure bootstrapping here if it belongs in `shared/Infrastructure.Shared`.

## Control API

- Keep the control API scenario-oriented.
- Prefer configuring domain scenarios over exposing one RPC per fake dependency method.
- Keep `Reset` for full fake-state reset.
- Keep narrower operations such as clearing only recorded requests when that is enough for a test step.

## Assertion Model

- Record inbound requests from the running booking domain.
- Make recorded requests useful for async assertions by including stable metadata such as timestamps, counts, and key
  identifiers.
