# Implementation Plan: Backend-Owned Marketplace Purchase Lifecycle History

**Branch**: `045-marketplace-purchase-history` | **Date**: 2026-08-29 | **Spec**: [spec.md](spec.md)
**Input**: Feature specification from `/specs/045-marketplace-purchase-history/spec.md`

## Summary

Replace the current mutable, one-row-per-purchase `MarketplacePurchaseHistory` projection with an append-only Booking-owned event stream for subscription and credit-entitlement purchases. Keep a derived current snapshot for the existing purchases list, expose a paginated backend event connection on eligible detail pages, and leave one-time booking details without a history tab.

## Technical Context

**Language/Version**: C# .NET 10 backend; TypeScript 6.0.3, React 19.2.6, Next.js 16.2.6 frontend
**Primary Dependencies**: Booking.Shared repositories/services, EF Core/PostgreSQL, HotChocolate/Fusion GraphQL, Temporal lifecycle workflows, Relay 21, Vitest, React Testing Library
**Storage**: Extend the existing Booking-owned `MarketplacePurchaseHistory` table with append-only event columns and retain its snapshot row; no production backfill required because there is no legacy production data
**Testing**: xUnit + AutoFixture/FakeItEasy unit tests; repository-layer integration tests; GraphQL contract/authorization tests; Vitest + React Testing Library
**Target Platform**: Booking API and customer/operator web applications
**Project Type**: Full-stack web service
**Performance Goals**: 95% of normal-scale history pages visible within 2 seconds; stable keyset pagination and deterministic order
**Constraints**: repository persistence boundary; CancellationToken last; explicit persisted-string-to-enum mappings; generated GraphQL/Relay outputs regenerated, never hand-edited; no raw EF in integration assertions; American English; no secrets in payloads/logs
**Scale/Scope**: All eligible marketplace subscriptions and credit entitlements across Host and Spaces organizations; one-time bookings remain out of detail history
**Scale/Scope**: All eligible marketplace subscriptions and credit entitlements across Host and Spaces organizations; one-time bookings remain out of detail history

## Constitution Check

_Gate: Passes before Phase 0 research and after Phase 1 design._

- [x] **I. Contract-First** — GraphQL source changes are confined to `src/booking/apis/Booking.Api/schema.graphqls`; run `scripts/generate-graphql.sh`, then regenerate affected Relay artifacts with `pnpm --dir src/web relay`. Generated schema and Relay files are outputs only.
- [x] **II. Domain Boundaries** — Booking owns the event stream, reducer, snapshot, lifecycle coordination, and service/API model boundary. GraphQL resolvers call services only. Database event/source discriminators are strings, while domain/API contracts use enums connected by explicit switch mappings with documented unknown-value behavior; no `Enum.Parse`/`Enum.TryParse`.
- [x] **III. Testing** — Unit tests cover reducer, idempotency decisions, authorization decisions, event mapping, and lifecycle write-point coordination. Focused integration tests cover migration, uniqueness/concurrency, repository paging, and GraphQL schema wiring. Assertions use repository/query methods rather than DbContext.
- [x] **IV. Frontend** — History is added only to subscription and entitlement detail surfaces. Relay fragments/queries are colocated, generated artifacts are regenerated, Skedular typography wrappers are used, and American English copy is applied. This feature adds no mutation; reads use Relay refresh/refetch behavior and never `window.location.reload()`. Public documentation is reviewed; no update is required unless existing purchase-detail documentation describes the changed history surface.
- [x] **V. Pattern Consistency** — The append-only event stream is a deliberate extension of the existing Booking-owned projection: immutable facts provide auditability while the snapshot preserves the established list query. The repository remains the persistence seam and the service remains the authorization/mapping seam.
- [x] **VI. Logging** — Structured logs cover append attempts/results, idempotent replays, lifecycle transitions, ordered reads, snapshot reduction warnings, authorization failures, and recovery paths without payment credentials, tokens, or sensitive customer data.

## Project Structure

### Documentation

```text
specs/045-marketplace-purchase-history/
├── spec.md
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   ├── graphql.md
│   └── lifecycle-write-points.md
└── tasks.md
```

### Source Code

```text
src/booking/shared/Booking.Shared/
├── Database/Entities/MarketplacePurchaseHistory.cs
├── Models/MarketplacePurchaseHistory*.cs
├── Repositories/MarketplacePurchaseHistoryRepository.cs
└── Services/{MarketplaceBookingService,MarketplaceBookingSubscriptionService,Entitlements,MarketplaceRefund*}.cs

src/booking/apis/Booking.Api/
├── schema.graphqls
├── Services/MarketplacePurchaseHistoryService.cs
├── GraphQL/MarketplacePurchaseHistory/
└── Mappers/GraphQlMapper.cs

src/booking/domain/
├── Booking.Domain.IntegrationTests/Repositories/MarketplacePurchaseHistoryRepositoryShould.cs
└── Booking.Domain.IntegrationTests/schema.graphql

src/web/apps/webapp/
├── src/components/marketplacePurchaseHistory/
├── src/components/marketplaceEntitlement/
├── src/components/organizationStoreFrontGuest/
└── src/queries/__generated__/
```

**Structure Decision**: Extend the existing `MarketplacePurchaseHistory` table and its Booking shared model → repository → service → GraphQL mapper and web Relay layers. Do not add a new event table, domain, API, or customer-data store.

## Phase 0: Research Decisions

See [research.md](research.md). All planning unknowns are resolved before implementation tasks.

## Phase 1: Design Outputs

- [data-model.md](data-model.md) defines immutable event identity, payload, snapshot reduction, relationships, and invariants.
- [contracts/graphql.md](contracts/graphql.md) defines the service-facing GraphQL shape and eligible-source behavior.
- [contracts/lifecycle-write-points.md](contracts/lifecycle-write-points.md) maps authoritative lifecycle transitions to append calls.
- [quickstart.md](quickstart.md) defines migration, backend, GraphQL, frontend, and one-time booking validation.

## Delivery Sequence

1. Add event model/configuration and reducer tests.
2. Add migration and repository append/read/snapshot behavior.
3. Wire authoritative subscription, entitlement, payment, and refund transitions.
4. Add GraphQL service contract, authorization, pagination, and tests.
5. Add subscription/entitlement frontend rendering and one-time booking regressions.
6. Regenerate generated outputs, run focused/full validation, review documentation, and update graph metadata.

## Complexity Tracking

_No constitution violations requiring justification._
