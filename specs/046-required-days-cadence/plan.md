# Implementation Plan: Required Days Across Longer Cadences

**Branch**: `046-required-days-cadence` | **Date**: 2026-09-02 | **Spec**: [spec.md](spec.md)

## Summary

Extend `ProductPricing.RequiredDaysPerWeek` from weekly-only offers to supported purchase cadences longer than one week. Spec 047 removes sub-day cadences and makes credit entitlements cadence-free. Scheduled reservations/subscriptions use customer-selected weekdays and generate one booking occurrence per selected weekday in each applicable complete UTC week; entitlements use their validity period and allow at most N successful redemptions per complete UTC week. No booking or location timezone is stored or consulted.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6 / React 19 / Next.js 16
**Primary Dependencies**: EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Kafka/protobuf events, Temporal, Relay, Vitest
**Storage**: Existing ProductPricing projections; Booking-owned entitlement redemption history/usage query with concurrency protection
**Testing**: xUnit + AutoFakeItEasyData; repository-backed integration tests; Vitest + React Testing Library
**Target Platform**: Marketplace/Booking APIs, processors, workflows, and Host/Spaces web editors
**Project Type**: Full-stack web service
**Performance Goals**: Weekly eligibility queries use indexed, bounded entitlement/booking history lookups.
**Constraints**: Supported purchase cadences are Daily, Weekly, Fortnightly, Monthly, TwoMonths, Quarterly, FourMonths, FiveMonths, SixMonths, and Yearly; the setting is hidden for Daily and cadence-free entitlements use validity periods. UTC weeks; no raw EF outside repositories; generated artifacts are never hand-edited.
**Scale/Scope**: Weekly and longer purchase cadences, scheduled reservations/subscriptions, and cadence-free credit entitlements.

## Constitution Check

- [x] Contract-first: preserve existing fields where possible; regenerate GraphQL/protobuf/Relay outputs only from source definitions.
- [x] Domain boundaries: Marketplace owns pricing; Booking owns booking and entitlement enforcement through services/repositories and events.
- [x] Testing: unit-first; integration tests cover persistence and concurrent redemption boundaries.
- [x] Frontend: update Host/Spaces editors and customer surfaces with Relay-safe mutations and American English copy.
- [x] Pattern consistency: one UTC-week calculator and explicit lifecycle counting rules.
- [x] Logging: structured logs cover validation, schedule generation, blocked redemption, release, retry, and recovery decisions.

## Phase 0: Research

See [research.md](research.md). Spec 047 is the authority for the supported cadence set and cadence-free entitlement model. Existing ProductPricing, GraphQL, protobuf, JSON, event mappers, and editors already carry the nullable field; new work is enforcement and UI gating.

## Phase 1: Design

See [data-model.md](data-model.md), [contracts/required-days.md](contracts/required-days.md), and [quickstart.md](quickstart.md).

## Project Structure

Backend changes belong in `src/marketplace/apis/Marketplace.Api`, `src/marketplace/shared/Marketplace.Shared`, `src/booking/shared/Booking.Shared`, `src/booking/processors/Booking.Processors`, and Booking integration tests. Frontend changes belong in `src/web/apps/webapp-host`, `src/web/apps/webapp-spaces`, and affected customer components under `src/web/apps/webapp`; generated Relay/schema outputs are regenerated.

## Complexity Tracking

No constitution violations.
