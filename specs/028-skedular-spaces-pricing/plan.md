# Implementation Plan: Skedular Spaces Pricing Implementation

**Branch**: `028-skedular-spaces-pricing` | **Date**: 2026-06-14 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/028-skedular-spaces-pricing/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Implement Skedular Spaces pricing, subscription, booking-instance quota, entitlement, offering-level discounts, and web upgrade prompting by using the existing Organization pricing catalog and offering model with an independent Spaces catalog version. Organization remains the owning domain for product offering/catalog state, subscription assignment, billing discount state, and admin/workaround offering updates, while Booking owns booking-instance quota usage and enforcement across one-off, multi-slot, recurring, subscription-generated, and admin-created booking creation paths. The Spaces frontend consumes server-driven catalog, quota status, and upgrade/contact prompts without adding new checkout or subscription mutation flows beyond existing supported subscription paths.

## Technical Context

**Language/Version**: C# .NET 10 backend; TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router frontend  
**Primary Dependencies**: HotChocolate/Fusion GraphQL, REST/OpenAPI definitions where needed, EF Core/PostgreSQL, Temporal workflows, Kafka/protobuf organization events if subscription state changes need projection, Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, existing Organization pricing catalog services, existing Booking private/recurring booking workflows  
**Storage**: PostgreSQL via Organization-owned EF Core persistence for subscription/catalog state and offering-level discount percentage; Booking-owned EF Core booking rows for monthly booking-instance usage counts
**Testing**: .NET unit tests for catalog mapping, subscription assignment, entitlement decisions, booking quota calculation, and rollover behavior; integration tests for Booking persistence/workflows, Organization GraphQL/catalog surfaces, and cross-domain subscription projection; Vitest/React Testing Library for Spaces quota/pricing UI; generated schema/artifact validation through existing generator scripts  
**Target Platform**: Existing Skedular backend services and web apps in the monorepo, primarily `src/organization`, `src/booking`, `src/shared/Api.Shared.Services`, `src/web/apps/webapp-spaces`, generated GraphQL/OpenAPI surfaces, and Temporal workers  
**Project Type**: Cross-domain web-service plus frontend feature  
**Performance Goals**: Booking quota entitlement checks complete within 100 ms p95 for create workflows in automated validation; usage reads for booking creation are close to real time by counting persisted Booking rows for the current billing period, with minor concurrent overage accepted as an explicit simplicity tradeoff; pricing catalog reads complete within 500 ms p95 under normal product-page load  
**Constraints**: Use product-specific catalog versions (`TEAMS_V1` for Teams and `SPACES_V1` for Spaces) on the shared Organization catalog/offering infrastructure; do not hardcode frontend pricing values; offering-level discounts default to 0 and are applied at billing without mutating catalog prices or quotas; no quotas on locations/resources/desks/rooms/equipment/products/customers/subscriptions/memberships; generated GraphQL/OpenAPI/Relay artifacts must not be hand-edited; integration tests assert persistence through repositories, not raw `DbContext`; user-facing copy uses American spelling; billing periods use UTC day boundaries and first-day-of-month rollover using the existing Teams Temporal activity pattern; current-period quota counts only booking instances scheduled within that billing period
**Scale/Scope**: Spaces Free/Growth/Business/Contact Us catalog entries, monthly booking-instance quota enforcement, default Free assignment for existing organizations, Enterprise/admin override support, quota status/upgrade prompts in Spaces, and recurring booking-instance enforcement; full new checkout/subscription mutation UI and non-booking-resource quotas are out of scope

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature is expected to touch GraphQL schema and may touch OpenAPI if pricing/quota endpoints are exposed through REST. Source definitions and server code must be updated first, then `scripts/generate-graphql.sh`; if OpenAPI YAML changes, run `api-definitions/openapi/generate.sh` and `src/web/apps/webapp/scripts/generate.sh` where consumed. Generated GraphQL schemas, Relay artifacts, and OpenAPI clients must not be hand-edited.
- [x] **II. Domain Boundaries** — Organization owns catalog/subscription assignment; Booking owns booking-instance creation, usage counting, quota status, and quota enforcement. Cross-domain state must flow through public GraphQL/OpenAPI/gRPC/event contracts or local projected organization state, not direct database access.
- [x] **III. Testing** — Backend unit tests are required for catalog, subscription, entitlement, usage counting, quota rejection, out-of-period exclusion, and recurring generation behavior. Persistence, GraphQL/OpenAPI, and cross-domain projection paths require integration tests using repository-layer assertions.
- [x] **IV. Frontend** — Spaces frontend changes are included. Relay fragments must be collocated where GraphQL is used, generated artifacts must be regenerated, typography wrappers must come from `@skedular/ui`, shared runtime helpers from `@skedular/shared`, and all customer/operator copy uses American spelling.
- [x] **V. Pattern Consistency** — The feature uses the existing Organization catalog/offering ownership and first-day-of-month Temporal activity pattern, while keeping Spaces on its own `SPACES_V1` catalog version. Justification: Spaces follows the Teams implementation pattern but can evolve independently.
- [x] **VI. Logging** — Structured logging is required for catalog filtering, subscription assignment/defaulting, quota allow/block decisions, booking create rollback decisions, recurring instance enforcement, monthly rollover, and failure/recovery paths.

## Project Structure

### Documentation (this feature)

```text
specs/028-skedular-spaces-pricing/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── graphql-spaces-pricing.md
│   ├── openapi-spaces-pricing.md
│   └── entitlement-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/shared/Api.Shared.Services/
├── Models/
│   └── shared offering, entitlement, and quota decision DTOs
└── Offering/
    └── evolved pricing entitlement evaluator and reason codes

src/organization/shared/Organization.Shared/
├── Models/PricingCatalog/
│   └── Spaces product offering, plan, version, and name mappings
├── Services/Pricing/
│   └── Spaces catalog provider and V1 mapping extensions
└── Database/Entities/
    └── existing organization offering/subscription state

src/organization/apis/Organization.Api/
└── GraphQL/Pricing/
    └── Spaces pricing catalog and subscription read/update surfaces where required

src/booking/shared/Booking.Shared/
├── Models/
│   └── replicated Spaces plan/quota period state
├── Repositories/
│   └── booking-row usage count repository methods for current-period quota reads
├── Services/
│   └── Spaces entitlement and booking quota services
└── Workflows/
    └── recurring booking quota integration

src/booking/apis/Booking.Api/
├── GraphQL/Booking/
│   └── Booking-owned Spaces quota status and quota error surfaces
└── Services/
    └── private booking creation paths enforce quota before creating new instances

src/web/apps/webapp-spaces/
├── src/components/
│   └── pricing/quota/upgrade prompt surfaces
└── generated Relay/OpenAPI artifacts if consumed

api-definitions/
├── graphql/skedular/v1/schema.graphql
└── openapi/skedular/*/*.yaml when REST contracts change
```

**Structure Decision**: Use Organization as the owning domain for Spaces catalog and subscription assignment because it already owns product-aware pricing catalog and organization offering state. Use Booking as the owning domain for booking-instance quota usage and quota status because booking instance creation, scheduled instance timing, cancellation semantics, recurring generation, and failure rollback are booking-owned behavior. Shared DTOs/reason codes/evaluators live in `Api.Shared.Services` only where they are cross-domain contracts. Spaces frontend consumes Organization-driven catalog/subscription data and Booking-driven quota status instead of maintaining pricing constants.

## Complexity Tracking

No constitution violations requiring complexity exceptions. The cross-domain design follows existing Organization pricing ownership and Booking workflow ownership rather than introducing a separate pricing service or direct cross-domain persistence access.

## Phase 0: Research

See [research.md](./research.md).

## Phase 1: Design and Contracts

See [data-model.md](./data-model.md), [contracts/graphql-spaces-pricing.md](./contracts/graphql-spaces-pricing.md), [contracts/openapi-spaces-pricing.md](./contracts/openapi-spaces-pricing.md), [contracts/entitlement-contract.md](./contracts/entitlement-contract.md), and [quickstart.md](./quickstart.md).

## Post-Design Constitution Check

- [x] **I. Contract-First** — Contracts identify GraphQL as the primary surface and OpenAPI as optional/secondary. Regeneration commands are documented in quickstart.
- [x] **II. Domain Boundaries** — Data model keeps catalog/subscription ownership in Organization and booking usage/status/enforcement ownership in Booking.
- [x] **III. Testing** — Quickstart defines unit, integration, and web validation paths, including repository-layer persistence assertions.
- [x] **IV. Frontend** — Web plan keeps Spaces pricing/quota UI server-driven, with generated artifact discipline and `@skedular/ui`/`@skedular/shared` boundaries.
- [x] **V. Pattern Consistency** — Research documents product-specific catalog versions while keeping Booking usage derived from booking rows instead of a separate usage table or JSON counter.
- [x] **VI. Logging** — Data model and quickstart include observable decisions and log verification.
