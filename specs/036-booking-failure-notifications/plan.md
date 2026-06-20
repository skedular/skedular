# Implementation Plan: Booking Failure Communications

**Branch**: `036-booking-failure-notifications` | **Date**: 2026-07-22 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/036-booking-failure-notifications/spec.md`

## Summary

Make marketplace booking outcomes durable, unambiguous, and communicable without replacing the existing booking/payment architecture. The booking domain will atomically claim final availability through an EF-managed serializable transaction with bounded retry, retain classified failure outcomes and delivery records, expose them in booking history/details as the in-app surface, and use retry-safe asynchronous delivery for customer plus authorized stakeholder email communications. Existing one-time resource release and subscription-cycle release workflows are reused; missing availability-conflict finalization, notification idempotency, and user-visible failure explanations are added.

## Technical Context

**Language/Version**: C#/.NET 10; TypeScript 6.0.3
**Primary Dependencies**: Booking domain services/repositories, Entity Framework Core/PostgreSQL, Temporal, Kafka outbox, HotChocolate/Fusion GraphQL, React 19/Next.js 16/Relay 21
**Storage**: Booking-owned PostgreSQL; existing Kafka/Temporal outboxes for reliable downstream work
**Testing**: xUnit/FakeItEasy unit tests; booking integration tests; Temporal workflow/activity tests; Vitest/React Testing Library
**Target Platform**: Skedular backend services and marketplace web application
**Project Type**: Domain-backed web application and asynchronous workflow system
**Performance Goals**: Final allocation returns a confirmed or classified final conflict within the normal booking submission interaction; notification delivery is asynchronous and must not delay the outcome.
**Constraints**: Preserve domain ownership; use repository-owned EF-managed serializable allocation with bounded retry and no raw SQL; do not hand-edit generated contracts/artifacts; no duplicate notification per failure/recipient/channel; American English user copy.
**Scale/Scope**: Marketplace one-time bookings, initial multi-day/series creation, and subscription recurring occurrences for Spaces and Host; authorized customers plus Spaces and Host owners/administrators.

## Constitution Check

_Pre-design and post-design review: PASS._

- [x] **I. Contract-First** — Booking GraphQL types/queries will change. Add source schema/resolvers first, run `scripts/generate-graphql.sh`, and regenerate Relay artifacts; no exported schema or Relay artifact will be edited directly.
- [x] **II. Domain Boundaries** — Booking owns failure outcomes, allocation, workflows, and delivery records. Organization/customer recipient data is already replicated in Booking; no cross-domain database access is required. Existing Kafka/Temporal public outbox interfaces are reused.
- [x] **III. Testing** — Unit tests cover classification, idempotency, recipient selection, and UI messages. Integration tests cover repository allocation and durable failure records through repository assertions. Workflow/activity tests cover payment and recurrence cleanup; concurrency tests cover competing claims.
- [x] **IV. Frontend** — Web changes use collocated Relay fragments/operations and generated artifacts. Feature components use `@skedular/ui` typography wrappers and American English copy.
- [x] **V. Pattern Consistency** — A booking-owned outcome/event aggregate follows the existing marketplace refund/event model. Repository-level slot locking is required because the current read-then-attach allocation path permits a race; a new general notification platform is intentionally avoided.
- [x] **VI. Logging** — Allocation start/claim/conflict, outcome finalization, capacity release, dispatch/retry, and delivery completion/failure will use structured logs with correlation and booking/series identifiers and without sensitive payloads.

## Project Structure

### Documentation

```text
specs/036-booking-failure-notifications/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
└── contracts/
    └── booking-failure-notifications.md
```

### Source Changes

```text
src/booking/
├── apis/Booking.Api/
│   ├── GraphQL/Booking/              # failure outcome types, fields, mutations/query shapes
│   └── Services/                     # public submission outcome mapping
├── shared/Booking.Shared/
│   ├── Activities/                   # workflow-safe finalization and notification delivery
│   ├── Database/Entities/            # booking failure, event, and delivery entities/configuration
│   ├── Database/Migrations/          # generated migration from entity model
│   ├── Repositories/                 # atomic slot claim and failure/delivery repositories
│   ├── Services/                     # classification, recipient/template, finalization services
│   ├── Workflows/                    # retry-safe notification workflow if required
│   └── Workflows/                    # payment and recurring cleanup integration
└── shared/Booking.Shared.*Tests/     # unit, integration, and workflow/activity coverage

src/web/apps/webapp/src/
├── components/marketplaceProductBooking/       # final outcome and rebook action
├── components/marketplaceProductSubscription/  # cycle/occurrence outcome presentation
└── components/booking/                         # retained failure history surface

api-definitions/graphql/skedular/v1/            # regenerated only by scripts/generate-graphql.sh
```

**Structure Decision**: Keep all lifecycle ownership in Booking. Reuse the marketplace web app for customer and stakeholder presentation; do not introduce a separate notification domain or direct database coupling to Organization/Customer.

## Complexity Tracking

| Decision | Why Needed | Simpler Alternative Rejected Because |
| --- | --- | --- |
| Repository atomic slot claim | The existing availability read occurs before association writes and can let concurrent buyers claim the same capacity. Use an EF-managed serializable transaction with bounded retry, not raw SQL. | Rechecking in the service remains a check-then-write race. |
| Booking failure/event/delivery aggregate | Final state, audit history, and per-recipient idempotency must survive workflow retries. | Overloading payment status or directly emailing after commit loses reason/history and can duplicate sends. |
| Feature-local in-app failure surface | No durable generic in-app notification service exists; retained booking history/details is the agreed in-app surface. | Toasts disappear and do not meet retained-history requirements. |
