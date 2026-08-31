# Implementation Plan: Required Days Across Longer Cadences

**Branch**: `046-required-days-cadence` | **Date**: 2026-08-31 | **Spec**: [spec.md](spec.md)

## Summary

Extend nullable `ProductPricing.RequiredDaysPerWeek` to all purchase cadences longer than one week. Reservations and subscriptions require exactly N bookings per complete UTC calendar week; credit entitlements allow at most N confirmed redemptions per complete UTC week. `availableDays` remains the allowed weekday set and boundary partial weeks are exempt.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6 / React 19 / Next.js 16
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Kafka/protobuf events, Temporal, Relay, Vitest
**Storage**: Existing ProductPricing JSON/event projections; Booking-owned durable entitlement/redemption history query or record
**Testing**: xUnit + AutoFakeItEasyData; repository-backed integration tests; Vitest + React Testing Library
**Target Platform**: Skedular backend APIs, processors, workflows, and web product editors
**Project Type**: Full-stack web service
**Performance Goals**: Weekly checks use bounded indexed history queries.
**Constraints**: UTC weeks; no raw EF outside repositories; generated artifacts are regenerated, never hand-edited.
**Scale/Scope**: All existing longer-than-weekly purchase cadences and both fulfillment types.

## Constitution Check

- [x] Contract-first: preserve existing fields; regenerate any affected GraphQL/event/Relay outputs.
- [x] Domain boundaries: Marketplace owns pricing; Booking owns enforcement and redemption persistence through services/repositories.
- [x] Testing: unit-first, with integration tests for durable counting and concurrency.
- [x] Frontend: update Host/Spaces editors, Relay operations, and customer-facing documentation as needed.
- [x] Pattern consistency: one shared UTC-week calculation path.
- [x] Logging: configuration, eligibility, rejected redemption, and subscription-generation decisions are structured and non-sensitive.

## Phase 0: Research

See [research.md](research.md). Existing model, GraphQL input, protobuf field, JSON serialization, event mappers, and editors already carry the value. The gap is longer-cadence enforcement and aggregate entitlement counting.

## Phase 1: Design

See [data-model.md](data-model.md), [contracts/required-days.md](contracts/required-days.md), and [quickstart.md](quickstart.md).

## Project Structure

Backend: `src/shared/Api.Shared.Services`, `src/marketplace/apis/Marketplace.Api`, `src/marketplace/shared/Marketplace.Shared`, `src/booking/shared/Booking.Shared`, and Booking integration tests. Frontend: product editors under `src/web/apps/webapp-host` and `src/web/apps/webapp-spaces`, with generated Relay artifacts.

## Complexity Tracking

No constitution violations.
