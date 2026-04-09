# Booking API Project Notes

This file applies to `booking/apis/Booking.Api`.

## Purpose

- This is the runnable API host for the booking domain.
- It serves HTTP, GraphQL (HotChocolate subgraph), and gRPC endpoints.
- Configuration, DI registration, middleware pipeline, and application startup live here.

## Agent Rule

- Read the parent `booking/apis/AGENTS.md` for API-layer rules.
- Keep `Program.cs` / `Extensions.cs` focused on host wiring; move business logic to `booking/shared/Booking.Shared`.
- Do not add application-layer business logic directly to controllers; delegate to shared services.
- Run `scripts/generate-graphql.sh` after any schema type or field change.
