# Implementation Plan: Customer Readiness Tracking

**Branch**: `016-customer-readiness-tracking` | **Date**: 2026-05-24 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `specs/016-customer-readiness-tracking/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Move federated customer readiness out of the authenticated request hot path by adding a customer-owned public Kafka event
topic, `customer_readiness`, and a customer-domain readiness aggregate. Booking, organisation, team, marketplace, and
location will publish `CustomerIdentityProvisioned` after their existing customer source-event subscribers durably
upsert local customer identity. Customer processors will consume the readiness topic, record per-domain state in a
collection keyed by the readiness domain enum, derive the central activating/active status, and expose that single
state to backend readiness/auth checks. Existing customers without central readiness state are blocked until an
operator manually triggers customer synchronisation/backfill.

## Technical Context

**Language/Version**: C# .NET 10 for customer/booking/organisation/team/marketplace/location APIs, processors, shared libraries, and integration tests; protobuf event definitions under `api-definitions/events/skedular`  
**Primary Dependencies**: Existing Kafka protobuf event envelope and metadata companions, Enterprise.Shared Kafka producer/consumer/outbox infrastructure, EF Core repositories and migrations, customer workaround republish API, HotChocolate/Fusion GraphQL/auth integration points where readiness gating is enforced, Microsoft/Serilog structured logging conventions  
**Storage**: PostgreSQL via EF Core in `customer/shared/Customer.Shared`; add central readiness state tables/entities and repository methods. Participating domains reuse existing local customer/identity persistence.  
**Testing**: xUnit unit tests for event mapping, readiness derivation, idempotency, publishers, subscribers, auth/readiness lookup, and logging; domain integration tests for Kafka/event processing and customer readiness persistence using repository-layer assertions; generated contract build checks after event regeneration  
**Target Platform**: Skedular backend domain APIs and processors, Kafka event bus, customer-domain PostgreSQL persistence, federated GraphQL access checks, operator-facing customer synchronisation/backfill endpoint  
**Project Type**: Cross-domain backend event-contract and persistence feature with authenticated/federated access gating changes  
**Performance Goals**: Normal authenticated/federated readiness checks perform one customer-domain readiness lookup and no runtime fan-out to booking, organisation, team, marketplace, or location. Readiness event handlers remain idempotent under repeated backfill/replay events.  
**Constraints**: Preserve contract-first generation; do not hand-edit generated protobuf outputs; `CustomerIdentityProvisioned` payload contains only `customerId` and `domain`; domain enum has no unspecified/unknown/none/customer values; missing readiness blocks access; no backward-compatible legacy fan-out fallback; integration tests must not query EF directly  
**Scale/Scope**: New `customer_readiness` topic/version; handwritten metadata companion; readiness publisher registration in booking, organisation, team, marketplace, and location processors; customer readiness subscriber and persistence; central required-domain service; replacement of backend hot-path fan-out readiness checks; manual customer synchronisation/backfill path

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

Answer each gate. If a gate fails, resolve the issue before proceeding.

- [x] **I. Contract-First** — This feature adds event protobufs under `api-definitions/events/skedular` and handwritten
      metadata under `shared/Api.Shared.Clients/Events/Skedular`. Run `api-definitions/events/generate.sh` after
      editing event definitions; do not check in generated `*V1Key.g.cs` or `*V1Value.g.cs` outputs.
- [x] **II. Domain Boundaries** — Cross-domain readiness flows through the customer-owned public `customer_readiness`
      topic. Participating domains only publish their own durable provisioning result and never read customer-domain
      readiness tables directly.
- [x] **III. Testing** — Unit tests are required for publishers, subscribers, readiness derivation, duplicate/replay
      handling, missing-domain pending behaviour, and the auth/readiness lookup. Persistence and Kafka processing
      require integration tests with repository-layer assertions, not raw `DbContext`.
- [x] **IV. Frontend** — Web UI is only in scope if the existing readiness/auth UX needs copy or blocking-state updates.
      Any changed copy must use British spelling, and any Relay or generated web artefacts must be regenerated rather
      than hand-edited.
- [x] **V. Pattern Consistency** — The design extends existing protobuf event, metadata companion, Kafka subscriber,
      outbox publisher, repository, and migration patterns. The only new shared concept is the customer-owned readiness
      aggregate, justified by the need to remove runtime fan-out while preserving domain ownership.
- [x] **VI. Logging** — Structured logs are planned for participating-domain provisioning decisions, readiness publish
      success/failure, customer readiness event consumption, duplicate/replay outcomes, activating-to-active
      transitions, missing-state access blocking, manual synchronisation/backfill start/completion, and failure paths.

## Project Structure

### Documentation (this feature)

```text
specs/016-customer-readiness-tracking/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── customer-readiness-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
api-definitions/events/skedular/
├── customer_readiness_v1_key.proto
└── customer_readiness_v1_value.proto

shared/Api.Shared.Clients/Events/Skedular/
└── CustomerReadiness/V1/CustomerReadinessMetadata.cs

customer/
├── shared/Customer.Shared/{Database,Models,Repositories,Services}
├── shared/Customer.Shared.UnitTests/
├── processors/Customer.Processors/{Program.cs,Subscribers}
├── processors/Customer.Processors.UnitTests/
├── apis/Customer.Api/{Controllers,Services}
├── apis/Customer.Api.UnitTests/
└── domain/Customer.Domain.IntegrationTests/

booking/processors/Booking.Processors/Subscribers/CustomerSubscriber.cs
booking/processors/Booking.Processors.UnitTests/

organization/processors/Organization.Processors/Subscribers/CustomerSubscriber.cs
organization/processors/Organization.Processors.UnitTests/

team/processors/Team.Processors/Subscribers/CustomerSubscriber.cs
team/processors/Team.Processors.UnitTests/

marketplace/processors/Marketplace.Processors/Subscribers/CustomerSubscriber.cs
marketplace/processors/Marketplace.Processors.UnitTests/

location/processors/Location.Processors/Subscribers/CustomerSubscriber.cs
location/processors/Location.Processors.UnitTests/
```

**Structure Decision**: Add the public readiness event contract in `api-definitions/events/skedular` because events are
the contract source of truth. Keep generated protobuf outputs out of source control and add only handwritten metadata
companions. Persist central readiness inside the customer domain because customer owns the aggregate decision. Publish
readiness from existing non-customer `CustomerSubscriber` handlers immediately after durable local provisioning and
cache invalidation. Replace hot-path readiness fan-out with a customer-domain service/repository lookup.

## Complexity Tracking

No constitution violations requiring justification. The feature introduces one cross-domain public topic because the
current request-time fan-out is incompatible with reliable federated GraphQL execution; event-based readiness is the
simpler bounded alternative.

## Phase 0: Research

See [research.md](./research.md). Research resolves event contract shape, participating-domain inventory, persistence
shape, backfill rollout, readiness lookup replacement, and verification strategy without unresolved clarifications.

## Phase 1: Design & Contracts

See [data-model.md](./data-model.md), [contracts/customer-readiness-contract.md](./contracts/customer-readiness-contract.md),
and [quickstart.md](./quickstart.md).

## Post-Design Constitution Check

- [x] **I. Contract-First** — The contract artifact names the new event protobuf files, metadata companion location,
      and required `api-definitions/events/generate.sh` regeneration step.
- [x] **II. Domain Boundaries** — Data model and contract keep readiness owned by customer and use only public Kafka
      events between domains.
- [x] **III. Testing** — Quickstart and data model identify unit, processor, persistence integration, and auth lookup
      tests, including repository-layer assertions for readiness state.
- [x] **IV. Frontend** — No web implementation is required by the plan unless existing blocked-access UI must be
      updated; any such work remains subject to generated Relay and British-English copy rules.
- [x] **V. Pattern Consistency** — Research selects existing Kafka publisher/subscriber, outbox, EF repository, and
      event metadata companion patterns instead of a new framework or direct cross-domain reads.
- [x] **VI. Logging** — Research, data model, contract, and quickstart all include structured logging requirements for
      publish/consume, idempotency, activation transitions, access blocking, manual sync, and failures.
