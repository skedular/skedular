# Implementation Plan: Skedular Teams Pricing Catalog Redesign

**Branch**: `027-teams-pricing-catalogue` | **Date**: 2026-06-12 | **Spec**: [spec.md](./spec.md)
**Input**: Feature specification from `/specs/027-teams-pricing-catalogue/spec.md`

**Note**: This template is filled in by the `/speckit.plan` command. See `.specify/templates/plan-template.md` for the execution workflow.

## Summary

Redesign Skedular Teams pricing into a server-driven, product-aware pricing catalog while preserving existing organization offering behavior. The plan extends the existing Version 1 organization offering model instead of introducing a new pricing catalog version immediately: current code already has Free, Pay As You Go, Enterprise, Early Bird, monthly offering windows, organization offering renewal workflows, and GraphQL offering fields. The implementation will add catalog-shaped read models, capacity options, new subscription state support, and shared entitlement decisions around that existing model, leaving all existing subscriptions unchanged and creating a framework that can later represent Skedular Spaces without reusing marketplace product-pricing internals incorrectly.

## Technical Context

**Language/Version**: C# .NET 10 backend; TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 App Router frontend  
**Primary Dependencies**: HotChocolate/Fusion GraphQL, REST/OpenAPI definitions where needed, EF Core/PostgreSQL, Temporal workflows, Kafka/protobuf organization events, Relay 21, `react-relay`, MUI 9, `@skedular/ui`, `@skedular/shared`, existing public-web data/rendering modules  
**Storage**: PostgreSQL via Organization-owned EF Core persistence for new subscription/catalog state; existing domain projections in Team, Location, and Booking continue consuming public organization state/events and storing projected pricing/subscription state locally as JSON/projection data  
**Testing**: .NET unit tests for service/domain behavior; domain integration tests for persistence, GraphQL, OpenAPI, event propagation, and entitlement outcomes; Vitest/React Testing Library for web rendering; generated schema/artifact validation through existing generator scripts  
**Target Platform**: Skedular backend services and web apps in the existing monorepo, including `src/organization`, `src/team`, `src/location`, `src/booking`, `src/gateway`, and `src/web/apps/public-web`  
**Project Type**: Cross-domain web-service plus frontend feature  
**Performance Goals**: Pricing catalog reads complete within 500 ms p95 under normal product-page load; entitlement checks complete within 100 ms p95 for create/update workflows  
**Constraints**: Generated GraphQL/OpenAPI/Relay artifacts must not be hand-edited; `Api.Shared` portability rules still apply; integration tests must assert persistence through repositories, not raw `DbContext`; user-facing copy uses American spelling; existing subscriptions, including Early Bird, remain unchanged  
**Scale/Scope**: Teams pricing catalog, new Teams subscription state support, active-user quota, Free/Pay As You Go/Enterprise Capacity behavior, read-only Free/Early Bird compatibility checks, and framework-level Spaces catalog representation; full Spaces commercial behavior and existing-subscription migration/downgrade are out of scope

## Constitution Check

_GATE: Must pass before Phase 0 research. Re-check after Phase 1 design._

- [x] **I. Contract-First** — This feature is expected to touch GraphQL schema and may touch OpenAPI if public/REST catalog endpoints are exposed. Source definitions and server code must be updated first, then `scripts/generate-graphql.sh`; if OpenAPI YAML changes, run `api-definitions/openapi/generate.sh` and `src/web/apps/webapp/scripts/generate.sh` where consumed. Generated GraphQL schemas, Relay artifacts, and OpenAPI clients must not be hand-edited.
- [x] **II. Domain Boundaries** — Organization owns Teams subscription/catalog state and publishes public organization/subscription facts through existing events. Team, Location, and Booking must consume their local event-projected JSON/projection state and shared `Api.Shared.Services` enforcement code, not direct Organization database access or runtime calls back to Organization.
- [x] **III. Testing** — Backend unit tests are required for catalog, subscription, active-user quota, read-only compatibility, and entitlement services. Persistence, GraphQL/OpenAPI, Temporal renewal, Kafka/event projection, and cross-domain entitlement paths require integration tests using repository-layer assertions.
- [x] **IV. Frontend** — Public/web pricing changes are included. Relay fragments must be collocated where GraphQL is used, generated artifacts must be regenerated, typography wrappers must come from `@skedular/ui`, shared runtime helpers from `@skedular/shared`, and all customer/operator copy uses American spelling.
- [x] **V. Pattern Consistency** — The feature introduces a broader pricing catalog abstraction but evolves existing Organization offering patterns rather than replacing them. Justification: current `OfferingCode`, `OrganizationOffering`, renewal workflow, and GraphQL offering fields already express much of Teams pricing; extending them reduces migration risk and keeps ownership in Organization.
- [x] **VI. Logging** — Structured logging is required for pricing version/catalog selection, subscription creation/change, read-only compatibility decisions, active-user qualification, entitlement allow/block decisions, Contact Us threshold decisions, renewal scheduling, and failure/recovery paths.

## Project Structure

### Documentation (this feature)

```text
specs/027-teams-pricing-catalogue/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── graphql-pricing-catalogue.md
│   ├── openapi-pricing-catalogue.md
│   └── entitlement-contract.md
└── tasks.md
```

### Source Code (repository root)

```text
src/shared/Api.Shared.Services/
├── Models/
│   └── pricing/subscription/catalog DTOs and choice details
└── Offering/
    └── evolved V1 offering/catalog constants and mapping

src/organization/shared/Organization.Shared/
├── Database/Entities/
│   └── organization subscription/catalog version state
├── Repositories/
│   └── subscription, active-user, and read-only compatibility queries
├── Services/
│   └── pricing catalog, subscription, compatibility, quota, and entitlement services
└── Workflows/
    └── existing offering renewal workflow updates where required

src/organization/apis/Organization.Api/
├── GraphQL/Offering/
│   └── pricing catalog queries and subscription mutations/details
├── Services/
│   └── organization-facing subscription lifecycle orchestration
└── Mappers/
    └── catalog/subscription GraphQL mapping

src/team/apis/Team.Api/Services/Authorization/
src/location/apis/Location.Api/Services/Authorization/
src/booking/apis/Booking.Api/Services/Authorization/
└── use shared Api.Shared.Services entitlement logic against local event-projected JSON/projection state

api-definitions/
├── graphql/skedular/v1/schema.graphql
└── openapi/skedular/organization/*.yaml

src/web/apps/public-web/
├── src/data/pricing.ts
├── src/pages/pricing*.*
└── tests/public-site-content.test.ts

src/web/apps/webapp/
└── generated Relay artifacts if authenticated pricing/subscription management consumes new GraphQL fields
```

**Structure Decision**: Use Organization as the owning domain for Teams pricing catalog and new organization subscription support. Keep shared DTOs/enums and enforcement rules in `Api.Shared.Services` where they are cross-domain contracts. Team, Location, and Booking consume Organization-published events, store pricing/subscription state locally as JSON/projection data, and execute shared enforcement code locally. Public-web pricing rendering moves away from static hardcoded pricing data toward catalog responses or generated static catalog data sourced from backend contracts.

## Complexity Tracking

No constitution violations requiring complexity exceptions. The new catalog abstraction is justified as an evolution of the existing Organization offering model and as the minimum structure needed to make pricing server-driven and product-aware.

## Phase 0: Research

See [research.md](./research.md).

## Phase 1: Design and Contracts

See [data-model.md](./data-model.md), [contracts/graphql-pricing-catalogue.md](./contracts/graphql-pricing-catalogue.md), [contracts/openapi-pricing-catalogue.md](./contracts/openapi-pricing-catalogue.md), [contracts/entitlement-contract.md](./contracts/entitlement-contract.md), and [quickstart.md](./quickstart.md).

## Post-Design Constitution Check

- [x] **I. Contract-First** — Contracts identify GraphQL as the primary surface and OpenAPI as optional/secondary. Regeneration commands are documented in quickstart.
- [x] **II. Domain Boundaries** — Data model keeps subscription ownership in Organization and preserves the event projection plus shared enforcement-code boundary.
- [x] **III. Testing** — Quickstart and contracts define unit, integration, and web validation paths, including repository-layer persistence assertions.
- [x] **IV. Frontend** — Web plan removes static pricing constants and calls out generated Relay/OpenAPI discipline.
- [x] **V. Pattern Consistency** — Research documents extending V1 rather than creating a new catalog version now.
- [x] **VI. Logging** — Data model and quickstart include observable decisions and log verification.
