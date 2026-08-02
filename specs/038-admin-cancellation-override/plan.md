# Implementation Plan: Admin Cancellation Policy Override

**Branch**: `038-admin-cancellation-override` | **Date**: 2026-08-02 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/038-admin-cancellation-override/spec.md`

## Summary

Introduce a server-authoritative cancellation actor model so product-owning Spaces/Host owners and administrators with existing booking/subscription management permission can override customer cancellation eligibility for marketplace bookings and subscriptions. The override requires a short reason, is audited, preserves immediate versus period-end semantics, and does not bypass the separate provider-specific refund workflows: Stripe remains automatic when eligible, while bank-transfer and Xero refunds retain approval and settlement controls.

## Technical Context

**Language/Version**: C# .NET 10; TypeScript 6 / React 19 / Next.js 16 for affected web surfaces  
**Primary Dependencies**: Booking.Shared cancellation/refund services, Booking.Api GraphQL mutations, existing organization authorization, MarketplaceRefundAdminService, Stripe/Xero/bank-transfer refund workflows, Relay 21  
**Storage**: Booking-owned PostgreSQL persistence for cancellation/refund audit data and existing MarketplaceRefund state; migration required if current audit/refund entities cannot represent actor, override, and reason fields  
**Testing**: xUnit + AutoFixture + FakeItEasy unit tests; focused Booking integration tests for persistence/concurrency/schema wiring; Vitest + React Testing Library for changed web flows  
**Target Platform**: Cloud-hosted Booking API and web applications  
**Project Type**: Full-stack web service  
**Performance Goals**: Cancellation authorization and policy decision must remain within the existing cancellation request latency budget; provider processing remains asynchronous where currently asynchronous  
**Constraints**: Server-authoritative authorization; product-owning organization boundary; no client-provided bypass flags; provider approval rules preserved; no direct EF in integration tests; generated GraphQL/Relay outputs regenerated, never hand-edited  
**Scale/Scope**: Marketplace bookings and subscriptions for Spaces and Host organizations; unrelated team, invitation, and non-marketplace cancellation flows are out of scope

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — GraphQL mutation/input and response surfaces may change. Source changes belong in Booking API GraphQL definitions/classes, followed by `scripts/generate-graphql.sh` and affected Relay generation; generated schema/artifacts are not hand-edited.
- [x] **II. Domain Boundaries** — Cancellation and refund ownership remain in Booking. Organization authorization is consumed through the existing public authorization/service path; no cross-domain database access is planned. New actor/state values use shared Booking models and explicit mappings.
- [x] **III. Testing** — Unit tests cover actor resolution, authorization, reason validation, policy bypass, modes, idempotency, and provider routing. Focused integration tests cover persistence/audit concurrency and schema wiring only; repository-layer assertions are required.
- [x] **IV. Frontend** — Operator and customer cancellation surfaces may change in `webapp` and `webapp-spaces`; Relay fragments remain collocated, generated artifacts are regenerated, shared typography wrappers are used, and copy uses American English.
- [x] **V. Pattern Consistency** — The feature extends existing cancellation, refund, authorization, audit, and provider automation patterns. The explicit actor is a shared service model because both booking and subscription paths need the same server-side decision.
- [x] **VI. Logging** — Add structured logs for actor resolution, permission outcome, policy evaluation/override, reason validation, cancellation transitions, refund routing, provider approval waits, failures, retries, and recovery.

## Project Structure

### Documentation

```text
specs/038-admin-cancellation-override/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── cancellation-graphql.md
```

### Source Code

```text
src/booking/shared/Booking.Shared/
├── Models/                         # cancellation actor/outcome models if required
├── Services/
│   ├── MarketplaceBookingService.cs
│   ├── MarketplaceBookingSubscriptionService.cs
│   ├── MarketplaceRefundService.cs
│   └── MarketplaceRefundAutomationService.cs
└── Database/                       # audit/refund persistence and migration if required

src/booking/apis/Booking.Api/
├── GraphQL/Booking/                # booking cancellation input/mutation/payload
├── GraphQL/MarketplaceBookingSubscription/ # subscription cancellation input/mutation
├── Services/                       # authorization/context and refund admin boundaries
└── UnitTests/                      # GraphQL/service authorization tests

src/web/apps/webapp/src/            # customer cancellation affordances/messages
src/web/apps/webapp-spaces/src/     # operator cancellation UI, reason capture, refund status
```

**Structure Decision**: Extend the existing Booking domain and its two web consumers. Do not create a new service or persistence boundary. Keep refund processing in the existing refund aggregate/automation/admin workflows and keep authorization at the API-to-shared-service boundary.

## Phase 0: Research Summary

See [research.md](research.md). All specification clarifications are resolved. The key implementation constraint is that cancellation override and refund processing are separate decisions, with provider-specific approval behavior preserved.

## Phase 1: Design Summary

- [data-model.md](data-model.md) defines the actor, request, audit, and provider relationship requirements.
- [contracts/cancellation-graphql.md](contracts/cancellation-graphql.md) defines the client-facing mutation and error behavior.
- [quickstart.md](quickstart.md) defines acceptance validation for customer, operator, unauthorized, subscription-mode, provider, and idempotency scenarios.

## Post-Design Constitution Check

- [x] Contract-first generation path identified.
- [x] Booking owns cancellation/refund behavior; organization permission is consumed through the existing boundary.
- [x] Unit-first and focused integration testing strategy defined.
- [x] Frontend generated-artifact and typography constraints identified.
- [x] Existing cancellation/refund patterns extended without introducing a parallel workflow.
- [x] Logging requirements cover authorization, policy, lifecycle, provider boundaries, and failures.

## Complexity Tracking

No constitution violations requiring justification.
